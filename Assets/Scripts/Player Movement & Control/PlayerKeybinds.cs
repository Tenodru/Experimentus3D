using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets and handles all player keybinds.
/// </summary>
public class PlayerKeybinds : MonoBehaviour
{
    [Header("Movement Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Weapon Keybinds")]
    public KeyCode primaryFireKey = KeyCode.Mouse0;
    public KeyCode secondaryFireKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;
}
