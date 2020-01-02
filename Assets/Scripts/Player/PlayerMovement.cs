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
    private bool isGrounded = true; 
    public Animator animator;
    public Rigidbody2D myRigidbody2D;
    public Collider2D playerCollider;
    public EntityHealth playerHealth;

    public EntityAttack playerAttack;
    public bool inputIsLocked = false;


    void Start ()
    {
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapArea((Vector2) playerCollider.bounds.min, 
                        (Vector2) playerCollider.bounds.min + new Vector2(playerCollider.bounds.size.x, groundCheckY), whatIsGround);
        animator.SetBool("IsJumping", !isGrounded);
    }
    
    public void resetPlayerActions(){
        playerHealth.revive();
        resetPlayerAnimations();
        resetMovement();
    }

    public void resetPlayerAnimations(){
        animator.SetBool("IsJumping", false);
        animator.SetFloat("Speed", 0);
        playerAttack.resetAttackAnimation();
    }

    public void resetMovement(){
        myRigidbody2D.velocity = new Vector2(0, myRigidbody2D.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(!inputIsLocked){
            // Die if your got hit by something else than yourself.
            // We have to explicitely look for a sword as a collider because else
            // the winning player could also be colliding with the other player and would die instead.
            if(collider.gameObject.name == "DeathZones") {
                Die();
            }
            if(collider.gameObject.name == playerAttack.playerSword.name && collider.gameObject != playerAttack.playerSword){
                // test if the attacks are canceling each other
                EntityAttack attackerEntityAttack = collider.gameObject.GetComponentInParent<EntityAttack>();
                // if the attacker has an Entity Attack the attack might be cancelled
                if(attackerEntityAttack == null || !playerAttack.attackIsCancelling(attackerEntityAttack)){
                    playerHealth.takeDamage(1);
                    if(playerHealth.isDead) {
                        Die();
                    }
                }
            }
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
