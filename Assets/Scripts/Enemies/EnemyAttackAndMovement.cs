using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAndMovement : MonoBehaviour, IEnemyAttackAndMovement
{
    [SerializeField]
    private float m_moveSpeed = 10f;
    [SerializeField]
    private bool m_facingRight;
    private Rigidbody2D m_rigidbody2D;
    private BoxCollider2D m_enemyCollider;
    private EntityHealth m_enemyHealth;
    private Animator m_animator;
    private Rigidbody2D m_playerRigidbody2D;
    private bool m_isFollowingPlayer = false;
    
    private static string[] m_ATTACK_ANIMATOR_PARAMETERS = {"AttackingUp", "Attacking", "AttackingDown"};

    public bool InputIsLocked { get; set; } = false;

    void Awake() {
        m_animator = gameObject.GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        m_enemyHealth = gameObject.GetComponent<EntityHealth>();
    }

    void Start()
    {
        m_playerRigidbody2D = GameMediator.Instance.ActiveGame.GetActivePlayers()[0].GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(m_isFollowingPlayer)
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

    private void startAttackIfPossible(int attackDirectionIndex)
    {
        if(!InputIsLocked)
        {
            m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[attackDirectionIndex], true);
        }
    }

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
        return Toolkit.GetAnimationLength(m_animator, m_ATTACK_ANIMATOR_PARAMETERS[0]);
    }
    public float GetAttackMiddleDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, m_ATTACK_ANIMATOR_PARAMETERS[1]);
    }
    public float GetAttackDownDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, m_ATTACK_ANIMATOR_PARAMETERS[2]);
    }
}
