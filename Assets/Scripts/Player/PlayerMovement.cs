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
    private bool m_facingRight = true;
    [SerializeField]
    private float m_groundCheckY = 0.1f;
    [SerializeField]
    private LayerMask m_whatIsGround;
    [SerializeField]
    private Recorder m_recorder;

    private bool m_isGrounded = true;
    private Rigidbody2D m_rigidbody2D;
    private Collider2D m_playerCollider;
    private EntityHealth m_playerHealth;
    private EntityAttack m_playerAttack;

    public bool InputIsLocked { get; set; } = false;
    public Animator Animator { get; private set; }

    private void Awake()
    {
        Animator = gameObject.GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_playerCollider = gameObject.GetComponent<BoxCollider2D>();
        m_playerHealth = gameObject.GetComponent<EntityHealth>();
        m_playerAttack = gameObject.GetComponent<EntityAttack>();
    }

    void FixedUpdate()
    {
        m_isGrounded = Physics2D.OverlapArea(m_playerCollider.bounds.min,
                        (Vector2)m_playerCollider.bounds.min + new Vector2(m_playerCollider.bounds.size.x, m_groundCheckY), m_whatIsGround);
        Animator.SetBool("IsJumping", !m_isGrounded);
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
        m_playerAttack.ResetAttackAnimation();
    }

    public void ResetMovement()
    {
        m_rigidbody2D.velocity = new Vector2(0, m_rigidbody2D.velocity.y);
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
            if (collider.gameObject.name == m_playerAttack.playerSword.name && collider.gameObject != m_playerAttack.playerSword)
            {
                // Test if the attacks are canceling each other.
                EntityAttack attackerEntityAttack = collider.gameObject.GetComponentInParent<EntityAttack>();
                // If the attacker has an EntityAttack the attack might be cancelled.
                if (attackerEntityAttack == null || !m_playerAttack.attackIsCancelling(attackerEntityAttack))
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
        if (!InputIsLocked)
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
        if (!InputIsLocked)
        {
            // Controls.
            var moveX = value.Get<float>();
            // Animation.
            Animator.SetFloat("Speed", Mathf.Abs(moveX));

            // Player Direction.
            if (moveX < 0.0f && m_facingRight)
            {
                FlipPlayer();
            }
            else if (moveX > 0.0f && !m_facingRight)
            {
                FlipPlayer();
            }

            // Physics.
            m_rigidbody2D.velocity = new Vector2(moveX * m_moveSpeed, m_rigidbody2D.velocity.y);
        }
    }

    private void FlipPlayer()
    {
        m_facingRight = !m_facingRight;
        gameObject.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
        // Flip the layer of the sword.
        m_playerAttack.ChangeOrderInLayer();
    }

    void OnRecord(InputValue value)
    {
        m_recorder.Record();
    }

    public void Die()
    {
        int maxHealth = m_playerHealth.maxHealth;
        m_playerHealth.TakeDamage(maxHealth);
        GameMediator.Instance.HandleDeath(gameObject);
    }

    public void SetPosition(Vector2 position)
    {
        gameObject.transform.position = new Vector3(position.x, position.y,
            gameObject.transform.position.z);
    }
}
