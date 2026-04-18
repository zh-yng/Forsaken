using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public abstract class StateMachine : MonoBehaviour
{
    //control variables
    [SerializeField] private  float moveSpeed = 5f;

    //Game Objects
    private Animator animator;
    protected Rigidbody2D rb;
    protected GameObject player;
    protected Transform sprite;
    protected Vector3 appliedMovement;

    //States
    protected State currentState;
    protected bool isParryStunned = false;

    //getters and settesr
    public State CurrentState {get {return currentState; } set {currentState = value;}}
    public Animator Anim {get {return animator;}}
    public Rigidbody2D RB {get {return rb;}}
    public GameObject Player {get {return player;}}
    public Transform Sprite {get {return sprite;}}
   
    public float AppliedMovementX {get {return appliedMovement.x;} set {appliedMovement.x = value;}}
    public float AppliedMovementY {get {return appliedMovement.y;} set {appliedMovement.y = value;}}
    public float MoveSpeed {get {return moveSpeed;} set {moveSpeed = value;}}
    public bool IsParryStunned {get {return isParryStunned;}}

    public void Awake()
    {
        Init();
        EnterBeginningState();
    }

    protected virtual void Init()
    {
        //set reference variables
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sprite = transform.Find("sprite");
    }

    protected virtual void EnterBeginningState(){}

    // Update is called once per frame
    public void FixedUpdate()
    {
        UpdateState();
        FaceMovement();    
    }

    protected virtual void UpdateState()
    {
        currentState.UpdateStates();
        rb.linearVelocity = appliedMovement;
    }

    public void JumpToState(State state)
    {
        currentState.ExitState();
        currentState = state;
        currentState.EnterStates();
    }

    protected virtual void FaceMovement()
    {
        if (rb.linearVelocity.x != 0)
        {
            sprite.localScale = new Vector3(Mathf.Sign(rb.linearVelocity.x), 1, 1);
        }
    }

    public void Stun(float time, float rate)
    {
        Debug.Log("stunned");
        BeginSlowDown(time, rate);
    }

    private void BeginSlowDown(float time, float rate)
    {
        StartCoroutine(SlowDown(time, rate));
    }

    public IEnumerator SlowDown(float time, float rate)
    {
        isParryStunned = true;
        moveSpeed /= rate;
        animator.speed /= rate;

        yield return new WaitForSecondsRealtime(time);

        moveSpeed *= rate;
        animator.speed = 1f;
        isParryStunned = false;
    }
}
