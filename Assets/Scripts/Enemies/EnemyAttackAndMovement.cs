using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAndMovement : Entity, IEnemyAttackAndMovement
{
    [SerializeField]
    private float m_attackRange = 10;
    [SerializeField]
    protected bool m_isFriendlyFireActive = false;

    protected BoxCollider2D m_playerCollider;
    protected Rigidbody2D m_playerRigidbody2D;

    private bool m_isFollowingPlayer = false;
    private bool m_isAttackChained = false;
    private bool m_playerIsInRange = false;
    private string m_playerSwordName;
    private static string[] m_ATTACK_ANIMATOR_ANIMATION_NAMES = {"attack_up", "attack_mid", "attack_down"};

    protected override void Start()
    {
        base.Start();
        GameObject player = GameMediator.Instance.ActiveGame.GetActivePlayers()[0];
        m_playerRigidbody2D = player.GetComponent<Rigidbody2D>();
        m_playerSwordName = player.GetComponent<PlayerMovement>().PlayerSword.name;
    }

    void Update()
    {
        if (m_isFollowingPlayer && !IsInputLocked)
        {
            Vector2 playerPosition = m_playerRigidbody2D.position;
            float movementDirection = m_playerRigidbody2D.position.x - m_rigidbody2D.position.x;
            // Normalize the movemetDirection.
            movementDirection = movementDirection < 0 ? -1 : 1;
            m_animator.SetFloat("Speed", Mathf.Abs(movementDirection));

            // Flip Enemy Direction if player now walks in opposite direction.
            if (movementDirection < 0.0f && m_isFacingRight)
            {
                FlipEntity();
            }
            else if (movementDirection > 0.0f && !m_isFacingRight)
            {
                FlipEntity();
            }

            // Apply the movement to the physics.
            m_rigidbody2D.velocity = new Vector2(movementDirection * m_moveSpeed, m_rigidbody2D.velocity.y);
        }
        m_playerIsInRange = m_attackRange >= Vector2.Distance(m_rigidbody2D.position, m_playerRigidbody2D.position);
    }

    // This method initiates the entity dying animation.
    protected override void Die(){
        base.Die();
        m_animator.SetBool("IsDying", true);
    }

    // This method destroys the Game Object.
    // It is called at the end of the death animation.
    private void DestroyObject(){
        Destroy(gameObject);
    }

    // This method starts the new attack.
    // This rather inconvenient approach is needed in order to avoid a problem
    // that takes place when attacks are directly chained by the AttackPatternExecutor.
    private void startAttackIfPossible(int attackDirectionIndex)
    {
        if (!IsInputLocked)
        {
            m_currentAttackingDirection = attackDirectionIndex;
            if (!m_isAttackChained)
            {
                m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[m_currentAttackingDirection], true);
            }
            m_isAttackChained = true;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        // We abort if the collider is not from a player when friendly fire is off.
        if (!m_isFriendlyFireActive && collider.gameObject.name != m_playerSwordName)
        {
            return;
        }
        base.OnTriggerEnter2D(collider);
    }

    // These methods implements the IEnemyAttackAndMovement that is needed by the AttackPatternExecutor.
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

    public bool IsPlayerInRange(){
        return m_playerIsInRange;
    }

    public override void StopAttacking()
    {
        base.StopAttacking();
        m_isAttackChained = false;
    }

    // This method is called at the end of the attack animation
    // and turns the attack off animation when no other attack is already registered.
    // This is part of the attack chaning problem.
    public void StopAttackingAnimation(int previousAttackingDirection)
    {
        if (!m_isAttackChained)
        {
            // Stop the ended attack.
            m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[previousAttackingDirection], false);
        }
        else if (previousAttackingDirection != m_currentAttackingDirection) 
        {
            // Stop the ended attack.
            m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[previousAttackingDirection], false);
            // Start the new attack that has a different direction
            m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[m_currentAttackingDirection], true);
        }
        // Reset the attribute
        m_isAttackChained = false;
    }
}
