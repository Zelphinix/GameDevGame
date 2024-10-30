using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed = 3f;
    public float jumpHeight = 5f;

    public float speedBoost = 30f;
    public float speedBoostDuration = 10f;

    public float lightBoostMultiplier = 2f;
    public float lightBoostDuration = 10f;

    public float jumpBoostDuration = 10f;

    public float dashSpeed = 10f;
    public float dashDuration = 1f;
    public float dashCooldown = 1f;
    public float dashBoostDuration = 10f;

    private float moveSpeed;
    private bool isSpeedBoosted = false;
    private bool isJumpBoosted = false;
    private bool isDashBoosted = false;
    private bool canDoubleJump = false;
    private bool canDash = false;
    private bool isDashing = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Light2D currentLight;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentLight = GetComponent<Light2D>();
        moveSpeed = normalSpeed;
    }

    void Update()
    {
        if (!isDashing)
        {
            float moveInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Keep the y velocity unchanged

            sr.flipX = moveInput < 0;

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash(moveInput));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsGrounded())
                {
                    // Normal jump
                    Debug.Log(IsGrounded().ToString());
                    rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                    canDoubleJump = isJumpBoosted; // Enable double jump if jump boosted
                }
                else if (canDoubleJump)
                {
                    // Double jump
                    rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                    canDoubleJump = false; // Disable double jump after use
                }
            }
        }
    }

    //????????????????
    private bool IsGrounded()
    {
        // Get the player's collider
        Collider2D collider = GetComponent<Collider2D>();

        // Check if there are any colliders directly below the player
        return Physics2D.OverlapBox(collider.bounds.center - new Vector3(0, collider.bounds.extents.y, 0),
                                     new Vector2(collider.bounds.size.x, 0.1f),
                                     0,
                                     LayerMask.GetMask("Ground")) != null; // Replace "Ground" with your ground layer
    }

    public void ActivateSpeedBoost(Color colorParam)
    {
        if (!isSpeedBoosted)
        {
            isSpeedBoosted = true;
            moveSpeed = speedBoost;
            currentLight.color = colorParam;
            Invoke("DeactivateSpeedBoost", speedBoostDuration);
        }
    }

    void DeactivateSpeedBoost()
    {
        isSpeedBoosted = false;
        currentLight.color = Color.white;
        moveSpeed = normalSpeed;
    }

    public void ActivateLightBoost(Color colorParam)
    {
        currentLight.pointLightOuterRadius *= lightBoostMultiplier;
        currentLight.color = colorParam;
        Invoke("DeactivateLightBoost", lightBoostDuration);
    }

    void DeactivateLightBoost()
    {
        currentLight.pointLightOuterRadius /= lightBoostMultiplier;
        currentLight.color = Color.white;
    }

    public void ActivateJumpBoost(Color colorParam)
    {
        isJumpBoosted = true;
        currentLight.color = colorParam;
        Invoke("DeactivateJumpBoost", jumpBoostDuration);
    }

    void DeactivateJumpBoost()
    {
        isJumpBoosted = false;
        currentLight.color = Color.white;
    }

    public void ActivateDashBoost(Color colorParam)
    {
        canDash = true;
        currentLight.color = colorParam;
        Invoke("DeactivateDashBoost", dashBoostDuration);
    }
    public void DeactivateDashBoost()
    {
        canDash = false;
        currentLight.color = Color.white;
    }

    private IEnumerator Dash(float moveInput)
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;  // Disable gravity during dash

        // Set dash velocity
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed * moveInput, 0);

        yield return new WaitForSeconds(dashDuration);  // Wait for dash duration

        rb.velocity = Vector2.zero;  // Stop dash velocity
        rb.gravityScale = originalGravity;  // Restore gravity
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);  // Cooldown period
        if (isDashBoosted)
        {
            canDash = true;  // Allow dash again
        }
    }
}
