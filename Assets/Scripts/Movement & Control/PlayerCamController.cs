using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles camera control, movement, and effects.
/// </summary>
public class PlayerCamController : MonoBehaviour
{
    [Header("Mouse Sens")]
    [Tooltip("Mouse X sensitivity.")]
    public float sensX;
    [Tooltip("Mouse Y sensitivity.")]
    public float sensY;

    [Header("Object References")]
    [Tooltip("Transform used to record player orientation.")]
    public Transform orientation;

    Camera cam;
    float startFOV;
    float dashFOV;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GetComponent<Camera>();
        startFOV = cam.fieldOfView;
        dashFOV = startFOV + 10f;
    }

    private void Update()
    {
        // Get and calculate mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp the up/down look angle.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate camera & orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    /// <summary>
    /// Prebuilt FOV lerp for dashing.
    /// </summary>
    /// <param name="duration">Duration to lerp the FOV over.</param>
    [Obsolete("Use LerpFOVIncrease instead.")]
    public void LerpDashFOV(float duration = 0.1f)
    {
        StartCoroutine(co_LerpCamFOV(dashFOV, duration));
    }

    /// <summary>
    /// Lerps the camera FOV to the specified value over the specified duration.
    /// </summary>
    /// <param name="endValue">End FOV value to lerp to.</param>
    /// <param name="duration">Duration to lerp the FOV over.</param>
    public void LerpFOV(float endValue, float duration = 0.1f)
    {
        StartCoroutine(co_LerpCamFOV(endValue, duration));
    }

    /// <summary>
    /// Lerps the camera FOV to the specified increased value over the specified duration.
    /// </summary>
    /// <param name="fovIncrease">How many degrees to increase the current FOV by.</param>
    /// <param name="duration">Duration to lerp the FOV over.</param>
    public void LerpFOVIncrease(float fovIncrease, float duration = 0.1f)
    {
        StartCoroutine(co_LerpCamFOV(startFOV + fovIncrease, duration));
    }

    /// <summary>
    /// Lerps the camera FOV to its original value.
    /// </summary>
    public void ResetFov()
    {
        StartCoroutine(co_LerpCamFOV(startFOV, 0.1f));
    }

    /// <summary>
    /// Lerps the Camera FOV to the specified value over the specified duration.
    /// </summary>
    /// <param name="endValue">The end FOV value to lerp to.</param>
    /// <param name="duration">The duration to lerp over.</param>
    /// <returns></returns>
    IEnumerator co_LerpCamFOV(float endValue, float duration)
    {
        float time = 0;
        float startFOV = cam.fieldOfView;

        while (time < duration)
        {
            cam.fieldOfView = Mathf.Lerp(startFOV, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = endValue;
    }
}
