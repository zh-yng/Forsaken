using Unity.VisualScripting;
using UnityEngine;
using System;
public class CrowStateMachine : StateMachine, IDamageable
{
    [Header("Attack Controls")]
    [SerializeField] private float targetDistance;
    [SerializeField] private float aggroDistance;
    [SerializeField] private float stunTime;
    [SerializeField] private float stunInterval;
    [SerializeField] private int damage;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float jumpForceX;
    [SerializeField] private float jumpForceY;
    [SerializeField] private int maxHealth = 50;

    private bool isFlipped = false;
    private bool isStunned = false;
    private bool inAttack = false;
    private bool windUpFinished = true;

    private int health;
    private ParticleSystem damageTakenParticles;
    public bool IsStunned { get { return isStunned; } set { isStunned = value; } }
    public bool WindUpFinished { get { return windUpFinished; } set { windUpFinished = value; } }
    public bool InAttack { get { return inAttack; } set { inAttack = value; } }
    public bool Flipped { get { return isFlipped; } }
    public int Health { get { return health; } set { health = value; } }
    public int Damage { get { return damage; } set { damage = value; } }
    public Vector2 JumpForce { get { return new Vector2(jumpForceX, jumpForceY); } }
    public float Cooldown { get { return damageCooldown; } set { damageCooldown = value; } }
    public float StunTime { get { return stunTime; } }
    public float StunInterval { get { return stunInterval; } }
    public float TargetDistance { get { return targetDistance; } }
    public float AggroDistance { get { return aggroDistance; } set { aggroDistance = value; } }

    public Action<CrowStateMachine> CrowDeath;

    protected override void Init()
    {
        base.Init();
        sprite = transform.Find("Sprite");
        Health = maxHealth;
        damageTakenParticles = sprite.Find("hit received particles").GetComponent<ParticleSystem>();
    }

    protected override void EnterBeginningState()
    {
        currentState = new CrowStartState(this);
        currentState.EnterStates();
    }

    protected override void UpdateState()
    {
        Debug.Log(currentState);
        rb.linearVelocity = appliedMovement;
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
        }
        else if (sprite.position.x > player.transform.position.x && !isFlipped)
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
            CrowDeath?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    public bool InRange()
    {
        return (transform.position - Player.transform.position).magnitude <= TargetDistance;
    }
    public bool InAggroRange()
    {
        return (transform.position - Player.transform.position).magnitude <= aggroDistance;
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

    public void Attack()
    {
        currentState.SwitchState(new CrowWalkState(this));
    }


}