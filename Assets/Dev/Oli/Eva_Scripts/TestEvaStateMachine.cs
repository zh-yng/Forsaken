using UnityEngine;
using System.Collections.Generic;
public class TestEvaStateMachine : StateMachine
{
    [SerializeField] private float followDistance;
    [SerializeField] private float timeInIdle;
    [SerializeField] private List<Transform> hidingSpots = new List<Transform>();
    // Use the child sprite reference found in Init()
    [Header("VFX")]
    [SerializeField] private ParticleSystem scaredParticles;

    private bool isFlipped = false;
    private bool isHurt = false; 
    private bool isTransitioning = false;
    private int hurtFinished = 0;
    private int health;
    
    
    public bool IsHurt{get {return isHurt;} set {isHurt = value;}}
    public bool IsTransitioning {get {return isTransitioning;} set {isTransitioning = value;}}
    public int HurtFinished {get {return hurtFinished; } set {hurtFinished = value;}}
    public int Health {get {return health;} set {health = value;}}
    public ParticleSystem ScaredParticles {get {return scaredParticles;}}
    // This allows states to read the current state, but not change it directly
    // public State CurrentState {get {return currentState;}}
    public float FollowDistance {get {return followDistance;}}
    // public float MoveSpeed {get {return moveSpeed;}}
    public Transform TargetHideSpot { get; set; }

    public List<Transform> GetHidingSpots() {return hidingSpots;}

    protected override void Init()
    {
        base.Init();
        sprite = transform.Find("Sprite");
        Health = 100;
        if (scaredParticles == null) 
        scaredParticles = GetComponentInChildren<ParticleSystem>();
    }

    protected override void EnterBeginningState()
    {
        IsTransitioning = false;
        currentState = new TestEvaIdleState(this);
        currentState.EnterStates();
    }

    protected override void UpdateState()
    {
        HandleHideInput();
        
        if (!IsTransitioning)
        {
            rb.linearVelocity = appliedMovement;
        }
        currentState.UpdateStates();
    }

    protected override void FaceMovement()
    {
        Transform currentTarget = player.transform;
        //Determine if Eva should be facing player or hiding spot
        if (currentState is TestEvaMoveToHideState && TargetHideSpot != null)
        {
            currentTarget = TargetHideSpot;
        }

        Vector3 flipped = sprite.localScale;
        flipped.x *= -1f;
        if (sprite.position.x < currentTarget.position.x && isFlipped)
        {
            sprite.localScale = flipped;
            isFlipped = false;
        } else if (sprite.position.x > currentTarget.position.x && !isFlipped)
        {
            sprite.localScale = flipped;
            isFlipped = true;
        }
    }

    public bool FollowRange()
    {
        return Vector3.Distance(transform.position,Player.transform.position) >= FollowDistance;
    }


    //Handle Hide Input and trigger hiding behavior
    public void HandleHideInput()
    {
        if (Input.GetKey(KeyCode.J))
        {
            //quit hidden or moving to hidespot if pressed j
            //Debug.Log("J key was pressed! Current frame: " + Time.frameCount);
            if (currentState is TestEvaHiddenState || currentState is TestEvaMoveToHideState)
            {
                //Debug.Log("J, Off, switching to idle.");
                currentState.SwitchState(new TestEvaIdleState(this));
                if (scaredParticles != null) scaredParticles.Stop();
            }
            else //idle/follow then start hiding if pressed j
            {
                //Debug.Log("J, On, starting to hide.");
                StartHiding();
                if (scaredParticles != null) scaredParticles.Play();
            }
        }
    }
    

    //Function to find closest hiding spot and switch to move to hide state
    public void StartHiding()
    {

        if (hidingSpots.Count == 0) return;

        float closestDistance = Mathf.Infinity;
        Transform closestSpot = null;

        foreach (Transform spot in hidingSpots)
        {
            float distance = Vector3.Distance(transform.position, spot.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSpot = spot;
            }
        }

        if (closestSpot != null)
        {
            TargetHideSpot = closestSpot;
            currentState.SwitchState(new TestEvaMoveToHideState(this));
        }
            
    }

    //Debug code

    // private void OnGUI()
    // {
    //     // This will stay on your screen in the top-left
    //     string stateInfo = currentState != null ? currentState.GetType().Name : "No State";
    //     GUI.Box(new Rect(10, 10, 250, 50), "EVA DEBUGGER");
    //     GUI.Label(new Rect(20, 30, 200, 20), "State: " + stateInfo);
        
    //     if (TargetHideSpot != null)
    //         GUI.Label(new Rect(20, 45, 200, 20), "Target: " + TargetHideSpot.name);
    // }
    
}