using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

/// <summary></summary>
public abstract class Entity : MonoBehaviour, IAttack
{
    /// <summary>The m weapon collider</summary>
    [SerializeField]
    protected BoxCollider2D m_weaponCollider;
    /// <summary>The m is facing right</summary>
    [SerializeField]
    protected bool m_isFacingRight;
    [SerializeField]
    private float m_distanceInWhichEntityCountsAsGrounded = 0.1f;
    /// <summary>The m jump force</summary>
    [SerializeField]
    protected float m_jumpForce = 22.0f;
    /// <summary>The m move speed</summary>
    [SerializeField]
    protected float m_moveSpeed = 10.0f;
    /// <summary>The m attack damage</summary>
    [SerializeField]
    protected int m_attackDamage = 1;
    [SerializeField]
    private LayerMask m_whatIsGround;

    /// <summary>The m animator</summary>
    protected Animator m_animator;
    /// <summary>The m rigidbody2 d</summary>
    protected Rigidbody2D m_rigidbody2D;
    /// <summary>The m collider</summary>
    protected BoxCollider2D m_collider;
    /// <summary>The m entity health</summary>
    protected EntityHealth m_entityHealth;

    /// <summary>The m is grounded</summary>
    protected bool m_isGrounded = false;
    /// <summary>The m current attacking direction</summary>
    protected int m_currentAttackingDirection = 0;
    /// <summary>The horizontal is grounded distance</summary>
    protected static readonly float HORIZONTAL_IS_GROUNDED_DISTANCE = 0.1f;
    /// <summary>The attack animator parameter names</summary>
    protected static readonly string[] ATTACK_ANIMATOR_PARAMETER_NAMES = { "AttackingUp", "Attacking", "AttackingDown" };
    /// <summary>The jumping animator parameter name</summary>
    protected static readonly string JUMPING_ANIMATOR_PARAMETER_NAME = "IsJumping";
    /// <summary>The speed animator parameter name</summary>
    protected static readonly string SPEED_ANIMATOR_PARAMETER_NAME = "Speed";

    /// <summary>The death zones name</summary>
    public static readonly string DEATH_ZONES_NAME = "DeathZones";
    /// <summary>The invalid position</summary>
    public static readonly Vector2 INVALID_POSITION = new Vector2(-99999999, -99999999);
    /// <summary>Gets or sets a value indicating whether this instance is input locked.</summary>
    /// <value>
    ///   <c>true</c> if this instance is input locked; otherwise, <c>false</c>.</value>
    public bool IsInputLocked { get; set; } = false;
    /// <summary>Gets or sets a value indicating whether this <see cref="Entity"/> is attacking.</summary>
    /// <value>
    ///   <c>true</c> if attacking; otherwise, <c>false</c>.</value>
    public bool Attacking { get; protected set; }
    /// <summary>Gets or sets a value indicating whether this instance is rolling.</summary>
    /// <value>
    ///   <c>true</c> if this instance is rolling; otherwise, <c>false</c>.</value>
    public bool IsRolling { get; protected set; } = false;

    /// <summary>Awakes this instance.</summary>
    protected virtual void Awake()
    {
        m_animator = gameObject.GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_collider = gameObject.GetComponent<BoxCollider2D>();
        m_entityHealth = gameObject.GetComponent<EntityHealth>();
        m_weaponCollider = gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    /// <summary>Starts this instance.</summary>
    protected virtual void Start()
    {
        if (!m_isFacingRight)
        {
            FlipEntity();
        }
        m_weaponCollider.enabled = false;
        Attacking = false;
    }

    /// <summary>Updates this instance.</summary>
    protected virtual void Update() 
    {
        UpdateIsGrounded();
        m_animator.SetBool(JUMPING_ANIMATOR_PARAMETER_NAME, !m_isGrounded);
    }

    /// <summary>Updates the is grounded.</summary>
    protected void UpdateIsGrounded()
    {
        m_isGrounded = Physics2D.OverlapArea((Vector2) m_collider.bounds.min - new Vector2(HORIZONTAL_IS_GROUNDED_DISTANCE, 0.0f),
                        (Vector2)m_collider.bounds.min + new Vector2(m_collider.bounds.size.x + HORIZONTAL_IS_GROUNDED_DISTANCE, m_distanceInWhichEntityCountsAsGrounded), m_whatIsGround);
    }

    // This method flips the entity sprite.
    /// <summary>Flips the entity.</summary>
    protected virtual void FlipEntity()
    {
        m_isFacingRight = !m_isFacingRight;
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
    }

    /// <summary>Dies this instance.</summary>
    protected virtual void Die()
    {
        StopAttacking();
    }

    /// <summary>Called when [trigger enter2 d].</summary>
    /// <param name="collider">The collider.</param>
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsInputLocked)
        {
            if (collider.gameObject.name == DEATH_ZONES_NAME)
            {
                Die();
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
                    Die();
                }
            }
        }
    }

    /// <summary>Called when [jump].</summary>
    /// <param name="value">The value.</param>
    protected void OnJump(InputValue value)
    {
        if (!IsInputLocked && !IsRolling && m_isGrounded)
        {
            m_animator.SetBool(JUMPING_ANIMATOR_PARAMETER_NAME, true);
            m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, m_jumpForce);
        }
    }

    // This method resets the attack including the animator.
    /// <summary>Resets the attack animation.</summary>
    protected void ResetAttackAnimation()
    {
        Attacking = false;
        foreach (string parameter in ATTACK_ANIMATOR_PARAMETER_NAMES)
        {
            m_animator.SetBool(parameter, false);
        }
    }

    /// <summary>Resets the entity animations.</summary>
    public virtual void ResetEntityAnimations()
    {
        m_animator.SetBool(JUMPING_ANIMATOR_PARAMETER_NAME, false);
        m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, 0);
        foreach (string attack_parameter in ATTACK_ANIMATOR_PARAMETER_NAMES)
        {
            m_animator.SetBool(attack_parameter, false);
        }
        Attacking = false;
    }

    /// <summary>Resets the movement.</summary>
    public virtual void ResetMovement()
    {
        m_rigidbody2D.velocity = new Vector2(0, m_rigidbody2D.velocity.y);
    }

    // StartAttacking and StopAttacking are triggered by the attack animations
    // in order to mark the time window where the attack deals damage.
    /// <summary>Starts the attacking.</summary>
    public void StartAttacking()
    {
        m_weaponCollider.enabled = true;
        m_weaponCollider.isTrigger = true;
    }

    /// <summary>Stops the attacking.</summary>
    public virtual void StopAttacking()
    {
        m_weaponCollider.enabled = false;
        m_weaponCollider.isTrigger = false;
    }

    /// <summary>Resets the entity actions.</summary>
    public void ResetEntityActions()
    {
        m_entityHealth.Revive();
        ResetEntityAnimations();
        ResetMovement();
    }

    /// <summary>Determines whether [is facing right].</summary>
    /// <returns>
    ///   <c>true</c> if [is facing right]; otherwise, <c>false</c>.</returns>
    public bool IsFacingRight()
    {
        return m_isFacingRight;
    }

    // Attacks cancel each other if they are on the same height, both are currently in the deal damage window
    // and the facing direction is not the same.
    /// <summary>Determines whether [is attack cancelling] [the specified attack direction from other entity].</summary>
    /// <param name="attackDirectionFromOtherEntity">The attack direction from other entity.</param>
    /// <param name="entityIsFacingRight">if set to <c>true</c> [entity is facing right].</param>
    /// <returns>
    ///   <c>true</c> if [is attack cancelling] [the specified attack direction from other entity]; otherwise, <c>false</c>.</returns>
    public bool IsAttackCancelling(int attackDirectionFromOtherEntity, bool entityIsFacingRight)
    {
        return (attackDirectionFromOtherEntity == m_currentAttackingDirection) && m_weaponCollider.enabled
            && (entityIsFacingRight != m_isFacingRight);
    }

    /// <summary>Gets the attack direction.</summary>
    /// <returns></returns>
    public int GetAttackDirection()
    {
        return m_currentAttackingDirection;
    }

    /// <summary>Gets the attack damage.</summary>
    /// <returns></returns>
    public int GetAttackDamage()
    {
        return m_attackDamage;
    }
}
