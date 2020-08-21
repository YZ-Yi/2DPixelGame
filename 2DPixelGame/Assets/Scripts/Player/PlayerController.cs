using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 10f;
    public float dashSpeed = 20f;
    public bool doubleJumpEnabled = false;
    public float crouchSpeed = 7f;                      //player cannot dash while crouching
    public float jumpVelocity = 25f;
    public Transform ceilingCheckFlat;
    public Animator animator;
    public Transform respawnPoint;

    private LevelManger gameLevelManager;

    private Rigidbody2D my_rigidbody;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private PlayerStat playerStat;

    private bool isGrounded;
    private bool isJumping = false;
    private bool canJump;
    private bool isDashing;
    private bool isCrouching = false;
    private bool isCtrlPressed = false;
    private bool isFirstJump = false;

    private int xInput;
    private int facingDir = 1;


    [SerializeField] public LayerMask platformLayerMask;

    private void Awake()
    {
        my_rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        gameLevelManager = FindObjectOfType<LevelManger>();
        playerStat = FindObjectOfType<PlayerStat>();
    }

    private void FixedUpdate()
    {
        my_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        CeilingCheck();
        GroundCheck();

        Move();
    }

    private void Update()
    {
        checkInput();
        animator.SetFloat("Speed", Mathf.Abs(my_rigidbody.velocity.x));
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
        {
            Jump();
            Debug.Log("Pressed Space");
        }

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

      //  Debug.Log(canJump);

        if (canJump)
        {
            my_rigidbody.velocity = Vector2.up * jumpVelocity;
            Debug.Log("Jumping" + my_rigidbody.velocity);
            isJumping = true;

            if (!isFirstJump)
                isFirstJump = true;
            else
                isFirstJump = false;

            animator.SetBool("IsJumping", true);
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
            //Debug.Log("On the flat ground");

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

            //Debug.Log("In air");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerStat.TakeDamage(5);
        }

        //Debug.Log("speed: " + my_rigidbody.velocity);
    }

    private void GroundCheck()
    {
        float extraHeight = 0.01f;
        RaycastHit2D groundHit = Physics2D.Raycast(circleCollider.bounds.center, Vector2.down, circleCollider.bounds.extents.y + extraHeight, platformLayerMask);
        Color rayColor = Color.red;

        if (groundHit.collider != null)
        {
            rayColor = Color.green;
            isGrounded = true;
            //isJumping = false;

        }
        else
            isGrounded = false;
        Debug.DrawRay(circleCollider.bounds.center, Vector2.down * (circleCollider.bounds.extents.y + extraHeight), rayColor);
        //if (groundHit.collider != null)
        // Debug.Log("Hit ground");

        if (my_rigidbody.velocity.y <= 0f)
        {
            isJumping = false;

        }

        if (my_rigidbody.velocity.y <= 0f && isGrounded)
            animator.SetBool("IsJumping", false);

        if (isGrounded && !isJumping)
            canJump = true;
        else if (!isGrounded && isFirstJump && doubleJumpEnabled)
            canJump = true;
        else
            canJump = false;
       // Debug.Log("CanJump " + canJump);

    }

    private void Flip()
    {
        facingDir *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if(target.tag == "FallDetector")
        {
            float fallDamage = 45;
            gameLevelManager.Respawn();

            playerStat.TakeDamage(fallDamage);
        }
    }
}
