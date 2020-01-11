using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class EntityAttack : MonoBehaviour
{
    public GameObject playerSword;
    private Animator animator;
    private float attackDirection;
    private double attackDuration;
    private int currentAttackAnimationParameter = 0; 
    private double lastTimeAttacked = -10000;
    private bool attacking;
    private static float ATTACK_DIRECTION_DEADZONE = 0.1f;
    private static string[] ATTACK_ANIMATOR_PARAMETERS = {"AttackingUp", "Attacking", "AttackingDown"};
    private SpriteRenderer swordRenderer;
    private Collider2D blackSwordCollider;
    private PlayerMovement playerMovement;

    // Components should be gathered on Awake for safety reasons.
    private void Awake()
    {
        swordRenderer = playerSword.GetComponent<SpriteRenderer>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        blackSwordCollider = playerSword.GetComponent<Collider2D>();
        animator = playerMovement.Animator;
    }

    // Start is called before the first frame update
    void Start()
    {
        blackSwordCollider.enabled = false;
        attackDuration = getAnimationLength("Player_1_attack");
        attacking = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(attacking){
            lastTimeAttacked -= Time.deltaTime;
            if(lastTimeAttacked < 0){
                attacking = false;
                animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[currentAttackAnimationParameter], attacking);
            }
        }
    }

    float getAnimationLength(string name)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for(int i = 0; i < ac.animationClips.Length; ++i){
            if(ac.animationClips[i].name == name) {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }

    public void ResetAttackAnimation(){
        attacking = false;
        animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[0], false);
        animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[1], false);
        animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[2], false);
    }
    
    void OnAttack(InputValue value){
        if(!playerMovement.InputIsLocked){
            if(lastTimeAttacked < 0){
                attacking = true;
                determineAttackingParameter(attackDirection);
                animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[currentAttackAnimationParameter], attacking);
                lastTimeAttacked = attackDuration;
            }
        }
    }

    private void determineAttackingParameter(float attackDirectionAxisValue){
        if(attackDirectionAxisValue > ATTACK_DIRECTION_DEADZONE){
            currentAttackAnimationParameter = 0;
        } else if(attackDirectionAxisValue > -ATTACK_DIRECTION_DEADZONE){
            currentAttackAnimationParameter = 1;
        } else {
            currentAttackAnimationParameter = 2;
        }
    }

    public bool attackIsCancelling(EntityAttack attacker){
        return attacker != null && attacker.attacking && this.attacking && 
            attacker.currentAttackAnimationParameter == this.currentAttackAnimationParameter;
    }

    void OnAttackDirection(InputValue value){
        if(!playerMovement.InputIsLocked){
            attackDirection = value.Get<float>();
        }
    }

    public void startAttacking()
    {
        blackSwordCollider.enabled = true;
    }

    public void stopAttacking()
    {
        blackSwordCollider.enabled = false;
    }

    public void ChangeOrderInLayer(){
        swordRenderer.sortingOrder = swordRenderer.sortingOrder * -1;
    }
}
