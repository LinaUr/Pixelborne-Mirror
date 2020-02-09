using System.Collections;
using System.Collections.Generic;
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
    protected int m_currentAttackingDirection = 0;
    protected Animator m_animator;
    protected Rigidbody2D m_rigidbody2D;
    protected BoxCollider2D m_collider;
    protected EntityHealth m_entityHealth;
    [SerializeField]
    protected BoxCollider2D m_weaponCollider;
    public bool Attacking { get; protected set; }
    protected static float m_ATTACK_DIRECTION_DEADZONE = 0.35f;
    protected static string[] m_ATTACK_ANIMATOR_PARAMETERS = { "AttackingUp", "Attacking", "AttackingDown" };
    public bool InputIsLocked { get; set; } = false;

    protected virtual void Awake()
    {
        m_animator = gameObject.GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_collider = gameObject.GetComponent<BoxCollider2D>();
        m_entityHealth = gameObject.GetComponent<EntityHealth>();
        m_weaponCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
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

    // This method flips the enemy sprite.
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
        foreach(string parameter in m_ATTACK_ANIMATOR_PARAMETERS)
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

    // These methods are triggered by the attack animations
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

    // Attacks cancel each other if the are on the same height, both are currently in the deal damage window
    // and the facing direction is not the same.
    public bool AttackIsCancelling(int attackDirectionFromOtherEntity, bool entityIsFacingRight)
    {
        return attackDirectionFromOtherEntity == m_currentAttackingDirection && m_weaponCollider.enabled
            && entityIsFacingRight != m_isFacingRight;
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
        if (!InputIsLocked)
        {
            if (collider.gameObject.name == "DeathZones")
            {
                Die();
            }
            Vector2 colliderDirection = transform.position - collider.transform.position;
            bool attackerNeedsToFaceRight = colliderDirection.x > 0.0f ? true : false;
            IAttack enemyAttack = collider.transform.parent.GetComponent<IAttack>();
            // Take damage if the collider comes from an attacker and the attacks are not cancelling each other.
            if (enemyAttack != null && !AttackIsCancelling(enemyAttack.GetAttackDirection(), enemyAttack.IsFacingRight()
            && attackerNeedsToFaceRight == enemyAttack.IsFacingRight())
                && collider.enabled)
            {
                if(GameMediator.Instance.ActivePlayers[0] == this || GameMediator.Instance.ActivePlayers[1] == this) {
                    Debug.Log(enemyAttack);
                    Debug.Log(this);
                }
                m_entityHealth.TakeDamage(enemyAttack.GetAttackDamage());
                if (m_entityHealth.IsDead)
                {
                    Die();
                }
            }
        }
    }

    // Method definitions
    protected virtual void Die(){
        StopAttacking();
    }

}
