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

    //sloep check variables
    private Vector2 slopeNormalPrep;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float slopeDownAngleOld;
    [SerializeField] private float maxSlopeAngle;
  

    [SerializeField] private LayerMask platformLayerMask;

    private void Awake()
    {
        my_rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        my_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        Move();
    }

    private void Update()
    {
        isGrounded = GroundCheck();

        SlopeCheck();
        Jump();
    }

    private void Jump()
    {
        //if player isn't touched ceiling and still on the ground

        if (isGrounded || isOnSlope)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Player is on the ground");
                my_rigidbody.velocity = Vector2.up * jumpVelocity;
            }
        }
       


    }
    private void Move()
    {
        //dash
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift) && isMoveable)
            my_rigidbody.velocity = new Vector2(1 * dashSpeed, my_rigidbody.velocity.y);
        else if (Input.GetKey(KeyCode.D) && isMoveable)
            my_rigidbody.velocity = new Vector2(1 * walkSpeed, my_rigidbody.velocity.y);
        else if (Input.GetKey(KeyCode.A) && isMoveable)
            my_rigidbody.velocity = new Vector2(-1 * walkSpeed, my_rigidbody.velocity.y);

        else
        {
            my_rigidbody.velocity = new Vector3(0, my_rigidbody.velocity.y);
        }
    }

    private bool GroundCheck()
    {
        float extraHeight = 0.1f;
        RaycastHit2D groundHit  = Physics2D.Raycast(circleCollider.bounds.center, Vector2.down, circleCollider.bounds.extents.y + extraHeight, platformLayerMask);
        Color rayColor = Color.red;

        if (groundHit.collider != null)
            rayColor = Color.green;
        //Debug.DrawRay(circleCollider.bounds.center, Vector2.down * (circleCollider.bounds.extents.y + extraHeight), rayColor);
        if (groundHit.collider != null)
            Debug.Log("Hit ground");

        return groundHit.collider != null;
    }

    private void SlopeCheck()
    {
        //Vector2 checkPos = circleCollider.bounds.center - new Vector3(0.0f, circleCollider.bounds.size.y / 2 );
        Vector2 checkPos = transform.position - new Vector3(0.0f, circleCollider.bounds.size.y / 2);
        Debug.DrawLine(circleCollider.bounds.center, checkPos, Color.black); 

        SlopeHorizontalCheck(checkPos);
        SlopeVerticalCheck(checkPos);

    }

    private void SlopeHorizontalCheck(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeDistance, platformLayerMask);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeDistance, platformLayerMask);

        if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
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
           
            if(slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }


            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPrep, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.blue);
            Debug.Log("slopeDownAngle");
            Debug.Log(slopeDownAngle);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
            isMoveable = false;
        else
            isMoveable = true;
    }
}
