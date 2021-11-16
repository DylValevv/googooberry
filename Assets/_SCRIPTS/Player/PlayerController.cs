using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.VFX;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // main camera
    private Transform cameraMainTransform;
    public Image deathPanel;
    public GameState gameState;

    [Header("Locomotion")]
    #region<Locomotion Variables>
    // input system variables
    [SerializeField] private InputActionReference movementControl;

    // player locomotion variables
    private CharacterController controller;
    private Vector3 playerVelocity;
    enum Direction { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, Zero };
    Direction dir;
    Vector2 up = new Vector2(0, 1);
    Vector2 down = new Vector2(0, -1);
    Vector2 left = new Vector2(-1, 0);
    Vector2 right = new Vector2(1, 0);
    Vector2 upleft = new Vector2(-0.7f, 0.7f);
    Vector2 upright = new Vector2(0.7f, 0.7f);
    Vector2 downleft = new Vector2(-0.7f, -0.7f);
    Vector2 downright = new Vector2(0.7f, -0.7f);

    // adjustable player movement values
    [SerializeField] private float playerSpeed = 2.0f;
    private float OGplayerSpeed;
    [SerializeField] private float rotationSpeed = 4f;

    // jump variables
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private bool canAirAttack;
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

    [Header("Dashing")]
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

    [Header("Attack")]
    #region<Attack Variables>
    [SerializeField] private InputActionReference attackControl;
    [SerializeField] float attackCooldown;
    Coroutine attackCoroutine;

    Coroutine comboCooldownCoroutine;
    private int comboCount;
    // how long the player has to press Attack to do the next combo animation
    [SerializeField] private int comboTimer;
    // the pause between each attack. there is one for each attack to align with the animations 
    [SerializeField] private float[] timeBetweenNextAttack;
    float totalTime;

    [SerializeField] Weapon leftWeapon;
    [SerializeField] Weapon rightWeapon;

    private bool isAttacking;

    [SerializeField] private GameObject particleVisual;

    // abilitiy unlocks
    [SerializeField] private GameObject particleCollisionPrefab;
    private bool thirdHit;
    [SerializeField] private int thirdHitDamage;
    #endregion

    [Header("Animation")]
    #region<Animation Variables>
    [SerializeField] private GameObject animatedMesh;
    [SerializeField] private Animator anim;
    [SerializeField] private float blendMultiplier;
    [SerializeField] [Range(0, 1f)] private float blendTreeSmoothTime;
    // the speed the player reaches to after they attack
    [SerializeField] private float slowPlayerTo;
    // how quickly the player slows down after attacking
    [SerializeField] private float attackSmoothingIn;
    // how quickly the player goes back to their default speed after slowed after attacking
    [SerializeField] private float attackSmoothingOut;
    // like an animation controller, but instead it lets you take a base animator controller and lets you swap the clips within the animator. this is like a modular state machine
    // this lets you keep the same state machines across all charactersa
    private AnimatorOverrideController animOverride;
    // the clip that the script will look for to replace through the statemachine. everytime it finds an instance of this clip, it will swap it with the new clip
    [SerializeField] private AnimationClip utilClip;
    [SerializeField] private AnimationClip[] anims;
    private Coroutine blendTreeCoroutine;
    #endregion

    [Header("SFX")]
    #region<SFX Variables>
    public List<AudioClip> footsteps;
    public List<AudioClip> attacks;
    public List<AudioClip> impacts;
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
    /// assign and initialize the statemachine
    /// </summary>
    private void Awake()
    {
        //anim = GetComponent<Animator>();
        // create an animation override controller that is based off of our current animation controller
        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        // assign the override controller back into the animator so it can be manipulated/be used
        anim.runtimeAnimatorController = animOverride;
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

        OGplayerSpeed = playerSpeed;

        //Debug.Log("turn off third hit");
        //thirdHit = true;

        particleVisual.SetActive(false);

        dir = Direction.Zero;
    }
    #endregion

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        // airattack condition detection
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity);
        canAirAttack = hit.distance > 2;

        #region<Gravity Check>
        // see if the player should fall and if they are touching the ground

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

        #region<Enum Direction Handling>
        if (movement == up)
        {
            dir = Direction.Up;
        }
        if (movement == down)
        {
            dir = Direction.Down;
        }
        if (movement == left)
        {
            dir = Direction.Left;
        }
        if (movement == right)
        {
            dir = Direction.Right;
        }
        if (movement.x < 0 && movement.y > 0)
        {
            dir = Direction.UpLeft;
        }
        if (movement.x > 0 && movement.y > 0)
        {
            dir = Direction.UpRight;
        }
        if (movement.x < 0 && movement.y < 0)
        {
            dir = Direction.DownLeft;
        }
        if (movement.x > 0 && movement.y < 0)
        {
            dir = Direction.DownRight;
        }
        if (controller.velocity == Vector3.zero)
        {
            dir = Direction.Zero;
        }
        #endregion
        //Debug.Log(dir);

        // statemachine handling
        anim.SetBool("UtilStop", true);
        UpdateBlendTreeMoveSpeed();

        // calculate the angle to rotate to based off of where the camera is looking
        if (movement != Vector2.zero)
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
        if (groundedPlayer)
        {
            jumpElapsedTime = 0;
            dashes = 0;
            canDash = true;
            jumpTimes = 0;
            anim.SetBool("InAir", false);
        }
        // return if the player is pressing and holding jump
        jumpButtonPressed = jumpControl.action.ReadValue<float>() > 0 ? true : false;

        // register how many times the player pressed the jump button
        if (jumpControl.action.triggered)
        {
            anim.SetBool("InAir", true);
            // normalize the magnitude
            float normalizedSpeed = controller.velocity.magnitude / playerSpeed;

            // make sure the blend trees are scaled to these dynamic values
            //anim.SetFloat("MoveSpeed", normalizedSpeed);
            jumpTimes++;
        }

        // if the jump action is triggered and the player is on the ground, jump
        if (jumpButtonPressed && jumpTimes == 1)
        {
            // if on the ground, jump
            if (CanJump() || CanContinueJump())
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
                //playerVelocity.y = jumpHeight;
                jumpElapsedTime += Time.deltaTime;
            }
        }
        // else dash
        else if (jumpButtonPressed && !groundedPlayer)
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
        if (attackControl.action.triggered && !isAttacking && !canAirAttack)
        {
            attackCoroutine = StartCoroutine(Attack());
        }
    }

    /// <summary>
    /// the player physically attacks. this is where animation timing and combos are handled
    /// </summary>
    private IEnumerator Attack()
    {
        // toggle on the collider of the weapon
        isAttacking = true;

        totalTime = 0;
        if(comboCooldownCoroutine == null) comboCooldownCoroutine = StartCoroutine(CooldownCountdown(attackCooldown));
        if (comboCount < 3)
        {
            comboCount++;
        }
        else
        {
            

            comboCount = 0;
            comboCount++;
        }
        // air combos
        if (canAirAttack)
        {
            PlayAnim("AirAttack" + comboCount, true);
        }
        // ground combos
        else
        {
            if(comboCount == 3)
            {
                PlayAnim("GroundAttack3", true);
            }
            else
            {
                float rand = UnityEngine.Random.Range(1, 3);
                Debug.Log(rand);
                PlayAnim("GroundAttack" + comboCount + "_" + rand, true);
            }

        }

        leftWeapon.ToggleCollider(isAttacking);
        rightWeapon.ToggleCollider(isAttacking);

        // have the correct pause between the attack sequence to align with the attack animations. both ground and air combos should have the same timing with their respecitve indices
        // room for future implementation of ground slam and different handling dependent on that
        StopMovementTemporarily(timeBetweenNextAttack[comboCount - 1], true);

        yield return null;
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

        particleVisual.SetActive(false);

        comboCount = 0;
        leftWeapon.SheathWeapon();
        rightWeapon.SheathWeapon();

        comboCooldownCoroutine = null;
    }

    /// <summary>
    /// countdown for player dash combo & dash functionality
    /// </summary>
    private IEnumerator Dash()
    {
        PlayAnim("Dash", true);

        float dashTotalTime = 0;

        // dash refractory period begin
        canDash = false;
        dashes++;

        //movementControl.action.Disable();
        Vector3 moveDirection = transform.TransformDirection(Vector3.forward);

        while (dashTotalTime <= dashTime)
        {
            controller.Move(moveDirection * dashSpeed * Time.deltaTime);
            dashTotalTime += Time.deltaTime;
            yield return null;
        }

        //movementControl.action.Enable();

        yield return new WaitForSeconds(dashAgainCooldown);
        // dash refractory period end
        canDash = true;
    }

    public void ShiftAbility()
    {
        PlayAnim("ShiftAbility", true);
    }

    public void RAbility()
    {
        PlayAnim("RAbility", true);
    }

    /// <summary>
    /// unleashes a ranged attack
    /// </summary>
    public void RangedAttack()
    {
        // play the visual
        particleVisual.SetActive(false);
        particleVisual.SetActive(true);

        // instantiate the collision particle
        GameObject particle = Instantiate(particleCollisionPrefab, transform.position + transform.forward, transform.rotation);
        particle.GetComponent<ParticleCollision>().InitializeParticle(this, thirdHitDamage, particleVisual);
    }
    #endregion

    #region<SFX Functions>
    public void PlayFootSteps()
    {
        if (footsteps.Count > 0)
        {
            int num = UnityEngine.Random.Range(0, footsteps.Count);
            //AudioManager.instance.Play("Footstep" + footsteps[num].ToString());
        }
    }

    public void PlayAttack()
    {
        if (attacks.Count > 0)
        {
            int num = UnityEngine.Random.Range(0, attacks.Count);
            //AudioManager.instance.Play("Attack" + attacks[num].ToString());
        }
    }

    public void PlayImpact()

    {
        if (impacts.Count > 0)
        {
            int num = UnityEngine.Random.Range(0, impacts.Count);
            //AudioManager.instance.Play("Impact" + attacks[num].ToString());
        }
    }
    #endregion

    #region<Animation Functions>
    /// <summary>
    /// called when we want to swap to a new animation
    /// </summary>
    /// <param name="clip">the new clip that we are going to be swapping in</param>
    /// <param name="utilExit">does the animation loop. this is defaulted to false (it does loop)</param>
    public void AnimUtil(AnimationClip clip, bool utilExit = false)
    {
        animOverride[utilClip] = clip;
        anim.SetBool("UtilStop", false);
        anim.SetBool("UtilExit", utilExit);
        anim.SetTrigger("Util");
    }

    /// <summary>
    /// updates the blend tree based on player movement
    /// </summary>
    private void UpdateBlendTreeMoveSpeed()
    {
        // blend from idle to run
        if (controller.velocity.magnitude > 0)
        {
            anim.SetFloat("MoveSpeed", 1, blendTreeSmoothTime, Time.deltaTime * 2f);
        }
        // blend from run to idle
        else
        {
            anim.SetFloat("MoveSpeed", 0, blendTreeSmoothTime, Time.deltaTime * 2f);
        }
    }

    /// <summary>
    /// a helper function to make it easier to play specific animations by name
    /// </summary>
    /// <param name="name">the name of the animation to play</param>
    /// <param name="dontLoop">whether to loop the animation or not</param>
    public void PlayAnim(string name, bool dontLoop = false)
    {
        AnimationClip clip = Array.Find(anims, anim => anim.name == name);
        if (clip == null)
        {
            Debug.LogWarning("Animation Clip: " + name + " not found!");
            return;
        }

        AnimUtil(clip, dontLoop);
    }

    /// <summary>
    /// starts the StopMovementTemporarily coroutine
    /// </summary>
    /// <param name="noFadeDelay">the amount of time to temporarily slow the movement if fade is false</param>
    /// <param name="fade">if true, it will smooth into the stop/start. if false, it will have no exit time and immediately start/stop</param>
    public void StopMovementTemporarily(float noFadeDelay, bool fade)
    {
        StartCoroutine(StopMovementTemporarilyCo(noFadeDelay, fade));
    }

    /// <summary>
    /// stop/slow movement for a certain amount of time
    /// </summary>
    /// <param name="noFadeDelay">the amount of time to temporarily slow the movement if fade is false</param>
    /// <param name="fade">if true, it will smooth into the stop/start. if false, it will have no exit time and immediately start/stop</param>
    IEnumerator StopMovementTemporarilyCo(float noFadeDelay, bool fade)
    {
        // initialize timer duration
        float timeElapsed = 0;
        float currentPlayerSpeed = playerSpeed;

        if(comboCount == 3)
        {
            attackSmoothingIn *= 2;
        }

        if (fade)
        {
            // slow the player down to a temporary stop
            while (timeElapsed < attackSmoothingIn)
            {
                playerSpeed = Mathf.Lerp(currentPlayerSpeed, slowPlayerTo, timeElapsed / attackSmoothingIn);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        // instantly slow player
        else
        {
            playerSpeed = slowPlayerTo;
        }

        // if there is no fade, have a noticeable pause to feel the affect of the stopping/slowing
        if(!fade) yield return new WaitForSeconds(noFadeDelay);

        // reset timer duration
        timeElapsed = 0;
        currentPlayerSpeed = playerSpeed;

        if (fade)
        {
            // slow the player down to a temporary stop
            while (timeElapsed < attackSmoothingOut)
            {
                playerSpeed = Mathf.Lerp(currentPlayerSpeed, OGplayerSpeed, timeElapsed / attackSmoothingOut);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        // instantly accerlate player to starter speed
        else
        {
            playerSpeed = OGplayerSpeed;
        }

        if (comboCount == 3)
        {
            attackSmoothingIn /= 2;
        }
        float offset = attackSmoothingIn + attackSmoothingOut;
        yield return new WaitForSeconds(noFadeDelay - offset);

        // toggle off the collider of the weapon
        isAttacking = false;
        leftWeapon.ToggleCollider(isAttacking);
        rightWeapon.ToggleCollider(isAttacking);
    }
    #endregion

    #region<MISC>
    /// <summary>
    /// called in the animation event of "GroundedAttack3" to stop and resume player speed for more impact
    /// </summary>
    /// <param name="newSpeed">the speed to slow the player to</param>
    /// <param name="backToNormal">whether the player should be slowed or resume to their normal speed</param>
    public void ThirdHitHelper(float newSpeed, bool backToNormal)
    {
        if (!backToNormal)
        {
            playerSpeed = newSpeed;
        }
        else playerSpeed = OGplayerSpeed;
    }

    /// <summary>
    /// turn on ability
    /// </summary>
    public void SetThirdHit()
    {
        thirdHit = true;
    }
    #endregion

    public void Die()
    {
        Debug.Log("player should die");
        PlayAnim("Die", true);
        Sequence mySequence = DOTween.Sequence();

        deathPanel.gameObject.SetActive(true);

        TextMeshProUGUI deathText = deathPanel.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log("DEAHT PANEL: ", deathText);

        mySequence.Append(deathPanel.DOFade(1f, 0.5f));
        mySequence.Append(deathText.DOFade(1f, 0.5f));

        mySequence.AppendInterval(1.5f).OnComplete(() => Respawn());
    }

    private void Respawn()
    {
        Debug.Log("RESPAWN TIME!");
        GetComponent<CharacterController>().enabled = false;
        gameObject.transform.position = gameState.spawnPoint;
        GetComponent<CharacterController>().enabled = true;
        gameState.playerHealth = gameState.maxPlayerHealth;
        //TODO: reset death panel and text alpha to 0
        deathPanel.color = new Color(0,0,0,0);
        TextMeshProUGUI deathText = deathPanel.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        deathText.color = new Color(1, 1, 1, 0); 
        deathPanel.gameObject.SetActive(false);
    }
}
