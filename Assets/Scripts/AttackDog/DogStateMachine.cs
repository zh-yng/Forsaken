using Unity.VisualScripting;
using UnityEngine;
public class DogStateMachine : StateMachine, IDamageable
{
    [Header("Object References")]
    [SerializeField] private GameManager manager;

    [Header("Attack Controls")]
    [SerializeField] private float targetDistance;
    [SerializeField] private float stunTime;
    [SerializeField] private float stunInterval;
    [SerializeField] private int damage;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float jumpForceX;
    [SerializeField] private float jumpForceY;
    
    private bool isFlipped = false;
    private bool isStunned = false;
    private bool inAttack = false;
    private bool onGround = false;
    private bool windUpFinished = true;
    private int hurtFinished = 0;
    private int introFinished = 0;
    private int health;
    private ParticleSystem damageTakenParticles;
    public bool FightStarted {get {return manager.FightStarted;}}
    public bool IsStunned {get {return isStunned;} set {isStunned = value;}}
    public bool IsTransitioning {get {return manager.IsTransitioning;} set {manager.IsTransitioning = value;}}
    public bool WindUpFinished { get {return windUpFinished;} set { windUpFinished = value; } }
    public bool InAttack {get {return inAttack; } set {inAttack = value;}}
    public bool OnGround {get { return onGround; } set { onGround = value; } }
    public bool Flipped { get {return isFlipped;}}
    public int HurtFinished {get {return hurtFinished; } set {hurtFinished = value;}}
    public int IntroFinished {get {return introFinished; } set {introFinished = value;}}
    public int Health {get {return health;} set {health = value;}}
    public int Damage {get {return damage;} set {damage = value;}}
    public Vector2 JumpForce {get {return new Vector2(jumpForceX, jumpForceY);}}
    public float Cooldown {get {return damageCooldown;} set {damageCooldown = value;}}
    public float StunTime {get {return stunTime;}}
    public float StunInterval {get {return stunInterval;}}
    public float TargetDistance {get {return targetDistance;}}
    public int CurrentStage {get {return manager.CurrentStage;} set {manager.CurrentStage = value;}}

    protected override void Init()
    {
        base.Init();
        sprite = transform.Find("Sprite");
        Health = 100;
        damageTakenParticles = sprite.Find("hit received particles").GetComponent<ParticleSystem>();
    }

    protected override void EnterBeginningState()
    {
        IsTransitioning = false;
        currentState = new DogStartState(this);
        currentState.EnterStates();
    }

    protected override void UpdateState()
    {
        if (!IsTransitioning)
        {
            if (!inAttack)
            {
                rb.linearVelocity = appliedMovement;
            }
            else
            {
                rb.AddForce(appliedMovement, ForceMode2D.Impulse);
            }
        }
        currentState.UpdateStates();
    }

    protected override void FaceMovement()
    {
        Vector3 flipped = sprite.localScale;
        flipped.x *= -1f;
        if (sprite.position.x < player.transform.position.x && isFlipped)
        {
            sprite.localScale = flipped;
            isFlipped = false;
        } else if (sprite.position.x > player.transform.position.x && !isFlipped)
        {
            sprite.localScale = flipped;
            isFlipped = true;
        }
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.gameObject.GetComponent<PlayerStateMachine>().ApplyDamage(Damage);
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    public void flashCharacter()
    {
        sprite.GetComponent<DamageFlash>().BeginFlash();
    }

    public void ApplyDamage(int damage)
    {
        Health -= damage;
        Debug.Log("Enemy Health: " + Health);
        flashCharacter();
        damageTakenParticles.Play();
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public bool InRange()
    {
        return Vector3.Distance(transform.position,Player.transform.position) <= TargetDistance;
    }

    public void onWindupStart()
    {
        windUpFinished = false;
    }

    public void onWindupEnd()
    {
        windUpFinished = true;
    }

    public void OnAttackStart()
    {
        inAttack = true;

    }

    public void OnAttackEnd()
    {
        inAttack = false;
    }
    

}