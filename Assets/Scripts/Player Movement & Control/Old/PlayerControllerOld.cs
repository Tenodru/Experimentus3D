using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerOld : MonoBehaviour
{
    // Movement variables
    [Header("Movement")]
    public float walkSpeed = 5f;            // How fast the player walks.
    public float runSpeed = 10f;            // How fast the player will run.
    public float jumpPower = 5f;            // The strength of the player's jump.
    public float gravity = 10f;             // The strength of gravity.

    [Header("Mouse Look")]
    public float lookSpeed = 2f;            // How fast the player will turn with mouse movement.
    public float lookXLimit = 45f;          // The maximum angle the player can look up or down.

    Vector3 moveDirection;                  // Vector representing the direction and magnitude of player movement.
    float rotationX = 0;                    // Rotation around the X axis.

    public bool canMove = true;

    // Components
    public Camera playerCamera;
    CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Vectors representing the forward and right directions for the player, taking rotation into account.
        Vector3 forward = transform.TransformDirection(Vector3.forward);                        
        Vector3 right = transform.TransformDirection(Vector3.right);

        #region Walking and Running -------------------------------
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
    
        /* Setting currentSpeed with ternary operators
        float currentSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float currentSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        */


        // This is the classic way to write the above lines with if statements instead of ternary (?:) operators.
        float currentSpeedX = 0;
        float currentSpeedY = 0;
        if (canMove)
        {
            if (isRunning)
            {
                currentSpeedX = runSpeed * Input.GetAxis("Vertical");
                currentSpeedY = runSpeed * Input.GetAxis("Horizontal");
            } else
            {
                currentSpeedX = walkSpeed * Input.GetAxis("Vertical");
                currentSpeedY = runSpeed * Input.GetAxis("Horizontal");
            }
        }

        float moveDirectionY = moveDirection.y;
        moveDirection = (forward * currentSpeedX) + (right * currentSpeedY);
        #endregion

        #region Jumping -------------------------------------------
        if (Input.GetKey(KeyCode.Space) && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        } else
        {
            moveDirection.y = moveDirectionY;
        }

        // While player is in the air, apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #endregion

        #region Rotation ------------------------------------------
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        #endregion
    }
}


