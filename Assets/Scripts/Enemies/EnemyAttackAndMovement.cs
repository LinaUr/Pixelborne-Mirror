using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAndMovement : MonoBehaviour, IEnemyAttackAndMovement
{
    [SerializeField]
    private float m_moveSpeed = 10f;
    [SerializeField]
    private int m_attackDamage = 1;
    [SerializeField]
    private bool m_facingRight;
    private Rigidbody2D m_rigidbody2D;
    private BoxCollider2D m_weaponCollider;
    private BoxCollider2D m_enemyCollider;
    private EntityHealth m_enemyHealth;
    private Animator m_animator;
    private Rigidbody2D m_playerRigidbody2D;
    private bool m_isFollowingPlayer = false;
    private bool m_isAttackChained = false;
    private string m_playerSwordName;
    private int m_nextAttackingDirection;
    
    private static string[] m_ATTACK_ANIMATOR_PARAMETERS = {"AttackingUp", "Attacking", "AttackingDown"};
    private static string[] m_ATTACK_ANIMATOR_ANIMATION_NAMES = {"attack_up", "attack_mid", "attack_down"};

    public bool InputIsLocked { get; set; } = false;

    void Awake() {
        m_animator = gameObject.GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        m_enemyHealth = gameObject.GetComponent<EntityHealth>();
        m_weaponCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        m_weaponCollider.enabled = false; 
    }

    void Start()
    {
        GameObject player = GameMediator.Instance.ActiveGame.GetActivePlayers()[0];
        m_playerRigidbody2D = player.GetComponent<Rigidbody2D>();
        m_playerSwordName = player.GetComponent<EntityAttack>().PlayerSword.name;
    }

    void Update()
    {
        if (m_isFollowingPlayer)
        {
            Vector2 playerPosition = m_playerRigidbody2D.position;
            float movementDirection = m_playerRigidbody2D.position.x - m_rigidbody2D.position.x;
            // Normalize the movemetDirection.
            movementDirection = movementDirection < 0 ? -1 : 1;
            m_animator.SetFloat("Speed", Mathf.Abs(movementDirection));

            // Enemy Direction.
            if (movementDirection < 0.0f && m_facingRight)
            {
                FlipEnemy();
            }
            else if (movementDirection > 0.0f && !m_facingRight)
            {
                FlipEnemy();
            }

            // Physics.
            m_rigidbody2D.velocity = new Vector2(movementDirection * m_moveSpeed, m_rigidbody2D.velocity.y);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!InputIsLocked)
        {
            if (collider.gameObject.name == "DeathZones")
            {
                Die();
            }
            // We have to explicitly look for a the player sword since friendly fire is not desired.
            if (collider.gameObject.name == m_playerSwordName)
            {
                IAttack attackersAttack = collider.gameObject.GetComponentInParent<IAttack>();
                // Take damage if the collider comes from an attacker and the attacks are not cancelling each other.
                if (attackersAttack != null && !AttackIsCancelling(attackersAttack.GetAttackDirection()))
                {
                    m_enemyHealth.TakeDamage(attackersAttack.GetAttackDamage());
                    if (m_enemyHealth.IsDead)
                    {
                        Die();
                    }
                }
            }
        }
    }

    // Attacks cancel each other if the are on the same height, both are currently in the deal damage window
    // and the facing direction is not the same.
    public bool AttackIsCancelling(int attackDirectionFromOtherEntity)
    {
        return attackDirectionFromOtherEntity == m_nextAttackingDirection && m_weaponCollider.enabled;
    }


    public int GetAttackDirection()
    {
        return m_nextAttackingDirection;
    }

    public int GetAttackDamage()
    {
        return m_attackDamage;
    }

    public bool IsFacingRight()
    {
        return m_facingRight;
    }


    // This method initiates the entity dying animation.
    private void Die(){
        m_animator.SetBool("IsDying", true);
    }

    // This method destroys the Game Object.
    // Is called at the end of the death animation.
    private void DestroyObject(){
        Destroy(gameObject);
    }

    // This method flips the enemy sprite.
    private void FlipEnemy()
    {
        m_facingRight = !m_facingRight;
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
    }

    public void ResetEnemyActions()
    {
        m_enemyHealth.Revive();
        ResetEnemyAnimations();
        ResetMovement();
    }

    public void ResetEnemyAnimations()
    {
        m_animator.SetBool("IsJumping", false);
        m_animator.SetFloat("Speed", 0);
        m_animator.SetBool("Rolling", false);
        foreach(string parameter in m_ATTACK_ANIMATOR_PARAMETERS)
        {
            m_animator.SetBool(parameter, false);
        }
    }

    public void ResetMovement()
    {
        m_rigidbody2D.velocity = new Vector2(0, m_rigidbody2D.velocity.y);
    }

    // This method starts the new attack.
    // This rather inconvenient approach is needed in order to avoid a 
    // that takes place when attacks are directly chained by the AttackPatternExecutor.
    private void startAttackIfPossible(int attackDirectionIndex)
    {
        if(!InputIsLocked)
        {
            m_nextAttackingDirection = attackDirectionIndex;
            if(!m_isAttackChained)
            {
                m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[m_nextAttackingDirection], true);
            }
            m_isAttackChained = true;
        }
    }

    // These method implement the IEnemyAttackAndMovement that is needed by the AttackPatternExecutor.
    public void AttackUp()
    {
        startAttackIfPossible(0);
    }

    public void AttackMiddle()
    {
        startAttackIfPossible(1);
    }

    public void AttackDown()
    {
        startAttackIfPossible(2);
    }

    public void StartFollowPlayer()
    {
        m_isFollowingPlayer = true;
    }

    public void StopFollowPlayer()
    {
        m_isFollowingPlayer = false;
    }

    public float GetAttackUpDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, m_ATTACK_ANIMATOR_ANIMATION_NAMES[0]);
    }

    public float GetAttackMiddleDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, m_ATTACK_ANIMATOR_ANIMATION_NAMES[1]);
    }

    public float GetAttackDownDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, m_ATTACK_ANIMATOR_ANIMATION_NAMES[2]);
    }

    // These methods are triggered by the attack animations
    // in order to mark the time window where the attack deals damage.
    public void OnStartAttacking()
    {
        m_weaponCollider.enabled = true;
    }

    public void OnStopAttacking()
    {
        m_weaponCollider.enabled = false;
        m_isAttackChained = false;
    }

    // This method is called at the end of the attack animation
    // and turns of the attack animation when no other attack is already registered.
    // This is part of the Race Condition solution.
    public void StopAttackingAnimation(int previousAttackingDirection)
    {
        if(!m_isAttackChained){
            // Stop the ended attack
            m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[previousAttackingDirection], false);
        }
        else if(previousAttackingDirection != m_nextAttackingDirection) 
        {
            // Stop the ended attack
            m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[previousAttackingDirection], false);
            // Start the new attack that has a different direction
            m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[m_nextAttackingDirection], true);
        }
        // Reset the Attribute
        m_isAttackChained = false;
    }
}
