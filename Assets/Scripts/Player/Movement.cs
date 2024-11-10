using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;

    float xInput;
    bool jumpIsReleased;
    bool dashInputDown;
    bool dashInputUp;
    float originalGravityScale;

    [Header("Movement Settings")]
    public float groundSpeed;

    [Header("Ground Settings")]
    [Range(0,1)] public float groundDecay;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    public bool grounded;

    [Header("Jump Settings")]
    [Range(5, 15)] public float jumpSpeed;

    [Header("Coyote Time Settings")]
    public float coyoteTime = 0.2f; // Duración del Coyote Time en segundos
    private float coyoteTimeCounter; // Temporizador de Coyote Time
    private bool hasJumped;

    [Header("Dash Settings")]
    public float dashPower;
    public float dashTime;
    public float dashCooldown;
    bool canDash = true;
    bool isDashing;
    float auxDashCooldown;

    [Header("WallJump")]
    public LayerMask wallMask;
    public float wallJumpSpeed;
    public bool isWalling;
    public BoxCollider2D wallCheck;
    public float wallJumpCooldown = 0.2f; // Cooldown para evitar escalada
    private bool canWallJump = true; // Control para permitir o no el wall jump



    private void Start()
    {
        auxDashCooldown = dashCooldown;
        originalGravityScale = body.gravityScale ;
    }

    private void Update()
    {
        if (isDashing) {return; }
        GetInput();
        HandleJump();
        HandleWallJump();
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
        CheckWall();
        ApplyFriction();
    }

    void GetInput() 
    {
        xInput = Input.GetAxis("Horizontal");
        jumpIsReleased = Input.GetKeyUp(KeyCode.X);
        dashInputDown = Input.GetKeyDown(KeyCode.Z);
        dashInputUp = Input.GetKeyUp(KeyCode.Z);
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
    private void HandleJump()
    {
        // Si el personaje está en el suelo, reiniciamos el temporizador de Coyote Time y permitimos saltar de nuevo
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
            hasJumped = false; // Permitir salto porque está en el suelo
        }
        else
        {
            // Si no está en el suelo, descontamos el tiempo de Coyote Time
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Permitir el salto solo si el personaje no ha saltado y el contador de Coyote Time es mayor a 0
        if (Input.GetKeyDown(KeyCode.X) && coyoteTimeCounter > 0f && !hasJumped)
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            hasJumped = true; // Marcar que el personaje ha saltado para evitar dobles saltos
            coyoteTimeCounter = 0f; // Reiniciar el temporizador para evitar múltiples saltos en el aire
        }

        // Cancelar el impulso del salto si el jugador suelta el botón y la velocidad es positiva
        if (jumpIsReleased && body.velocity.y > 0)
        {
            body.velocity = new Vector2(body.velocity.x, 0);
        }
    }
    private void HandleWallJump()
    {
        if (isWalling && !grounded && canWallJump)
        {
            // Reducir la gravedad para que el personaje se deslice lentamente por la pared
            body.gravityScale = 0.05f;
            hasJumped = false;

            // Añadir una pequeña velocidad de caída controlada para evitar el deslizamiento hacia arriba
            if (body.velocity.y > -0.5f)
            {
                body.velocity = new Vector2(body.velocity.x, -0.5f);
            }

            // Verificar si el jugador quiere saltar (presiona el botón de salto)
            if (Input.GetKeyDown(KeyCode.X) && xInput != Mathf.Sign(transform.localScale.x)) // Solo si se mueve en dirección opuesta a la pared
            {
                // Determinar la dirección del salto en función de hacia qué lado está mirando el personaje
                float jumpDirection = transform.localScale.x == 1 ? -1 : 1;

                // Aplicar la velocidad de wall jump en la dirección opuesta y hacia arriba
                body.velocity = new Vector2(jumpDirection * wallJumpSpeed, wallJumpSpeed);

                // Marcar que el personaje ha saltado para evitar múltiples saltos
                hasJumped = true;

                // Cambiar la dirección de la escala para que mire hacia el lado opuesto después del wall jump
                transform.localScale = new Vector3(jumpDirection, 1, 1);

                // Activar cooldown para evitar escalada
                StartCoroutine(WallJumpCooldown());
            }
        }
        else if (!isWalling)
        {
            // Restaurar la gravedad original cuando el personaje ya no esté en la pared
            body.gravityScale = originalGravityScale;
            hasJumped = true;
        }
    }
    private IEnumerator WallJumpCooldown()
    {
        canWallJump = false;  // Desactivar posibilidad de saltar de la pared
        yield return new WaitForSeconds(wallJumpCooldown);  // Esperar el cooldown
        canWallJump = true;   // Volver a permitir wall jump
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

    //CHECKEAR A QUINA DIRECCIO ESTA MIRANT
    void CheckWall() 
    {
        isWalling = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, wallMask).Length > 0;
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
