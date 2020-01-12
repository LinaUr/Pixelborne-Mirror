using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

// This class manages the basic attack functionality of an entity.
public class EntityAttack : MonoBehaviour
{
    [SerializeField]
    public GameObject PlayerSword;
    private Animator m_animator;
    private float m_attackDirection;
    private double m_attackDuration;
    private int m_currentAttackAnimationParameter = 0; 
    private double m_lastTimeAttacked = -10000;
    public bool Attacking { get; private set; }
    private static float m_ATTACK_DIRECTION_DEADZONE = 0.1f;
    private static string[] m_ATTACK_ANIMATOR_PARAMETERS = {"AttackingUp", "Attacking", "AttackingDown"};
    private SpriteRenderer m_swordRenderer;
    private Collider2D m_blackSwordCollider;
    private PlayerMovement m_playerMovement;

    // Components should be gathered on Awake for safety reasons.
    private void Awake()
    {
        m_swordRenderer = PlayerSword.GetComponent<SpriteRenderer>();
        m_playerMovement = gameObject.GetComponent<PlayerMovement>();
        m_blackSwordCollider = PlayerSword.GetComponent<Collider2D>();
        m_animator = m_playerMovement.Animator;
    }

    void Start()
    {
        m_blackSwordCollider.enabled = false;
        m_attackDuration = Utils.GetAnimationLength(m_animator, "Player_1_attack");
        Attacking = false;
    }

    void Update()
    {
        // Set the player as not attacking when the time that the attack animation needs is over.
        // Set the Animator variable as well.
        if(Attacking)
        {
            m_lastTimeAttacked -= Time.deltaTime;
            if(m_lastTimeAttacked < 0)
            {
                Attacking = false;
                m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[m_currentAttackAnimationParameter], Attacking);
            }
        }
    }

    // This method resets the attack including the animator.
    public void ResetAttackAnimation(){
        Attacking = false;
        m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[0], false);
        m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[1], false);
        m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[2], false);
    }
    
    // This method is triggered when the player presses the attack button.
    // According to the current attack direction based on the player input the attack is executed
    // unless the input is locked or the entity is already attacking.
    void OnAttack(InputValue value){
        if(!m_playerMovement.InputIsLocked && !m_playerMovement.IsRolling)
        {
            if(m_lastTimeAttacked < 0)
            {
                Attacking = true;
                determineAttackingParameter(m_attackDirection);
                m_animator.SetBool(m_ATTACK_ANIMATOR_PARAMETERS[m_currentAttackAnimationParameter], Attacking);
                m_lastTimeAttacked = m_attackDuration;
            }
        }
    }

    // This method determines the attack direction
    private void determineAttackingParameter(float attackDirectionAxisValue){
        if(attackDirectionAxisValue > m_ATTACK_DIRECTION_DEADZONE)
        {
            m_currentAttackAnimationParameter = 0;
        } else if(attackDirectionAxisValue > -m_ATTACK_DIRECTION_DEADZONE)
        {
            m_currentAttackAnimationParameter = 1;
        } else 
        {
            m_currentAttackAnimationParameter = 2;
        }
    }

    // This method tests if both entities are executing the same attack. This would cancel both attacks.
    public bool attackIsCancelling(EntityAttack attacker)
    {
        return attacker != null && attacker.Attacking && this.Attacking && 
            attacker.m_currentAttackAnimationParameter == this.m_currentAttackAnimationParameter;
    }

    // This method is invoked when the entity changes the attack direction e.g. PlayerInput and sets it to the current m_attackDirection.
    void OnAttackDirection(InputValue value)
    {
        if(!m_playerMovement.InputIsLocked)
        {
            m_attackDirection = value.Get<float>();
        }
    }

    // This method is executed by an Animation event and marks the beginning 
    // of the time where the sword actually can deal damage in the attack animation.
    public void startAttacking()
    {
        m_blackSwordCollider.enabled = true;
    }

    // This method is executed by an Animation event and marks the end
    // of the time where the sword actually can deal damage in the attack animation.
    public void stopAttacking()
    {
        m_blackSwordCollider.enabled = false;
    }

    // This method changes the weapon of the entity to alternate between these two states:
    // Weapon rendered before player, Weapon rendered behind player.
    public void ChangeOrderInLayer()
    {
        m_swordRenderer.sortingOrder = m_swordRenderer.sortingOrder * -1;
    }
}
