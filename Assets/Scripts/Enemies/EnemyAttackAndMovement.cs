using System.Diagnostics;
using UnityEngine;

public class EnemyAttackAndMovement : Entity, IEnemyAttackAndMovement
{
    [SerializeField]
    private bool m_isFriendlyFireActive = false;
    [SerializeField]
    private float m_attackRange = 10.0f;
    [SerializeField]
    private float m_minPlayerDistance = 0.25f;
    [SerializeField]
    private float m_sightRange = 10.0f;

    private bool m_isAttackChained = false;
    private bool m_isAutoJumping = false;
    private bool m_isFollowingPlayer = false;
    private bool m_isPlayerInRange = false;
    private Stopwatch m_stopwatch = new Stopwatch(); 
    private string m_playerSwordName;
    private Vector2 m_lastPosition = new Vector2();

    // Time in milliseconds.
    private static readonly float INTERVAL_FOR_POSITION_CHECK = 200;
    private static readonly float AUTO_JUMPING_ACTIVATION_DISTANCE = 0.001f;
    private static readonly string[] ATTACK_ANIMATION_NAMES = { "attack_up", "attack_mid", "attack_down" };

    protected Rigidbody2D m_playerRigidbody2D;

    protected static readonly string DYING_ANIMATOR_PARAMETER_NAME = "IsDying";

    protected override void Awake()
    {
        base.Awake();
        Singleplayer.Instance.ActiveEnemies.Add(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        m_stopwatch.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Singleplayer.Instance.Player == null)
        {
            return; 
        } 
        else if (m_playerRigidbody2D == null || m_playerSwordName == null)
        {
            GameObject player = Singleplayer.Instance.Player;
            m_playerRigidbody2D = player.GetComponent<Rigidbody2D>();
            m_playerSwordName = player.GetComponent<PlayerMovement>().PlayerSword.name;
        }
        else
        {
            if (m_isFollowingPlayer && !IsInputLocked)
            {
                float movementDirection = m_playerRigidbody2D.position.x - m_rigidbody2D.position.x;
                // Only walk closer to the player if the player is not already too close.
                if (Mathf.Abs(movementDirection) > m_minPlayerDistance)
                {
                    // Normalize the movementDirection.
                    movementDirection = movementDirection < 0 ? -1 : 1;
                    m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, Mathf.Abs(movementDirection));
                }

                // Flip enemy direction if player now walks in opposite direction.
                if (movementDirection < 0.0f && m_isFacingRight || movementDirection > 0.0f && !m_isFacingRight)
                {
                    FlipEntity();
                }
                // Apply the movement to the physics.
                m_rigidbody2D.velocity = new Vector2(movementDirection * m_moveSpeed, m_rigidbody2D.velocity.y);

                if (m_isAutoJumping)
                {
                    // If jumping is turned on then jump if the current position is almost equal to the last position.
                    // It is only checked every INTERVALL_FOR_POSITION_CHECK.

                    if (m_stopwatch.ElapsedMilliseconds >= INTERVAL_FOR_POSITION_CHECK)
                    {
                        if (Vector2.Distance(m_lastPosition, gameObject.transform.position) < AUTO_JUMPING_ACTIVATION_DISTANCE)
                        {
                            OnJump(null);
                        }
                        m_lastPosition = gameObject.transform.position;
                        m_stopwatch.Restart();
                    }
                }
            }
            else
            {
                // Stop the walking animation if the enemy is too close to the player.
                m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, 0);
            }
        }
        m_isPlayerInRange = m_attackRange >= Vector2.Distance(m_rigidbody2D.position, m_playerRigidbody2D.position);
    }

    // This method initiates the entity dying animation and ensures that the enemy does nothing else.
    protected override void Die(){
        base.Die();
        m_animator.SetBool(DYING_ANIMATOR_PARAMETER_NAME, true);
        IsInputLocked = true;
    }

    // This method starts the new attack.
    // This rather inconvenient approach is needed in order to avoid a problem
    // that takes place when attacks are directly chained by the AttackPatternExecutor.
    private void StartAttackIfPossible(int attackDirectionIndex)
    {
        if (!IsInputLocked)
        {
            m_currentAttackingDirection = attackDirectionIndex;
            if (!m_isAttackChained)
            {
                m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[m_currentAttackingDirection], true);
            }
            m_isAttackChained = true;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        // We abort if the collider is not from a player when friendly fire is off.
        if (!m_isFriendlyFireActive && collider.gameObject.name != m_playerSwordName && collider.gameObject.name != DEATH_ZONES_NAME)
        {
            return;
        }
        base.OnTriggerEnter2D(collider);
    }

    // These methods implement the IEnemyAttackAndMovement that is needed by the AttackPatternExecutor.
    public void AttackUp()
    {
        StartAttackIfPossible(0);
    }

    public void AttackMiddle()
    {
        StartAttackIfPossible(1);
    }

    public void AttackDown()
    {
        StartAttackIfPossible(2);
    }

    public void StartFollowPlayer()
    {
        m_isFollowingPlayer = true;
    }

    public void StopFollowPlayer()
    {
        m_isFollowingPlayer = false;
        m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, 0);
    }

    public void StartAutoJumping(){
        m_isAutoJumping = true;
    }

    public void StopAutoJumping(){
        m_isAutoJumping = false;
    }

    public float GetAttackUpDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, ATTACK_ANIMATION_NAMES[0]);
    }

    public float GetAttackMiddleDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, ATTACK_ANIMATION_NAMES[1]);
    }

    public float GetAttackDownDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, ATTACK_ANIMATION_NAMES[2]);
    }

    public bool IsPlayerInRange(){
        return m_isPlayerInRange;
    }

    public bool IsPlayerInAttackRange()
    {
        if (m_playerRigidbody2D == null)
        {
            return false;
        }
        return m_attackRange >= Vector2.Distance(m_playerRigidbody2D.transform.position, gameObject.transform.position);
    }

    public bool IsPlayerInSightRange()
    {
        if (m_playerRigidbody2D == null)
        {
            return false;
        }
        return m_sightRange >= Vector2.Distance(m_playerRigidbody2D.transform.position, gameObject.transform.position);
    }

    public override void StopAttacking()
    {
        base.StopAttacking();
        m_isAttackChained = false;
    }

    // This method is called at the end of the attack animation
    // and turns the attack off animation when no other attack is already registered.
    // This is part of the attack chaining problem.
    public void StopAttackingAnimation(int previousAttackingDirection)
    {
        if (!m_isAttackChained)
        {
            // Stop the ended attack.
            m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[previousAttackingDirection], false);
        }
        else if (previousAttackingDirection != m_currentAttackingDirection) 
        {
            // Stop the ended attack.
            m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[previousAttackingDirection], false);
            // Start the new attack that has a different direction
            m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[m_currentAttackingDirection], true);
        }
        // Reset the attribute
        m_isAttackChained = false;
    }

    // This method destroys the gameObject.
    // It is called by the death animation.
    void DestroySelf()
    {
        Destroy(gameObject);
    }


    // It is called at the end of the death animation.
    // This method is automatically called when the gameObject is destroyed.
    void OnDestroy()
    {
        Singleplayer.Instance.ActiveEnemies.Remove(gameObject);
    }
}
