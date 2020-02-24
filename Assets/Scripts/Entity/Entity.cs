using UnityEngine;

public abstract class Entity : MonoBehaviour, IAttack
{
    [SerializeField]
    protected int m_attackDamage = 1;
    [SerializeField]
    protected float m_moveSpeed = 10f;
    [SerializeField]
    protected float m_jumpForce = 22f;
    [SerializeField]
    protected bool m_isFacingRight;
    [SerializeField]
    protected BoxCollider2D m_weaponCollider;

    protected Animator m_animator;
    protected Rigidbody2D m_rigidbody2D;
    protected BoxCollider2D m_collider;
    protected EntityHealth m_entityHealth;

    protected int m_currentAttackingDirection = 0;
    protected static float m_ATTACK_DIRECTION_DEADZONE = 0.35f;
    protected static string[] m_ATTACK_ANIMATOR_PARAMETERS = { "AttackingUp", "Attacking", "AttackingDown" };
    public bool IsInputLocked { get; set; } = false;
    public bool Attacking { get; protected set; }

    protected virtual void Awake()
    {
        m_animator = gameObject.GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_collider = gameObject.GetComponent<BoxCollider2D>();
        m_entityHealth = gameObject.GetComponent<EntityHealth>();
        m_weaponCollider = gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    protected virtual void Start()
    {
       if (!m_isFacingRight)
        {
            FlipEntity();
        }
        m_weaponCollider.enabled = false;
        Attacking = false;
    }

    // This method flips the entity sprite.
    protected virtual void FlipEntity()
    {
        m_isFacingRight = !m_isFacingRight;
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
    }

    // This method resets the attack including the animator.
    protected void ResetAttackAnimation()
    {
        Attacking = false;
        foreach (string parameter in m_ATTACK_ANIMATOR_PARAMETERS)
        {
            m_animator.SetBool(parameter, false);
        }
    }

    public void ResetEntityActions()
    {
        m_entityHealth.Revive();
        ResetEntityAnimations();
        ResetMovement();
    }

    public virtual void ResetEntityAnimations()
    {
        m_animator.SetBool("IsJumping", false);
        m_animator.SetFloat("Speed", 0);
        Attacking = false;
    }

    public virtual void ResetMovement()
    {
        m_rigidbody2D.velocity = new Vector2(0, m_rigidbody2D.velocity.y);
    }

    // StartAttacking and StopAttacking are triggered by the attack animations
    // in order to mark the time window where the attack deals damage.
    public void StartAttacking()
    {
        m_weaponCollider.enabled = true;
        m_weaponCollider.isTrigger = true;
    }

    public virtual void StopAttacking()
    {
        m_weaponCollider.enabled = false;
        m_weaponCollider.isTrigger = false;
    }

    // Attacks cancel each other if they are on the same height, both are currently in the deal damage window
    // and the facing direction is not the same.
    public bool IsAttackCancelling(int attackDirectionFromOtherEntity, bool entityIsFacingRight)
    {
        return (attackDirectionFromOtherEntity == m_currentAttackingDirection) && m_weaponCollider.enabled
            && (entityIsFacingRight != m_isFacingRight);
    }
    
    public int GetAttackDirection()
    {
        return m_currentAttackingDirection;
    }

    public int GetAttackDamage()
    {
        return m_attackDamage;
    }

    public bool IsFacingRight()
    {
        return m_isFacingRight;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsInputLocked)
        {
            if (collider.gameObject.name == "DeathZones")
            {
                Die(true);
            }
            Vector2 colliderDirection = gameObject.transform.position - collider.gameObject.transform.position;
            bool attackerNeedsToFaceRight = colliderDirection.x > 0.0f ? true : false;
            IAttack enemyAttack = collider.gameObject.transform.parent.GetComponent<IAttack>();
            // Take damage if the collider comes from an attacker and the attacks are not cancelling each other.
            if (enemyAttack != null && (!IsAttackCancelling(enemyAttack.GetAttackDirection(), enemyAttack.IsFacingRight()
                && attackerNeedsToFaceRight == enemyAttack.IsFacingRight()))
                && collider.enabled)
            {
                m_entityHealth.TakeDamage(enemyAttack.GetAttackDamage());
                if (m_entityHealth.IsZero)
                {
                    Die(false);
                }
            }
        }
    }

    protected virtual void Die(bool isDeadByDeathZone){
        StopAttacking();
    }
}
