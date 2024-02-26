using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets and handles all player keybinds.
/// </summary>
public class PlayerKeybinds : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
}
