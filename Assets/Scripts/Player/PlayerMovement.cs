using Assets.Scripts.Recording;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class PlayerMovement : Entity
{
    [SerializeField]
    private GameObject m_playerSword;
    [SerializeField]
    private int m_playerIndex = 1;
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_playerPositionsTransform;

    private bool m_hasStablePosition = false; 
    private double m_attackDuration; 
    private double m_lastTimeAttacked = -1.0d;
    private float m_attackDirection;
    private float m_rollingMovementX;
    private IGame m_activeGame;
    private SpriteRenderer m_swordRenderer;
    private Stopwatch m_stopwatch = new Stopwatch();
    private Vector2 m_nonRollingColliderSize;
    private Vector2 m_rollingColliderSize;
    private Vector2 m_lastCheckedPosition;

    private static readonly float CONTROLLER_DEADZONE = 0.3f;
    // Time in milliseconds.
    private static readonly float INTERVAL_FOR_POSITION_CHECK = 400;
    protected static readonly string PLAYER_ATTACK_ANIMATION_NAME = "Player_1_attack";
    protected static readonly string ROLLING_ANIMATION_NAME = "Rolling";

    public GameObject PlayerSword { get { return m_playerSword; } }
    // Positions from outer left to outer right stage as they are in the scene.
    public IList<Vector2> Positions { get; set; }
    public Vector2 RevivePosition { get; private set; }
    

    public int Index
    {
        get
        {
            return m_playerIndex;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_nonRollingColliderSize = m_collider.size;
        m_rollingColliderSize = m_nonRollingColliderSize / 2;

        Positions = new List<Vector2>();
        if (m_playerPositionsTransform != null)
        {
            foreach (Transform positionsTransform in m_playerPositionsTransform)
            {
                Positions.Add(positionsTransform.position);
            }
        }
        m_swordRenderer = PlayerSword.GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        RevivePosition = gameObject.transform.position;
        // Registration of player on start for safety reasons.
        m_activeGame = Game.Current;
        m_activeGame.RegisterPlayer(gameObject);
        m_attackDuration = Toolkit.GetAnimationLength(m_animator, PLAYER_ATTACK_ANIMATION_NAME);
        m_stopwatch.Start();
    }

    protected override void Update()
    {
        base.Update();
        UpdateRevivePosition();

        // Since to the ground is not slippery, we need to reapply the velocity.
        if (IsRolling)
        {
            Vector2 manipulatedVelocity = m_rigidbody2D.velocity;
            manipulatedVelocity.x = m_rollingMovementX;
            m_rigidbody2D.velocity = manipulatedVelocity;
        }

        // Set the player as not attacking when the time that the attack animation needs is over.
        // Set the Animator variable as well.
        if (Attacking)
        {
            m_lastTimeAttacked -= Time.deltaTime;
            if (m_lastTimeAttacked < 0)
            {
                Attacking = false;
                m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[m_currentAttackingDirection], Attacking);
            }
        }
    }

    protected override void FlipEntity()
    {
        base.FlipEntity();
        // Flip the layer of the sword.
        ChangeOrderInLayer();
    }

    protected override void Die()
    {
        base.Die();
        m_entityHealth.Die();
        m_activeGame.HandleDeath(gameObject);
    }

    public override void ResetEntityAnimations()
    {
        base.ResetEntityAnimations();
        m_animator.SetBool(ROLLING_ANIMATION_NAME, false);
        StopRollingInvincibility();
        IsRolling = false;
        m_lastTimeAttacked = 0.0d;
    }

    public override void ResetMovement()
    {
        base.ResetMovement();
        IsRolling = false;
    }

    public void SetPosition(int index)
    {
        Vector2 position = Positions[index];
        gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
    }

    public void SetPositionForRevive(Vector2 revivePosition)
    {
        RevivePosition = revivePosition;
        gameObject.transform.position = new Vector3(revivePosition.x, revivePosition.y, gameObject.transform.position.z);
    }

    // This method starts the player roll if he is not already rolling, is on the ground,
    // the input is not locked and the player is not attacking.
    public void OnRoll(InputValue value)
    {
        if (!IsInputLocked && !Attacking && !IsRolling && m_isGrounded)
        {
            m_animator.SetBool(ROLLING_ANIMATION_NAME, true);
            m_rollingMovementX = m_rigidbody2D.velocity.x;
            IsRolling = true;
        }
    }

    public void StopRolling()
    {
        m_animator.SetBool(ROLLING_ANIMATION_NAME, false);
        IsRolling = false;
    }

    public void StartRollingInvincibility()
    {
        m_entityHealth.Invincible = true;
        m_collider.size = m_rollingColliderSize;
        m_activeGame.DisableEntityCollision(gameObject);
    }

    public void StopRollingInvincibility()
    {
        m_entityHealth.Invincible = false;
        m_collider.size = m_nonRollingColliderSize;
        m_activeGame.EnableEntityCollision(gameObject);
    }

    public void OnPauseGame()
    {
        if (!IsInputLocked)
        {
            Game.Pause();
        }
    }

    // This method changes the weapon of the entity to alternate between these two states:
    // Weapon rendered before player, Weapon rendered behind player.
    public void ChangeOrderInLayer()
    {
        m_swordRenderer.sortingOrder *= -1;
    }
    
    // This method is triggered when the player presses the attack button.
    // According to the current attack direction based on the player input the attack is executed
    // unless the input is locked or the entity is already attacking.
    void OnAttack(InputValue value)
    {
        if (!IsInputLocked && !IsRolling)
        {
            if (m_lastTimeAttacked <= 0)
            {
                Attacking = true;
                DetermineAttackingParameter(m_attackDirection);
                m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[m_currentAttackingDirection], Attacking);
                m_lastTimeAttacked = m_attackDuration;
            }
        }
    }

    // This method is invoked when the entity changes the attack direction, 
    // e.g. PlayerInput and sets it to the current m_attackDirection.
    void OnAttackDirection(InputValue value)
    {
        if (!IsInputLocked)
        {
            m_attackDirection = value.Get<float>();
        }
    }

    void OnMovement(InputValue value)
    {
        if (!IsInputLocked && !IsRolling)
        {
            // Controls.
            float moveX = value.Get<float>();

            if (Math.Abs(moveX) < CONTROLLER_DEADZONE)
            {
                moveX = 0.0f;
            }

            // Animation.
            m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, Mathf.Abs(moveX));

            // Player direction.
            if (moveX < 0.0f && m_isFacingRight)
            {
                FlipEntity();
            }
            else if (moveX > 0.0f && !m_isFacingRight)
            {
                FlipEntity();
            }

            // Physics.
            m_rigidbody2D.velocity = new Vector2((float)Math.Round(moveX) * m_moveSpeed, m_rigidbody2D.velocity.y);
        }
    }

    void OnRecord(InputValue value)
    {
        Recorder.Instance.Record();
    }

    // This method determines the attack direction.
    private void DetermineAttackingParameter(float attackDirectionAxisValue)
    {
        if (attackDirectionAxisValue > s_ATTACK_DIRECTION_DEADZONE)
        {
            m_currentAttackingDirection = 0;
        }
        else if (attackDirectionAxisValue > -s_ATTACK_DIRECTION_DEADZONE)
        {
            m_currentAttackingDirection = 1;
        }
        else 
        {
            m_currentAttackingDirection = 2;
        }
    }

    // This method updates the revive position of the player. The revive position has to be stable. 
    // For it to be stable the player has to continiously be grounded for INTERVAL_FOR_POSITION_CHECK
    // milliseconds.
    private void UpdateRevivePosition()
    {
        if (!m_isGrounded)
        {
            m_hasStablePosition = false;
        }
        else if (!m_hasStablePosition || m_stopwatch.ElapsedMilliseconds >= INTERVAL_FOR_POSITION_CHECK)
        {
            // As soon as the player hits ground again or the time between checks is up, this part is executed.

            if (m_hasStablePosition)
            {
                // Since the player was continiously grounded the last position is stable.
                RevivePosition = m_lastCheckedPosition;
            }
            m_lastCheckedPosition = gameObject.transform.position;
            m_stopwatch.Restart();
        }
    }

    private void OnDestroy()
    {
        m_activeGame.UnregisterPlayer(gameObject);
    }
}
