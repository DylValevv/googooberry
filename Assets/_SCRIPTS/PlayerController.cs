using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // main camera
    private Transform cameraMainTransform;

    #region<Locomotion Variables>
    // input system variables
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;

    // player locomotion variables
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    // adjustable player movement values
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float rotationSpeed = 4f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    #endregion

    #region<Initializing Functions>
    /// <summary>
    /// enable the action before we use it
    /// </summary>
    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
    }

    /// <summary>
    /// disable the action when the player isn't being used
    /// </summary>
    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
    }

    /// <summary>
    /// assign the references to the components
    /// </summary>
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
    }
    #endregion

    void Update()
    {
        #region<Gravity Check>
        // see if the player should fall and if they are touching the ground
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        #endregion

        // movement handler functions
        WalkHandler();
        JumpHandler();
    }

    /// <summary>
    /// handles player input of movement along the x and z axes
    /// </summary>
    private void WalkHandler()
    {
        // take in input picked up by the MouseLook and Movement actions in the Player action map
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        // move the player in the direction that the camera is facing by the player's movement input
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        // ensure that the player is not randomly moving upward
        move.y = 0;
        controller.Move(move * Time.deltaTime * playerSpeed);

        // calculate the angle to rotate to based off of where the camera is looking
        if(movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    /// <summary>
    /// handles player jumping when triggered
    /// </summary>
    private void JumpHandler()
    {
        // if the jump action is triggered on the current frame, jump
        if (jumpControl.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
