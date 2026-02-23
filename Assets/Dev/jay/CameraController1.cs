using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraController1 : MonoBehaviour
{
    public static CameraController1 I { get; private set; }

    [Serializable]
    public class RigData
    {
        public CameraRig rig;
        public Transform rigRoot;
        public Camera dummyCamera;

        [NonSerialized] public float x;
        [NonSerialized] public Vector3 localOffset;
        [NonSerialized] public Vector3 worldPos;
        [NonSerialized] public float orthoSize;
        [NonSerialized] public CameraRig.SpaceMode spaceMode;
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera runtimeCamera;

    [Header("Bounds")]
    [SerializeField] private bool clampToExtremeRigs = true;


    [Header("Rig discovery")]
    [SerializeField] private bool autoFindRigs = true;
    [SerializeField] private List<CameraRig> rigsManual = new();

    [Header("Smoothing")]
    [Tooltip("0 = instant. Higher = smoother.")]
    [SerializeField] private float positionSmoothTimeX = 0.10f;

    [Tooltip("0 = instant. Higher = smoother.")]
    [SerializeField] private float positionSmoothTimeY = 0.10f;

    [Tooltip("0 = instant. Higher = smoother.")]
    [SerializeField] private float sizeSmoothTime = 0.10f;

    [Header("Runtime Z")]
    [SerializeField] private float cameraZ = 0f;

    private readonly List<RigData> _rigs = new();

    private float _velX;
    private float _velY;
    private float _sizeVel;

    public float PrevPlayerX { get; private set; }
    public float CurrPlayerX { get; private set; }

    private void Reset()
    {
        runtimeCamera = GetComponentInChildren<Camera>();
    }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;

        if (!runtimeCamera) runtimeCamera = Camera.main;

        if (cameraZ == 0f && runtimeCamera)
            cameraZ = runtimeCamera.transform.position.z;

        RebuildRigCache();

        if (player) PrevPlayerX = CurrPlayerX = player.position.x;
    }

    private void OnDestroy()
    {
        if (I == this) I = null;
    }

    private void OnValidate()
    {
        if (!runtimeCamera) runtimeCamera = GetComponentInChildren<Camera>();
        if (Application.isPlaying) return;
        RebuildRigCache();
    }

    private void LateUpdate()
    {
        if (!player || !runtimeCamera) return;
        if (_rigs.Count == 0) return;

        PrevPlayerX = CurrPlayerX;
        CurrPlayerX = player.position.x;

        ApplySample(CurrPlayerX);
    }
    private void GetBounds(out Vector3 leftPos, out float leftOrtho, out Vector3 rightPos, out float rightOrtho)
    {
        var left = _rigs[0];
        var right = _rigs[^1];

        leftPos = EvaluateRigPose(left);
        rightPos = EvaluateRigPose(right);

        leftOrtho = left.orthoSize;
        rightOrtho = right.orthoSize;
    }

    public void RebuildRigCache()
    {
        _rigs.Clear();

        List<CameraRig> found = new();

        if (autoFindRigs)
            found.AddRange(FindObjectsByType<CameraRig>(FindObjectsSortMode.InstanceID));
        else
            found.AddRange(rigsManual);

        foreach (var rig in found)
        {
            if (!rig) continue;

            var dummy = rig.dummyCamera ? rig.dummyCamera : rig.GetComponentInChildren<Camera>(true);
            if (!dummy) continue;

            var d = new RigData
            {
                rig = rig,
                rigRoot = rig.transform,
                dummyCamera = dummy
            };

            CacheRig(d);
            _rigs.Add(d);
        }

        _rigs.Sort((a, b) => a.x.CompareTo(b.x));
    }

    private void CacheRig(RigData d)
    {
        d.x = d.rigRoot.position.x;
        d.localOffset = d.dummyCamera.transform.localPosition;
        d.worldPos = d.dummyCamera.transform.position;
        d.orthoSize = d.dummyCamera.orthographic
            ? d.dummyCamera.orthographicSize
            : runtimeCamera.orthographicSize;
        d.spaceMode = d.rig.spaceMode;
    }

    private void ApplySample(float playerX)
    {
        for (int i = 0; i < _rigs.Count; i++)
            CacheRig(_rigs[i]);

        Vector3 desiredPos;
        float desiredOrtho;

        if (_rigs.Count == 1)
        {
            var r = _rigs[0];
            desiredPos = EvaluateRigPose(r);
            desiredOrtho = r.orthoSize;
            ApplyPose(ClampIfNeeded(desiredPos, desiredOrtho, out desiredOrtho), desiredOrtho);
            return;
        }

        int right = FindFirstRigRightOfX(playerX);

        if (right <= 0)
        {
            var r = _rigs[0];
            desiredPos = EvaluateRigPose(r);
            desiredOrtho = r.orthoSize;
            ApplyPose(ClampIfNeeded(desiredPos, desiredOrtho, out desiredOrtho), desiredOrtho);
            return;
        }

        if (right >= _rigs.Count)
        {
            var r = _rigs[^1];
            desiredPos = EvaluateRigPose(r);
            desiredOrtho = r.orthoSize;
            ApplyPose(ClampIfNeeded(desiredPos, desiredOrtho, out desiredOrtho), desiredOrtho);
            return;
        }

        var a = _rigs[right - 1];
        var b = _rigs[right];

        float t = Mathf.InverseLerp(a.x, b.x, playerX);

        Vector3 posA = EvaluateRigPose(a);
        Vector3 posB = EvaluateRigPose(b);

        desiredPos = Vector3.LerpUnclamped(posA, posB, t);
        desiredOrtho = Mathf.LerpUnclamped(a.orthoSize, b.orthoSize, t);

        desiredPos = ClampIfNeeded(desiredPos, desiredOrtho, out desiredOrtho);
        ApplyPose(desiredPos, desiredOrtho);
    }

    private Vector3 ClampIfNeeded(Vector3 desiredPos, float desiredOrtho, out float clampedOrtho)
    {
        clampedOrtho = desiredOrtho;
        if (!clampToExtremeRigs || _rigs.Count == 0) return desiredPos;

        GetBounds(out var leftPos, out var leftOrtho, out var rightPos, out var rightOrtho);

        // Clamp X between extreme rig poses
        desiredPos.x = Mathf.Clamp(desiredPos.x, leftPos.x, rightPos.x);

        // Optional: clamp zoom between extreme rig zooms (prevents “expanding past” the edge setups)
        clampedOrtho = Mathf.Clamp(desiredOrtho, Mathf.Min(leftOrtho, rightOrtho), Mathf.Max(leftOrtho, rightOrtho));

        return desiredPos;
    }


    private Vector3 EvaluateRigPose(RigData r)
    {
        Vector3 desired = r.spaceMode == CameraRig.SpaceMode.World
            ? r.worldPos
            : player.position + r.localOffset;

        desired.z = cameraZ;
        return desired;
    }

    private void ApplyPose(Vector3 desiredPos, float desiredOrtho)
    {
        Vector3 current = runtimeCamera.transform.position;

        float x = positionSmoothTimeX <= 0f
            ? desiredPos.x
            : Mathf.SmoothDamp(current.x, desiredPos.x, ref _velX, positionSmoothTimeX);

        float y = positionSmoothTimeY <= 0f
            ? desiredPos.y
            : Mathf.SmoothDamp(current.y, desiredPos.y, ref _velY, positionSmoothTimeY);

        runtimeCamera.transform.position = new Vector3(x, y, cameraZ);

        if (!runtimeCamera.orthographic) return;

        runtimeCamera.orthographicSize =
            sizeSmoothTime <= 0f
                ? desiredOrtho
                : Mathf.SmoothDamp(runtimeCamera.orthographicSize, desiredOrtho, ref _sizeVel, sizeSmoothTime);
    }

    private int FindFirstRigRightOfX(float x)
    {
        for (int i = 0; i < _rigs.Count; i++)
            if (_rigs[i].x >= x) return i;

        return _rigs.Count;
    }
}
