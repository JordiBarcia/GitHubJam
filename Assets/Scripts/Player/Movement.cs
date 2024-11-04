using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;

    float xInput;
    bool jumpIsReleased;

    [Header("Movement Settings")]
    public float groundSpeed;

    [Header("Ground Settings")]
    [Range(0,1)] public float groundDecay;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    bool grounded;

    [Range(5, 15)] public float jumpSpeed;

    private void Start()
    {
        
    }

    private void Update()
    {
        GetInput();
        HandleJump();
        
    }

    private void FixedUpdate()
    {
        MoveWithInput();
        CheckGround();
        ApplyFriction();
    }

    void GetInput() 
    {
        xInput = Input.GetAxis("Horizontal");
        jumpIsReleased = Input.GetButtonUp("Jump");
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && grounded) 
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);

        }
        if (jumpIsReleased && body.velocity.y > 0) 
        {
            body.velocity = new Vector2(body.velocity.x, 0); 
        }
    }

    void MoveWithInput() 
    {
        if (Mathf.Abs(xInput) > 0)
        {
            body.velocity = new Vector2(xInput * groundSpeed, body.velocity.y);
            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    void ApplyFriction() 
    {
        if (grounded && xInput == 0 && body.velocity.y <= 0)
        {
            body.velocity *= groundDecay;
        }
    }

    void CheckGround() 
    {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }
}
