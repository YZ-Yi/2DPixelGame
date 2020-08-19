﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 10f;
    public float dashSpeed = 20f;
    public bool doubleJumpEnabled = false;
    public float crouchSpeed = 7f;                      //player cannot dash while crouching
    public float jumpVelocity = 25f;
    public Transform ceilingCheckFlat;

    private Rigidbody2D my_rigidbody;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;

    private bool isGrounded;
    private bool isJumping = false;
    private bool canJump;
    private bool isDashing;
    private bool isCrouching = false;
    private bool isCtrlPressed = false;

    private int xInput;
    private int facingDir = 1;


    [SerializeField] public LayerMask platformLayerMask;

    private void Awake()
    {
        my_rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();

    }

    private void FixedUpdate()
    {
        my_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        isGrounded = GroundCheck();
        CeilingCheck();
        Move();
    }

    private void Update()
    {
        checkInput();
        isGrounded = GroundCheck();

        if (isCrouching)
            boxCollider.enabled = false;
        else
            boxCollider.enabled = true;
        //Debug.Log(xInput + " " + slopeNormalPrep.x);
        //Debug.Log("V: " + my_rigidbody.velocity);
    }

    void checkInput()
    {
        if (Input.GetKey(KeyCode.A))
            xInput = -1;
        else if (Input.GetKey(KeyCode.D))
            xInput = 1;
        else
            xInput = 0;

        if (xInput == 1 && facingDir == -1)
        {
            Flip();
        }
        else if (xInput == -1 && facingDir == 1)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKey(KeyCode.LeftShift))
            isDashing = true;
        else
            isDashing = false;

        if (Input.GetKey(KeyCode.LeftControl))
            isCtrlPressed = true;
        else
            isCtrlPressed = false;

    }

    void  CeilingCheck()
    {
        float extraHeight = 0.1f;
        Vector2 checkPos = ceilingCheckFlat.position;
        RaycastHit2D ceilingHit = Physics2D.Raycast(checkPos, Vector2.up, extraHeight, platformLayerMask);
        Debug.DrawRay(checkPos, Vector2.up * extraHeight, Color.red);
        
        if (ceilingHit.collider != null)
            Debug.Log("Hit Ceiling");

        if (isCtrlPressed)
            isCrouching = true;
        else if (ceilingHit.collider != null)
            isCrouching = true;
        else if (!isCtrlPressed && ceilingHit.collider == null)
            isCrouching = false;


        if (isCrouching)
            boxCollider.enabled = false;
        else
            boxCollider.enabled = true;

    }


    private void Jump()
    {
        //if player isn't touched ceiling and still on the ground

        if (canJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                my_rigidbody.velocity = Vector2.up * jumpVelocity;
                isJumping = true;
                canJump = false;
            }
        }



    }
    private void Move()
    {

        if (isGrounded && !isJumping)
        {
            if (isDashing)
            {
                Vector2 velocity = new Vector2(xInput * dashSpeed, 0.0f);
                my_rigidbody.velocity = velocity;
            }
            else if (isCrouching)
            {
                Vector2 velocity = new Vector2(xInput * crouchSpeed, 0.0f);
                my_rigidbody.velocity = velocity;
            }
            else
            {
                Vector2 velocity = new Vector2(xInput * walkSpeed, 0.0f);
                my_rigidbody.velocity = velocity;

            }
            Debug.Log("On the flat ground");

        }
        else if (!isGrounded)
        {
            if (isDashing)
            {
                Vector2 velocity = new Vector2(dashSpeed * xInput, my_rigidbody.velocity.y);
                my_rigidbody.velocity = velocity;
            }
            else
            {
                Vector2 velocity = new Vector2(walkSpeed * xInput, my_rigidbody.velocity.y);
                my_rigidbody.velocity = velocity;
            }

            Debug.Log("In air");
        }

        //Debug.Log("speed: " + my_rigidbody.velocity);
    }

    private bool GroundCheck()
    {
        float extraHeight = 0.01f;
        RaycastHit2D groundHit = Physics2D.Raycast(circleCollider.bounds.center, Vector2.down, circleCollider.bounds.extents.y + extraHeight, platformLayerMask);
        Color rayColor = Color.red;

        if (groundHit.collider != null)
            rayColor = Color.green;
        //Debug.DrawRay(circleCollider.bounds.center, Vector2.down * (circleCollider.bounds.extents.y + extraHeight), rayColor);
        //if (groundHit.collider != null)
           // Debug.Log("Hit ground");

        if (my_rigidbody.velocity.y <= 0.0f)
        {
            isJumping = false;
        }
        if (isGrounded && !isJumping)
        {
            canJump = true;
        }

        return groundHit.collider != null;
    }

   

  
    private void Flip()
    {
        facingDir *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

}
