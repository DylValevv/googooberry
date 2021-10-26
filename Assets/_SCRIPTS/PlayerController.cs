using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // main camera
    private Transform cameraMainTransform;

    [Header("Locomotion Variables")]
    #region<Locomotion Variables>
    // input system variables
    [SerializeField] private InputActionReference movementControl;

    // player locomotion variables
    private CharacterController controller;
    private Vector3 playerVelocity;

    // adjustable player movement values
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float rotationSpeed = 4f;

    // jump variables
    [SerializeField] private InputActionReference jumpControl;
    private bool groundedPlayer;
    private int jumpTimes = 0;
    [SerializeField] private float jumpTimer = 0.2f;
    [SerializeField] private float jumpHeight = 1.0f;
    private float jumpElapsedTime;
    enum ButtonState { Released, Held };
    ButtonState previousButtonState;
    private bool jumpButtonPressed;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float gravityFallMultipler;
    #endregion

    [Header("Dashing Variables")]
    #region<Dashing Variables>
    // the amount of times the player can dash consecutively while in the air
    [SerializeField] private int dashAmountMax;
    // the speed of the dash
    [SerializeField] private float dashSpeed;
    // how long to dash forward
    [SerializeField] private float dashTime;
    // the time before the player can dash again
    [SerializeField] private float dashAgainCooldown;
    Coroutine dashCoroutine;
    // used after the dashagain cooldown is done
    private bool canDash;
    // dash variables
    private int dashes;
    #endregion

    [Header("Attack Variables")]
    #region<Attack Variables>
    [SerializeField] private InputActionReference attackControl;
    [SerializeField] float attackCooldown;
    Coroutine attackCoroutine;

    Coroutine comboCooldownCoroutine;
    private int comboCount;
    // how long the player has to press Attack to do the next combo animation
    [SerializeField] private int comboTimer;
    float totalTime;

    [SerializeField] Weapon weapon;

    private bool isAttacking;
    #endregion

    #region<Initializing Functions>
    /// <summary>
    /// enable the action before we use it
    /// </summary>
    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        attackControl.action.Enable();
    }

    /// <summary>
    /// disable the action when the player isn't being used
    /// </summary>
    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        attackControl.action.Disable();
    }

    /// <summary>
    /// assign the references to the components
    /// </summary>
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;

        isAttacking = false;

        dashes = 0;
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
            dashes = 0;
        }
        #endregion

        WalkHandler();
        JumpHandler();

        // attack handler functions
        AttackHandler();
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

    #region<Jump Functions>
    /// <summary>
    /// handles player jumping when triggered. while airborne they will dash if pressing the dash button
    /// </summary>
    private void JumpHandler()
    {
        // reset the sustained jump if on the ground
        if(groundedPlayer)
        {
            jumpElapsedTime = 0;
            dashes = 0;
            canDash = true;
            jumpTimes = 0;
        }

        // return if the player is pressing and holding jump
        jumpButtonPressed = jumpControl.action.ReadValue<float>() > 0 ? true : false;

        // register how many times the player pressed the jump button
        if (jumpControl.action.triggered) jumpTimes++;

        // if the jump action is triggered and the player is on the ground, jump
        if(jumpButtonPressed && jumpTimes == 1)
        {
            // if on the ground, jump
            if(CanJump() || CanContinueJump())
            {
                Debug.Log("jump hoe");
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                //playerVelocity.y = jumpHeight;
                jumpElapsedTime += Time.deltaTime;
            }
        }
        // else dash
        else if(jumpButtonPressed && !groundedPlayer)
        {
            // if the player can dash again, dash. else do nothing
            if (canDash && dashes < dashAmountMax)
            {
                dashCoroutine = StartCoroutine(Dash());
            }
        }

        gravityValue = jumpButtonPressed ? -9.81f : -9.81f * gravityFallMultipler;

        playerVelocity.y = playerVelocity.y + (gravityValue * Time.deltaTime);
        controller.Move(playerVelocity * Time.deltaTime);

        previousButtonState = jumpButtonPressed ? ButtonState.Held : ButtonState.Released;
    }

    /// <summary>
    /// assess if the player can jump at all
    /// </summary>
    /// <returns>if the player can jump</returns>
    private bool CanJump()
    {
        return groundedPlayer && previousButtonState == ButtonState.Released;
    }

    /// <summary>
    /// assess if the player can continue to jump or not
    /// </summary>
    /// <returns>if the player can continue to jump</returns>
    private bool CanContinueJump()
    {
        return jumpElapsedTime <= jumpTimer && groundedPlayer == false;
    }
    #endregion

    #region<Attack Functions>
    /// <summary>
    /// handles the player attack when pressing the jab/slash button
    /// </summary>
    private void AttackHandler()
    {
        if(attackControl.action.triggered && !isAttacking)
        {
            attackCoroutine = StartCoroutine(Attack());
        }
    }      

    /// <summary>
    /// the player physically attacks. this is where animation timing and combos are handled
    /// </summary>
    private IEnumerator Attack()
    {
        totalTime = 0;
        comboCooldownCoroutine = StartCoroutine(CooldownCountdown(attackCooldown));
        if (comboCount < 3)
        {
            comboCount++;
        }
        else
        {
            comboCount = 0;
            comboCount++;
        }
        // ground combos
        if(groundedPlayer)
        {
            Debug.Log("ground  babey");
            string tempString = "weaponTemp" + comboCount;
            weapon.GetComponent<Animation>().Play(tempString);
        }
        // air combos
        else
        {
            Debug.Log("air  babey");
            string tempString = "airWeaponTemp" + comboCount;
            weapon.GetComponent<Animation>().Play(tempString);
        }

        isAttacking = true;
        weapon.ToggleCollider(isAttacking);

        // play the animation which moves this attacking object
        // THE FOLLOWING IS TEMPORARY

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        weapon.ToggleCollider(isAttacking);
    }

    /// <summary>
    /// countdown for player cooldowns
    /// </summary>
    private IEnumerator CooldownCountdown(float cooldownTime)
    {
        while (totalTime <= comboTimer)
        {
            totalTime += Time.deltaTime;
            yield return null;
        }

        comboCount = 0;
    }

    /// <summary>
    /// countdown for player dash combo & dash functionality
    /// </summary>
    private IEnumerator Dash()
    {
        float dashTotalTime = 0;

        // dash refractory period begin
        canDash = false;
        dashes++;

        movementControl.action.Disable();
        Vector3 moveDirection = transform.TransformDirection(Vector3.forward);

        while (dashTotalTime <= dashTime)
        {
            controller.Move(moveDirection * dashSpeed * Time.deltaTime);
            dashTotalTime += Time.deltaTime;
            yield return null;
        }

        movementControl.action.Enable();

        yield return new WaitForSeconds(dashAgainCooldown);
        // dash refractory period end
        canDash = true;
    }
    #endregion
}
