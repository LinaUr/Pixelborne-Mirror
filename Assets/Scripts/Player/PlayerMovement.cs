using Assets.Scripts.Recording;
using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 10f;
    [SerializeField]
    private float m_jumpForce = 20f;
    [SerializeField]
    private bool m_isFacingRight = true;
    [SerializeField]
    private float m_groundCheckY = 0.1f;
    [SerializeField]
    private LayerMask m_whatIsGround;
    [SerializeField]
    private Recorder m_recorder;

    private bool m_isGrounded = true;
    private Rigidbody2D m_rigidbody2D;
    private BoxCollider2D m_playerCollider;
    private EntityHealth m_playerHealth;
    private EntityAttack m_playerAttack;

    private Vector2 m_NON_ROLLING_COLLIDER_SIZE;
    private Vector2 m_ROLLING_COLLIDER_SIZE = new Vector2(0.1919138f, 0.1936331f);
    private float m_rollingMovementX;

    public bool IsRolling {get; private set;}
    public bool InputIsLocked { get; set; } = false;
    public Animator Animator { get; private set; }

    private void Awake()
    {
        Animator = gameObject.GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_playerCollider = gameObject.GetComponent<BoxCollider2D>();
        m_playerHealth = gameObject.GetComponent<EntityHealth>();
        m_playerAttack = gameObject.GetComponent<EntityAttack>();
        m_NON_ROLLING_COLLIDER_SIZE = m_playerCollider.size;
        m_ROLLING_COLLIDER_SIZE = (m_NON_ROLLING_COLLIDER_SIZE / 2);
    }

    private void Start()
    {
       if (!m_isFacingRight)
        {
            FlipPlayer();
        }
    }

    void Update()
    {
        m_isGrounded = Physics2D.OverlapArea(m_playerCollider.bounds.min,
                        (Vector2)m_playerCollider.bounds.min + new Vector2(m_playerCollider.bounds.size.x, m_groundCheckY), m_whatIsGround);
        Animator.SetBool("IsJumping", !m_isGrounded);
        // Since to ground is not slippery, we need to reapply the velocity
        if(IsRolling) {
            Vector2 manipulatedVelocity = m_rigidbody2D.velocity;
            manipulatedVelocity.x = m_rollingMovementX;
            m_rigidbody2D.velocity = manipulatedVelocity;
        }
    }

    public void ResetPlayerActions()
    {
        m_playerHealth.Revive();
        ResetPlayerAnimations();
        ResetMovement();
    }

    public void ResetPlayerAnimations()
    {
        Animator.SetBool("IsJumping", false);
        Animator.SetFloat("Speed", 0);
        Animator.SetBool("Rolling", false);
        m_playerAttack.ResetAttackAnimation();
    }

    public void ResetMovement()
    {
        m_rigidbody2D.velocity = new Vector2(0, m_rigidbody2D.velocity.y);
        IsRolling = false;
    }

    // This methods checks if the collider kills the player.
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!InputIsLocked)
        {
            if (collider.gameObject.name == "DeathZones")
            {
                Die();
            }
            // We have to explicitely look for a sword as a collider because else
            // the winning player could also be colliding with this player and would die instead.
            if (collider.gameObject.name == m_playerAttack.PlayerSword.name && collider.gameObject != m_playerAttack.PlayerSword)
            {
                // Test if the attacks are canceling each other.
                EntityAttack attackerEntityAttack = collider.gameObject.GetComponentInParent<EntityAttack>();
                // If the attacker has an EntityAttack the attack might be cancelled.
                if (attackerEntityAttack == null || !m_playerAttack.AttackIsCancelling(attackerEntityAttack))
                {
                    m_playerHealth.TakeDamage(1);
                    if (m_playerHealth.isDead)
                    {
                        Die();
                    }
                }
            }
        }
    }

    void OnJump(InputValue value)
    {
        if (!InputIsLocked && !IsRolling)
        {
            if (m_isGrounded)
            {
                Animator.SetBool("IsJumping", true);
                m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, m_jumpForce);
            }
        }
    }

    void OnMovement(InputValue value)
    {
        if (!InputIsLocked && !IsRolling)
        {
            // Controls.
            var moveX = value.Get<float>();
            // Animation.
            Animator.SetFloat("Speed", Mathf.Abs(moveX));

            // Player Direction.
            if (moveX < 0.0f && m_isFacingRight)
            {
                m_isFacingRight = !m_isFacingRight;
                FlipPlayer();
            }
            else if (moveX > 0.0f && !m_isFacingRight)
            {
                m_isFacingRight = !m_isFacingRight;
                FlipPlayer();
            }

            // Physics.
            m_rigidbody2D.velocity = new Vector2(moveX * m_moveSpeed, m_rigidbody2D.velocity.y);
        }
    }

    private void FlipPlayer()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        // Flip the layer of the sword.
        m_playerAttack.ChangeOrderInLayer();
    }

    void OnRecord(InputValue value)
    {
        m_recorder.Record();
    }

    public void Die()
    {
        int maxHealth = m_playerHealth.MaxHealth;
        m_playerHealth.TakeDamage(maxHealth);
        GameMediator.Instance.HandleDeath(gameObject);
    }

    public void SetPosition(Vector2 position)
    {
        gameObject.transform.position = new Vector3(position.x, position.y,
            gameObject.transform.position.z);
    }

    // This method starts the player roll if he is not already rolling, is on the ground,
    // the input is not locked and the player is not attacking.
    public void OnRoll(InputValue value)
    {
        if(!InputIsLocked && !m_playerAttack.Attacking && !IsRolling && m_isGrounded)
        {
            Animator.SetBool("Rolling", true);
            m_rollingMovementX = m_rigidbody2D.velocity.x;
            IsRolling = true;
        }
    }

    public void StopRolling()
    {
        Animator.SetBool("Rolling", false);
        IsRolling = false;
    }

    public void StartRollingInvincibility()
    {
        m_playerHealth.Invincible = true;
        m_playerCollider.size = m_ROLLING_COLLIDER_SIZE;
        GameMediator.Instance.EnableEntityCollision(gameObject);
    }

    public void StopRollingInvincibility()
    {
        m_playerHealth.Invincible = false;
        m_playerCollider.size = m_NON_ROLLING_COLLIDER_SIZE;
        GameMediator.Instance.DisableEntityCollision(gameObject);
    }
}
