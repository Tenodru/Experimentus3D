using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Handles dash movement.
/// </summary>
[RequireComponent(typeof(PlayerMovementController))]
public class DashController : MonoBehaviour
{
    [Header("Dash Movement")]
    [Tooltip("Flat dash force.")]
    public float dashForce = 30f;
    [Tooltip("Upward dash force.")]
    public float dashUpwardForce = 0f;
    [Tooltip("Maximum upward speed while dashing.")]
    public float maxDashYSpeed = 3f;
    [Tooltip("How long a dash 'lasts'.")]
    public float dashDuration = 0.25f;

    [Header("Cooldown")]
    [Tooltip("Dash cooldown.")]
    public float dashCooldown = 1f;
    float dashCooldownTimer;

    [Header("Settings")]
    [Tooltip("If true, use the player cam instead of the orientation transform to determine forward direction.")] 
    public bool useCamForward = true;
    [Tooltip("If true, allow dashing in all directions.")]
    public bool allowAllDirections = true;
    [Tooltip("If true, disable gravity when dashing.")]
    public bool disableGravity = true;
    [Tooltip("If true, reset velocity before dashing.")]
    public bool resetVelocity = true;

    [Header("Camera Effects")]
    [Tooltip("The player camera.")]
    public PlayerCamController cam;
    [Tooltip("How much the FOV should increase when dashing.")]
    public float dashFOVIncrease;

    [Header("References")]
    [Tooltip("Transform used for recording player orientation.")]
    public Transform orientation;
    [Tooltip("Player camera parent.")]
    public Transform playerCam;
    Rigidbody rb;
    PlayerMovementController playerMovementController;

    Vector3 delayedForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovementController = GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) Dash();

        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Calculates and applies dash force.
    /// </summary>
    void Dash()
    {
        // If dash is not off cooldown, do not dash
        if (dashCooldownTimer > 0) return;
        else dashCooldownTimer = dashCooldown;

        playerMovementController.isDashing = true;
        playerMovementController.maxYSpeed = maxDashYSpeed;

        cam.LerpFOVIncrease(dashFOVIncrease);

        // Set the transform we want to use to orient our forward direction
        Transform forwardTransform;
        if (useCamForward) forwardTransform = playerCam;
        else forwardTransform = orientation;

        // Calculate dash direction and force vectors
        Vector3 direction = GetDirection(forwardTransform);
        Vector3 force = direction * dashForce + orientation.up * dashUpwardForce;
        delayedForce = force;

        // Disable gravity, if setting enabled
        if (disableGravity) rb.useGravity = false;

        Invoke(nameof(ApplyDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }

    /// <summary>
    /// Applies the dash force. Intended to be used witha delay to give PlayerMovementController time to switch states.
    /// </summary>
    void ApplyDashForce()
    {
        if (resetVelocity) rb.velocity = Vector3.zero;
        rb.AddForce(delayedForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Handles end-of-dash actions.
    /// </summary>
    void ResetDash()
    {
        playerMovementController.isDashing = false;
        playerMovementController.maxYSpeed = 0;

        cam.ResetFov();

        if (disableGravity) rb.useGravity = true;
    }

    /// <summary>
    /// Calculates and returns the direction the player wants to dash in.
    /// </summary>
    /// <param name="forwardT">The transform to use for forward orientation purposes.</param>
    /// <returns></returns>
    Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction;

        // Calculate direction if we can use all directions; otherwise just use forward
        if (allowAllDirections) direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else direction = forwardT.forward;

        // Also just use forward if the player isn't moving
        if (verticalInput == 0 && horizontalInput == 0) direction = forwardT.forward;

        return direction.normalized;
    }
}
