using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class PlayerMovement : MediatableMonoBehavior
{
    [HideInInspector]
    public bool m_inputIsLocked = false;

    [SerializeField]
    public Animator m_animator;

    [SerializeField]
    private Rigidbody2D m_myRigidbody2D;
    [SerializeField]
    private Collider2D m_playerCollider;
    [SerializeField]
    private EntityHealth m_playerHealth;
    [SerializeField]
    private EntityAttack m_playerAttack;
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

    private bool m_isGrounded = true;

    void FixedUpdate()
    {
        m_isGrounded = Physics2D.OverlapArea(m_playerCollider.bounds.min,
                        (Vector2)m_playerCollider.bounds.min + new Vector2(m_playerCollider.bounds.size.x, m_groundCheckY), m_whatIsGround);
        m_animator.SetBool("IsJumping", !m_isGrounded);
    }

    public void ResetPlayerActions()
    {
        m_playerHealth.Revive();
        ResetPlayerAnimations();
        ResetMovement();
    }

    public void ResetPlayerAnimations()
    {
        m_animator.SetBool("IsJumping", false);
        m_animator.SetFloat("Speed", 0);
        m_playerAttack.ResetAttackAnimation();
    }

    public void ResetMovement()
    {
        m_myRigidbody2D.velocity = new Vector2(0, m_myRigidbody2D.velocity.y);
    }

    // This methods checks if the collider kills the player.
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!m_inputIsLocked)
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
                    m_playerHealth.takeDamage(1);
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
        if (!m_inputIsLocked)
        {
            if (m_isGrounded)
            {
                m_animator.SetBool("IsJumping", true);
                m_myRigidbody2D.velocity = new Vector2(m_myRigidbody2D.velocity.x, m_jumpForce);
            }
        }
    }

    void OnMovement(InputValue value)
    {
        if (!m_inputIsLocked)
        {
            // Controls.
            var moveX = value.Get<float>();
            // Animation.
            m_animator.SetFloat("Speed", Mathf.Abs(moveX));

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
            m_myRigidbody2D.velocity = new Vector2(moveX * m_moveSpeed, m_myRigidbody2D.velocity.y);
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
        // Ignore locked input.
        gameMediator.Record();
    }

    public void Die()
    {
        gameMediator.HandleDeath(gameObject);
    }

    public void SetPosition(Vector2 position)
    {
        gameObject.transform.position = new Vector3(position.x, position.y,
            gameObject.transform.position.z);
    }
}
