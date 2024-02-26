using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles player movement.
/// </summary>
[RequireComponent(typeof(PlayerKeybinds))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Walk speed.")]
    public float walkSpeed = 5f;
    [Tooltip("Sprint speed.")]
    public float sprintSpeed = 7f;
    [Tooltip("Strength of drag on player while grounded.")]
    public float groundDrag = 5f;
    [Tooltip("Current movement speed.")]
    float moveSpeed = 5f;
    [Tooltip("How fast speed should change when lerping.")]
    float speedChangeFactor;

    [Header("Dashing")]
    [Tooltip("Max speed while dashing. Should usually be higher than sprint speed.")]
    public float dashSpeed = 10f;
    [Tooltip("How fast the end-of-dash momentum should slow back to normal speed.")]
    public float dashSpeedChangeFactor;
    public bool isDashing;
    [Tooltip("Maximum upward speed.")]
    [HideInInspector] public float maxYSpeed = 3f;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    [Header("Crouching")]
    [Tooltip("Crouch speed.")]
    public float crouchSpeed = 3f;
    [Tooltip("Height scale of player when crouching.")]
    public float crouchYScale = 0.5f;
    [Tooltip("Original height scale of player.")]
    float startYScale;

    [Header("Jumping")]
    [Tooltip("Jump power.")]
    public float jumpPower = 8f;
    [Tooltip("Jumping cooldown.")]
    public float jumpCooldown = 0.25f;
    [Tooltip("Multiplier to movespeed when jumping.")]
    public float airMultiplier = 0.5f;
    bool readyToJump;
    bool isJumping;

    [Header("Ground Check")]
    [Tooltip("Player height in units (e.g. Capsule is 2 by default).")]
    public float playerHeight;
    [Tooltip("The Layer to detect as ground.")]
    public LayerMask groundLayer;
    bool isGrounded;

    [Header("Slope Handling")]
    [Tooltip("Multiplier to movespeed when on a slope.")]
    public float slopeMovePower = 10f;
    [Tooltip("Max slope angle; slopes at larger angles will prevent movement.")]
    public float maxSlopeAngle;
    float slopeAngle;
    RaycastHit slopeHit;

    [Header("Object References")]
    [Tooltip("Transform to use to record player orientation.")]
    public Transform orientation;
    PlayerKeybinds keybinds;

    [Tooltip("Current player movement state.")]
    public MovementState state;

    // Input vars
    float horizontalInput;
    float verticalInput;

    // Other references
    Vector3 moveDirection;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        keybinds = GetComponent<PlayerKeybinds>();

        startYScale = transform.localScale.y;

        ResetJump();
    }

    private void Update()
    {
        StateHandler();
        GetInput();
        SpeedControl();

        // Check if player is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        // Apply drag
        rb.drag = (new[] { MovementState.Walking, MovementState.Sprinting, MovementState.Crouching }.Contains(state)) ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    /// <summary>
    /// Handles and sets player movement state.
    /// </summary>
    void StateHandler()
    {
        // Dashing
        if (isDashing)
        {
            state = MovementState.Dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        // Crouching
        else if (Input.GetKey(keybinds.crouchKey))
        {
            state = MovementState.Crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Sprinting
        else if (isGrounded && Input.GetKey(keybinds.sprintKey))
        {
            state = MovementState.Sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Walking
        else if (isGrounded)
        {
            state = MovementState.Walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Jumping or midair
        else
        {
            state = MovementState.Jumping;

            if (desiredMoveSpeed < sprintSpeed) desiredMoveSpeed = walkSpeed;
            else desiredMoveSpeed = sprintSpeed;
        }

        bool desiredMoveSpeedChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.Dashing) keepMomentum = true;

        if (desiredMoveSpeedChanged)
        {
            // Smoothly transition to desired move speed.
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    /// <summary>
    /// Checks for and gets player input.
    /// </summary>
    void GetInput()
    {
        // Get input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        #region Crouching
        // Check for crouching
        if (Input.GetKeyDown(keybinds.crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // Stop crouching
        if (Input.GetKeyUp(keybinds.crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
        #endregion

        #region Jumping
        // Check for jumping
        if (Input.GetKey(keybinds.jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        #endregion
    }

    /// <summary>
    /// Calculates and moves the player.
    /// </summary>
    void MovePlayer()
    {
        if (state == MovementState.Dashing) return;

        // Calculate movement direction vector
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Check if player is on a slope
        if (OnSlope() && !isJumping)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * slopeMovePower, ForceMode.Force);

            // Add downward force to prevent bounciness
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.Force);
            }
        }

        // Turn off gravity while on a slope to prevent player from sliding down
        rb.useGravity = !OnSlope();

        // If grounded, move normally
        if (isGrounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        // If in air, apply air multiplier
        else rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    /// <summary>
    /// Controls player velocity so it doesn't exceed a given speed.
    /// </summary>
    void SpeedControl()
    {
        // Limit speed while on slope
        if (OnSlope())
        {
            if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // Limit speed while on ground or in midair
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Limit velocity if greater than moveSpeed
            if (flatVelocity.magnitude > moveSpeed)
            {
                // Calculate what max velocity should be, then apply
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }

        // Limit Y velocity
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    /// <summary>
    /// Makes the player jump.
    /// </summary>
    void Jump()
    {
        isJumping = true;
        
        // First reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    /// <summary>
    /// Resets readyToJump.
    /// </summary>
    void ResetJump()
    {
        readyToJump = true;
        isJumping = false;
    }

    /// <summary>
    /// Detects if the player is on a slope, taking maxSlopeAngle into account.
    /// </summary>
    /// <returns></returns>
    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            slopeAngle = angle;
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    /// <summary>
    /// Returns a vector projected onto a slope.
    /// </summary>
    /// <returns></returns>
    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    /// <summary>
    /// Smoothly lerps moveSpeed to desired value.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SmoothLerpMoveSpeed()
    {
        float time = 0;
        float speedDiff = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startSpeed = moveSpeed;

        while (time < speedDiff)
        {
            moveSpeed = Mathf.Lerp(startSpeed, desiredMoveSpeed, time / speedDiff);
            time += Time.deltaTime * speedChangeFactor;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }
}

/// <summary>
/// Player movement states.
/// </summary>
public enum MovementState
{
    Idle,
    Walking,
    Sprinting,
    Crouching,
    Dashing,
    Jumping
}
