using System.Collections;
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
