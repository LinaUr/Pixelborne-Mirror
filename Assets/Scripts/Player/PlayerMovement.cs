using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class PlayerMovement : MediatableMonoBehavior
{
    public float moveSpeed = 10f;
    public float jumpForce = 20f;
    public bool facingRight = true;

    public float groundCheckY = 0.1f;
    public LayerMask whatIsGround;
    public bool isGrounded = true;
    public Animator animator;
    public Rigidbody2D myRigidbody2D;
    public Collider2D playerCollider;
    public float attackDirection;
    public GameObject playerSword;

    private double lastTimeAttacked = -10000;
    private double attackDuration;
    private bool attacking = false;
    private Collider2D blackSwordCollider;
    private int currentAttackAnimationParameter = 0;
    public bool inputIsLocked = false;

    
    public static float ATTACK_DIRECTION_DEADZONE = 0.1f;
    private static string[] ATTACK_ANIMATOR_PARAMETERS = {"AttackingUp", "Attacking", "AttackingDown"};
    

    public void resetPlayerAnimations(){
        animator.SetBool("IsJumping", false);
        animator.SetFloat("Speed", 0);
        animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[0], attacking);
        animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[1], attacking);
        animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[2], attacking);
    }

    public void resetMovement(){
        myRigidbody2D.velocity = new Vector2(0, myRigidbody2D.velocity.y);
    }

    void Start ()
    {
        myRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        playerCollider = gameObject.GetComponent<Collider2D>();
        attackDuration = getAnimationLength("Player_1_attack");
        blackSwordCollider = playerSword.GetComponent<Collider2D>();
        blackSwordCollider.enabled = false;
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

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapArea((Vector2) playerCollider.bounds.min, 
                        (Vector2) playerCollider.bounds.min + new Vector2(playerCollider.bounds.size.x, groundCheckY), whatIsGround);
        animator.SetBool("IsJumping", !isGrounded);
        if(attacking){
            lastTimeAttacked -= Time.deltaTime;
            if(lastTimeAttacked < 0){
                attacking = false;
                animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[currentAttackAnimationParameter], attacking);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(!inputIsLocked){
            if(collider.gameObject != gameObject){
                Die();
            }
        }
    }

    void OnAttack(InputValue value){
        if(!inputIsLocked){
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

    void OnJump(InputValue value)
    {
        if(!inputIsLocked){
        if (isGrounded)
            {
                animator.SetBool("IsJumping", true);
                myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, jumpForce);
            }
        }
    }

    void OnAttackDirection(InputValue value){
        if(!inputIsLocked){
            attackDirection = value.Get<float>();
        }
    }

    void OnMovement(InputValue value)
    {
        if(!inputIsLocked){

            // CONTROLS
            var moveX = value.Get<float>();

            // ANIMATIONS
            animator.SetFloat("Speed", Mathf.Abs(moveX));

            // PLAYER DIRECTION
            if (moveX < 0.0f && facingRight)
            {
                FlipPlayer();
            }
            else if (moveX > 0.0f && !facingRight)
            {
                FlipPlayer();
            }

            // PHYSICS
            myRigidbody2D.velocity = new Vector2(moveX * moveSpeed, myRigidbody2D.velocity.y);
        }
    }

    private void FlipPlayer()
    {
        facingRight = !facingRight;
        gameObject.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
    }

    public void startAttacking()
    {
        blackSwordCollider.enabled = true;
    }

    public void stopAttacking()
    {
        blackSwordCollider.enabled = false;
    }
    void OnRecord(InputValue value){
        // ignore locked input
        gameMediator.Record();
    }

    public void Die(){
        gameMediator.handleDeath(gameObject);
    }

    public void setPosition(Vector2 position){
        gameObject.transform.position = new Vector3(position.x, position.y, 
            gameObject.transform.position.z);
    }
}
