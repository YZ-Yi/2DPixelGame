using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float walkSpeed = 10f;
    private float dashSpeed = 20f;

    private Rigidbody2D my_rigidbody;
    private bool my_grounded;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;

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
    }

    private void Jump()
    {

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
    }

    private bool isGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D groundHit  = Physics2D.Raycast(circleCollider.bounds.center, Vector2.down, circleCollider.bounds.extents.y + extraHeight);
        Color rayColor = Color.red;

        if (groundHit.collider != null)
            rayColor = Color.green;
        Debug.DrawRay(circleCollider.bounds.center, Vector2.down * (circleCollider.bounds.extents.y + extraHeight));
        if (groundHit.collider != null)
            Debug.Log("Hit ground");


        return groundHit.collider != null;
    }
}
