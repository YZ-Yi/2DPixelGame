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
    private float slopeDistance = 1f;

    private Rigidbody2D my_rigidbody;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;

    private bool isGrounded;
    private bool isOnSlope = false;
    private bool isMoveable = true;
    private bool isJumping = false;
    private bool canJump;
    private bool isDashing;

    private int xInput;
    private int facingDir = 1;

    //sloep check variables
    private Vector2 slopeNormalPrep;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float slopeDownAngleOld;


    [SerializeField] public LayerMask platformLayerMask;
    [SerializeField] public float maxSlopeAngle;
    [SerializeField] public PhysicsMaterial2D fullFriction;
    [SerializeField] public PhysicsMaterial2D nonFriction;

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
        SlopeCheck();
        Move();
    }

    private void Update()
    {
        checkInput();
        isGrounded = GroundCheck();
        SlopeCheck();
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

        if (isGrounded && !isOnSlope && !isJumping)
        {
            if (isDashing)
            {
                Vector2 velocity = new Vector2(xInput * dashSpeed, 0.0f);
                my_rigidbody.velocity = velocity;
            }
            else
            {
                Vector2 velocity = new Vector2(xInput * walkSpeed, 0.0f);
                my_rigidbody.velocity = velocity;

            }
            Debug.Log("On the flat ground");

        }
        else if (isOnSlope && isGrounded && !isJumping && isMoveable)
        {
            if (isDashing)
            {
                Vector2 velocity = new Vector2(-xInput * dashSpeed * slopeNormalPrep.x, -xInput * dashSpeed * slopeNormalPrep.y);
                my_rigidbody.velocity = velocity;
            }
            else
            {
                Vector2 velocity = new Vector2(-xInput * walkSpeed * slopeNormalPrep.x, -xInput * walkSpeed * slopeNormalPrep.y);
                my_rigidbody.velocity = velocity;
            }
            Debug.Log("Moveable slope");

        }
        
        else if (!isGrounded)
        {
            if (!isMoveable && xInput * my_rigidbody.velocity.x > 0)//trying to move on unmoveable slope
            {
                Vector2 velocity = new Vector2(0, 0);
                my_rigidbody.velocity = velocity;
                Debug.Log("Set UnMove");
            }
            else
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
        if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }

        return groundHit.collider != null;
    }

    private void SlopeCheck()
    {
        //Vector2 checkPos = circleCollider.bounds.center - new Vector3(0.0f, circleCollider.bounds.size.y / 2 );
        Vector2 checkPos = circleCollider.bounds.center - new Vector3(0.0f, circleCollider.bounds.size.y / 2);
        //Debug.DrawLine(circleCollider.bounds.center, checkPos, Color.red);

        SlopeHorizontalCheck(checkPos);
        SlopeVerticalCheck(checkPos);

        //Debug.Log("Size" + circleCollider.bounds.size.y);
    }

    private void SlopeHorizontalCheck(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeDistance, platformLayerMask);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeDistance, platformLayerMask);

        Debug.DrawRay(checkPos, transform.right * slopeDistance, Color.blue);
        Debug.DrawRay(checkPos, -transform.right * slopeDistance, Color.blue);
        if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            Debug.DrawRay(slopeHitBack.point, slopeHitBack.normal, Color.red);
        }
        else if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            Debug.DrawRay(slopeHitFront.point, slopeHitFront.normal, Color.red);

        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    private void SlopeVerticalCheck(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeDistance, platformLayerMask);

        if (hit)
        {
            slopeNormalPrep = Vector2.Perpendicular(hit.normal);
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }


            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPrep, Color.yellow);
            Debug.DrawRay(hit.point, hit.normal, Color.blue);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
            isMoveable = false;
        else
            isMoveable = true;
        //Debug.Log("Current angle:" + slopeDownAngle + " " + slopeSideAngle);

        if (isOnSlope && isMoveable && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            my_rigidbody.sharedMaterial = fullFriction;
        else
            my_rigidbody.sharedMaterial = nonFriction;
    
    }

    private void Flip()
    {
        facingDir *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

}
