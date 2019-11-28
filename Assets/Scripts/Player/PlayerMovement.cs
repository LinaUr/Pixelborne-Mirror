<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 20f;
    public bool facingRight = true;

    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    public bool isGrounded = true;

    private Rigidbody2D rigidbody2D;

    void Start ()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
    }

    public void OnJump(InputValue value)
    {
        if (isGrounded)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
        }
    }

    public void OnMovement(InputValue value)
    {
        // CONTROLS
        var moveX = value.Get<float>();

        // ANIMATIONS

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
        var rigibody2D = gameObject.GetComponent<Rigidbody2D>();
        rigibody2D.velocity = new Vector2(moveX * moveSpeed, rigibody2D.velocity.y);
    }

    private void FlipPlayer()
    {
        facingRight = !facingRight;
        gameObject.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
    }

}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 20f;
    public bool facingRight = true;

    public Transform groundCheckPoint;
    public float groundCheckY = 0.1f;
    public LayerMask whatIsGround;
    public bool isGrounded = true;
    public Animator animator;
    public string otherPlayerTag;
    public Rigidbody2D rigidbody2D;
    public Collider2D playerCollider;
    public float attackDirection;

    private double lastTimeAttacked = -10000;
    private double attackDuration;
    private bool attacking = false;
    private Collider2D blackSwordCollider;
    private int currentAttackAnimationParameter = 0;
    
    public static float ATTACK_DIRECTION_DEADZONE = 0.1f;
    private static string[] ATTACK_ANIMATOR_PARAMETERS = {"AttackingUp", "Attacking", "AttackingDown"};
    

    void Start ()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        playerCollider = gameObject.GetComponent<Collider2D>();
        attackDuration = getAnimationLength("Player_1_attack");
        blackSwordCollider = GameObject.Find("BlackSword").GetComponent<Collider2D>();
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
        // isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckY, whatIsGround);
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
        // Debug.Log(collider.gameObject.name + " : " + collider.gameObject.tag + " : " + Time.time);
        Debug.Log("Got HIT!!!!!!¹11!!");
    }

    void OnAttack(InputValue value){
        if(lastTimeAttacked < 0){
            attacking = true;
            determineAttackingParameter(attackDirection);
            animator.SetBool(ATTACK_ANIMATOR_PARAMETERS[currentAttackAnimationParameter], attacking);
            lastTimeAttacked = attackDuration;
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
        Debug.Log(currentAttackAnimationParameter);
    
    }

    void OnJump(InputValue value)
    {
        if (isGrounded)
        {
            animator.SetBool("IsJumping", true);
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
        }
    }

    void OnAttackDirection(InputValue value){
        attackDirection = value.Get<float>();
    }

    void OnMovement(InputValue value)
    {
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
        var rigibody2D = gameObject.GetComponent<Rigidbody2D>();
        rigibody2D.velocity = new Vector2(moveX * moveSpeed, rigibody2D.velocity.y);
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

}
>>>>>>> origin/4-make-camera-switch-between-level-screens
