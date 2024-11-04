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
    bool dashInputDown;
    bool dashInputUp;

    [Header("Movement Settings")]
    public float groundSpeed;

    [Header("Ground Settings")]
    [Range(0,1)] public float groundDecay;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    bool grounded;

    [Header("Jump Settings")]
    [Range(5, 15)] public float jumpSpeed;

    [Header("Dash Settings")]
    public float dashPower;
    public float dashTime;
    public float dashCooldown;
    bool canDash = true;
    bool isDashing;
    float auxDashCooldown;


    private void Start()
    {
        auxDashCooldown = dashCooldown;
    }

    private void Update()
    {
        if (isDashing) {return; }
        GetInput();
        HandleJump();
        if (dashInputDown && canDash) 
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) { return; }
        MoveWithInput();
        CheckGround();
        ApplyFriction();
    }

    void GetInput() 
    {
        xInput = Input.GetAxis("Horizontal");
        jumpIsReleased = Input.GetKeyUp(KeyCode.X);
        dashInputDown = Input.GetKeyDown(KeyCode.C);
        dashInputUp = Input.GetKeyUp(KeyCode.C);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.X) && grounded) 
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

    private IEnumerator Dash() 
    {
        canDash = false;
        isDashing = true;
        float originalGravity = body.gravityScale;
        body.gravityScale = 0;
        body.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        yield return new WaitForSeconds(dashTime);
        body.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
