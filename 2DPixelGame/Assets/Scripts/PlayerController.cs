using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 10f;
    public float dashSpeed = 20f;
    public bool doubleJumpEnabled = false;
    public float crouchSpeed = 7f;                      //player cannot dash while crouching
    public float jumpVelocity = 0.00001f;

    private Rigidbody2D my_rigidbody;
    private bool my_grounded;
    private bool my_ceilinged;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
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
        my_grounded = isGrounded();
        my_ceilinged = isCeilinged();

        Jump();
    }

    private void Jump()
    {
        //if player isn't touched ceiling and still on the ground

        if (my_grounded)
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
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift))
            transform.position += new Vector3(1, 0) * dashSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift))
            transform.position -= new Vector3(1, 0) * dashSpeed * Time.deltaTime;
        //walk
        else if (Input.GetKey(KeyCode.D))
            transform.position += new Vector3(1, 0) * walkSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.A))
            transform.position -= new Vector3(1, 0) * walkSpeed * Time.deltaTime;
        else
        {
            my_rigidbody.velocity = new Vector3(0, my_rigidbody.velocity.y);
        }
    }

    private bool isGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D groundHit  = Physics2D.Raycast(circleCollider.bounds.center, Vector2.down, circleCollider.bounds.extents.y + extraHeight, platformLayerMask);
        Color rayColor = Color.red;

        if (groundHit.collider != null)
            rayColor = Color.green;
        Debug.DrawRay(circleCollider.bounds.center, Vector2.down * (circleCollider.bounds.extents.y + extraHeight));
        if (groundHit.collider != null)
            Debug.Log("Hit ground");

        return groundHit.collider != null;
    }

    private bool isCeilinged()
    {
        float extraHeight = 0.1f;
        RaycastHit2D ceilingHit = Physics2D.Raycast(boxCollider.bounds.center, Vector2.up, boxCollider.bounds.extents.y + extraHeight, platformLayerMask);
        Color rayColor = Color.red;

        if (ceilingHit.collider != null)
            rayColor = Color.green;
        Debug.DrawRay(boxCollider.bounds.center, Vector2.up * (boxCollider.bounds.extents.y + extraHeight));

        return ceilingHit.collider != null;
    }

  
}
