using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps player camera "attached" to the player.
/// We do this instead of directly attaching the camera to avoid bugginess with rigidbodies.
/// </summary>
public class PlayerCamMovePos : MonoBehaviour
{
    public Transform camPosition;

    void Update()
    {
        transform.position = camPosition.position;
    }
}
