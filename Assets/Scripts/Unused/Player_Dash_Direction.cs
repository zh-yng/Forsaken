using UnityEngine;

public class Player_Dash_Direction : MonoBehaviour
{
    private Camera cam;
    private Vector3 mousePos;
    private Vector3 rotation;

    public Vector3 DashDirection {get {return rotation.normalized;} set {rotation = value;}}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        rotation = mousePos - transform.position;

        float rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

}
