using Assets.Scripts.Recording;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class PlayerMovement : Entity
{
    [SerializeField]
    private int m_playerIndex;
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_playerPositionsTransform;
    [SerializeField]
    private Recorder m_recorder;
    [SerializeField]
    private GameObject m_playerSword;

    private float m_rollingMovementX;
    private float m_attackDirection;
    private double m_attackDuration; 
    private double m_lastTimeAttacked = -1;
    private Vector2 m_nonRollingColliderSize;
    private SpriteRenderer m_swordRenderer;
    private Vector2 m_rollingColliderSize;
    private float m_timeToNextSetRevivePosition = 0;
    private Vector2 m_nextPotentialRevivePosition;
    private IGame m_activeGame;
    private readonly float m_CONTROLLER_DEADZONE = 0.30f;
    private static readonly float TIME_BETWEEN_REVIVE_POSITION_SETTING = 0.4f;

    public Vector2 RevivePosition {get; private set; } = INVALID_POSITION;
    // Positions from outer left to outer right stage as they are in the scene.
    public IList<Vector2> Positions { get; set; }
    public GameObject PlayerSword { get { return m_playerSword; } }

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
        m_rollingColliderSize = (m_nonRollingColliderSize / 2);

        Positions = new List<Vector2>();
        foreach (Transform positionsTransform in m_playerPositionsTransform)
        {
            Positions.Add(positionsTransform.position);
        }

        m_swordRenderer = PlayerSword.GetComponent<SpriteRenderer>();
        m_activeGame = Game.Current;
        m_activeGame.RegisterPlayer(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        // Put registration in Start for safety reasons.
       
        m_attackDuration = Toolkit.GetAnimationLength(m_animator, "Player_1_attack");
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

    private void UpdateRevivePosition()
    {
        m_timeToNextSetRevivePosition -= Time.deltaTime;
        // Reset m_nextPotentialRevivePosition when the player is not on the ground.
        if (!m_isGrounded)
        {
            m_timeToNextSetRevivePosition = 0;
            m_nextPotentialRevivePosition = INVALID_POSITION;
        }
        else if (m_timeToNextSetRevivePosition <= 0)
        {
            if (m_nextPotentialRevivePosition != INVALID_POSITION)
            {
                RevivePosition = m_nextPotentialRevivePosition;
            }
            m_nextPotentialRevivePosition = gameObject.transform.position; 
            m_timeToNextSetRevivePosition = TIME_BETWEEN_REVIVE_POSITION_SETTING;
        }
    }

    public override void ResetEntityAnimations()
    {
        base.ResetEntityAnimations();
        m_animator.SetBool("Rolling", false);
        StopRollingInvincibility();
        IsRolling = false;
        m_lastTimeAttacked = 0;
    }
    public override void ResetMovement()
    {
        base.ResetMovement();
        IsRolling = false;
    }

    void OnMovement(InputValue value)
    {
        if (!IsInputLocked && !IsRolling)
        {
            // Controls.
            float moveX = value.Get<float>();

            if (Math.Abs(moveX) < m_CONTROLLER_DEADZONE)
            {
                moveX = 0.0f;
            }

            // Animation.
            m_animator.SetFloat("Speed", Mathf.Abs(moveX));

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

    protected override void FlipEntity()
    {
        base.FlipEntity();
        // Flip the layer of the sword.
        ChangeOrderInLayer();
    }

    void OnRecord1(InputValue value)
    {
        m_recorder.Record();
    }

    void OnRecord2(InputValue value)
    {
        m_recorder.Record();
    }

    protected override void Die()
    {
        base.Die();
        m_entityHealth.Die();
        m_activeGame.HandleDeath(gameObject);
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
            m_animator.SetBool("Rolling", true);
            m_rollingMovementX = m_rigidbody2D.velocity.x;
            IsRolling = true;
        }
    }

    public void StopRolling()
    {
        m_animator.SetBool("Rolling", false);
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

    private void OnDestroy()
    {
        m_activeGame.UnregisterPlayer(gameObject);
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

    // This method determines the attack direction.
    private void DetermineAttackingParameter(float attackDirectionAxisValue)
    {
        if (attackDirectionAxisValue > m_ATTACK_DIRECTION_DEADZONE)
        {
            m_currentAttackingDirection = 0;
        }
        else if (attackDirectionAxisValue > -m_ATTACK_DIRECTION_DEADZONE)
        {
            m_currentAttackingDirection = 1;
        }
        else 
        {
            m_currentAttackingDirection = 2;
        }
    }

    // This method is invoked when the entity changes the attack direction e.g. PlayerInput and sets it to the current m_attackDirection.
    void OnAttackDirection(InputValue value)
    {
        if (!IsInputLocked)
        {
            m_attackDirection = value.Get<float>();
        }
    }

    // This method changes the weapon of the entity to alternate between these two states:
    // Weapon rendered before player, Weapon rendered behind player.
    public void ChangeOrderInLayer()
    {
        m_swordRenderer.sortingOrder *= -1;
    }
}
