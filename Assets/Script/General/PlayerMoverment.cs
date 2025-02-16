using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer rbSprite;

    [SerializeField]
    private GameObject Animation;

    [Header("Force")]
    [SerializeField] private float forceX, forceDown, speed;
    [SerializeField][Range(0, 1)] float LerpConstant = 1;

    public ParticleSystem dust;

    [Header("Jump")]
    [SerializeField] float forceJump = 30;
    [SerializeField] private bool isGrounded;
    [SerializeField] private int jumpCount;
    [SerializeField] private int maxJump = 2;


    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultipler = 2f;

    private Animator animator;
    private PlayerMoveController controls;

    public bool isControllerPlayer;
    public void Initialize(PlayerMoveController controls, bool isControllerPlayer)
    {
        this.controls = controls;
        this.isControllerPlayer = isControllerPlayer;
        //controls.Keyboard.Jump.performed += ctx => Jump();
        //controls.Keyboard.DownMovement.performed += ctx => Down();

        //Debug.Log(gameObject.name + " isController : " + isControllerPlayer);
        directionB = false;
        if (isControllerPlayer)
        {
            // Configure for player using controller
            controls.Controller.Jump.performed += ctx => Jump();
            controls.Controller.DownMovement.performed += ctx => Down();

            controls.Controller.Horizontal.performed += ctx => direction = ctx.ReadValue<float>();
            controls.Controller.Horizontal.canceled += ctx => direction = 0; // Reset when released
        }
        else
        {
            // Configure for player using keyboard
            controls.Keyboard.Jump.performed += ctx => Jump();
            controls.Keyboard.DownMovement.performed += ctx => Down();

            controls.Keyboard.Horizontal.performed += ctx => direction = ctx.ReadValue<float>();
            controls.Keyboard.Horizontal.canceled += ctx => direction = 0; // Reset when released
        }
    }

    void Start()
    {
        isGrounded = true;
        rb = GetComponent<Rigidbody2D>();
        rbSprite = Animation.GetComponent<SpriteRenderer>();
        animator = Animation.GetComponent<Animator>();
        jumpCount = maxJump;
        gameObject.SetActive(false);
    }

    void Update()
    {
        Gravity();
        RayCastDetect();
    }
    public float direction;

    public bool directionB = false;

    private bool isHit = false;

    private void FixedUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            if (!isHit)
            {
                Vector2 movement = new Vector2(direction * forceX, rb.velocity.y);
                rb.velocity = Vector2.Lerp(rb.velocity, movement, LerpConstant);

                // set animation run
                speed = Mathf.Abs(rb.velocity.x);
                animator.SetFloat("Speed", speed);

                // flip player

                if (direction > 0)
                {
                    Vector3 rotate = transform.eulerAngles;
                    rotate.y = 0;
                    Animation.transform.eulerAngles = rotate;
                    directionB = true;
                }
                else if (direction < 0)
                {
                    Vector3 rotate = transform.eulerAngles;
                    rotate.y = 180;
                    Animation.transform.eulerAngles = rotate;
                    directionB = false;
                }
            }
        }
    }

    [Header("Force baack prop")]
    [SerializeField] private float baseKnockback = 5f;  // Minimum knockback force
    //[SerializeField] private float attackStrength = 10f;  // Strength of the attack
    [SerializeField] private float damagePercentage = 1f;  // Player's current damage percentage


    public void ApplyKnockback(Vector3 collisionPosition, float attackStrength)
    {

        if (isHit) { return; }

        damagePercentage = gameObject.GetComponent<Player>().GetVulnerability();

        float knockback = baseKnockback + (attackStrength * damagePercentage / 100);
        float damageMultiplier = 1 + (damagePercentage / 100);
        knockback *= damageMultiplier;

        Vector2 direction = (transform.position - collisionPosition).normalized;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        isHit = true;

        if (rb != null)
        {
            rb.AddForce(direction * knockback, ForceMode2D.Impulse);
            Debug.Log(name + " applied knockback force: " + direction * knockback);
        }
        StartCoroutine(ResetHitStatusAfterDelay(0.5f));

        // adjust health
        gameObject.GetComponent<Player>().TakeDamage(attackStrength, isControllerPlayer ? 2 : 1);


    }

    private IEnumerator ResetHitStatusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isHit = false;
    }


    private void Jump()
    {
        if (gameObject.activeInHierarchy)
        {
            if (jumpCount > 0 || isGrounded == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, forceJump);
                jumpCount--;
                animator.SetBool("IsJumping", true);
                dust.Play();
            }
            if (jumpCount <= 0 && isGrounded == true)
            {
                jumpCount = maxJump;
            }
        }
    }

    // adjust more forceDown
    private void Down()
    {
        if (!isGrounded)
        {
            fallSpeedMultipler = 2.5f;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = maxJump;
            isGrounded = true;
            dust.Play();

            animator.SetBool("IsJumping", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            fallSpeedMultipler = 2;
        }
    }

    private void Gravity()
    {
        // falling gravity rection
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultipler; // Fall incresingly faster
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }
    [Header("Wall Climb")]
    public LayerMask wallLayer;           // Layer for the walls
    public float rayDistance = 0.4f;      // Distance for raycast detection
    public float rayHeightOffset = 0.3f;  // Height offset for additional raycast
    private bool isWallDetected;          // Checks if wall is detected

    private void RayCastDetect()
    {
        // Cast raycasts to left and right at multiple heights to check for walls
        RaycastHit2D leftWallCheckLow = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, wallLayer);
        RaycastHit2D leftWallCheckHigh = Physics2D.Raycast(transform.position + Vector3.up * rayHeightOffset, Vector2.left, rayDistance, wallLayer);
        RaycastHit2D rightWallCheckLow = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, wallLayer);
        RaycastHit2D rightWallCheckHigh = Physics2D.Raycast(transform.position + Vector3.up * rayHeightOffset, Vector2.right, rayDistance, wallLayer);

        // Detect if any of the raycasts hit a wall
        isWallDetected = leftWallCheckLow.collider != null || leftWallCheckHigh.collider != null ||
                         rightWallCheckLow.collider != null || rightWallCheckHigh.collider != null;

        // Trigger wall climb animation if wall is detected
        if (isWallDetected)
        {
            animator.SetBool("isClimbingWall", true);
            rbSprite.flipX = true;
        }
        else
        {
            animator.SetBool("isClimbingWall", false);
            rbSprite.flipX = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Draw rays at base position
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * rayDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * rayDistance);
        // Draw rays at height offset position
        Gizmos.DrawLine(transform.position + Vector3.up * rayHeightOffset, transform.position + Vector3.up * rayHeightOffset + Vector3.left * rayDistance);
        Gizmos.DrawLine(transform.position + Vector3.up * rayHeightOffset, transform.position + Vector3.up * rayHeightOffset + Vector3.right * rayDistance);
    }

    // Get set properties
    public float GetSpeed() => speed;
    public void SetSpeed(float Speed) => speed = Speed;

    public bool IsGrounded() => isGrounded;
}
