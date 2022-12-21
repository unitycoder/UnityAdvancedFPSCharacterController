using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This script handles walking, sprinting, jumping, and crouching. It also has coyote time and good slope movement.
/// Slope Movement has been implemented and modified from - https://www.youtube.com/watch?v=GI5LAbP5slE&ab_channel=Hero3D
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    #region Components
    [Header("References")]
    [SerializeField] private PlayerMoveData playerMoveData;
    public PlayerMoveData moveData { get { return playerMoveData; } }
    [SerializeField] private PlayerManager playerManager;
    #endregion

    #region Initialization and Deactivation Logic
    bool playerInputInit = false;
    // Edge-triggered inputs, these are the stuff you definitely don't want to run every frame.
    private IEnumerator Start() // Initialize edge-triggered inputs only after PlayerInput's Instance is defined.
    {
        yield return new WaitForEndOfFrame();
        PlayerInput.Instance.jumpAction.performed += JumpInput;
        PlayerInput.Instance.sprintAction.performed += SprintInput;
        PlayerInput.Instance.sprintAction.canceled += WalkInput;
        PlayerInput.Instance.crouchAction.performed += CrouchInput;
        PlayerInput.Instance.crouchAction.canceled += StandInput;
        playerInputInit = true;
    }
    
    private void OnEnable() // Enable edge-triggered inputs.
    {
        if(!playerInputInit) return;
        Debug.Log("cockadoodleoooo");
        PlayerInput.Instance.jumpAction.performed += JumpInput;
        PlayerInput.Instance.sprintAction.performed += SprintInput;
        PlayerInput.Instance.sprintAction.canceled += WalkInput;
        PlayerInput.Instance.crouchAction.performed += CrouchInput;
        PlayerInput.Instance.crouchAction.canceled += StandInput;
    }

    private void OnDisable() // Disable edge-triggered inputs.
    {
        PlayerInput.Instance.jumpAction.performed -= JumpInput;
        PlayerInput.Instance.sprintAction.performed -= SprintInput;
        PlayerInput.Instance.sprintAction.canceled -= WalkInput;
        PlayerInput.Instance.crouchAction.performed -= CrouchInput;
        PlayerInput.Instance.crouchAction.canceled -= StandInput;
    }
    #endregion

    #region Update Methods
    void Update() // Self explanatory, come on man
    {
        Inputs();
        ApplyGravity();
        GroundChecker();
        ChangeSpeed();
        Move();
    }
    #endregion

    #region Inputs
    Vector3 moveInput;
    private void Inputs() // Keyboard / WASD inputs
    {
        moveData.x = PlayerInput.Instance.move.x;
        moveData.z = PlayerInput.Instance.move.y; // WASD is 2D, but it's important to pair it with Vector3.z
        moveInput = new Vector3(moveData.x, 0f, moveData.z);
    }
    #endregion

    #region Gravity and Detection Logic
    /**
    Built-in CharacterController component is not a Rigidbody! Define gravity by yourself using this method.
    */
    private void ApplyGravity()
    {
        moveData.velocity.y -= Physics.gravity.y * -2f * Time.deltaTime;
        
        playerManager.controller.Move(new Vector3(0f, moveData.velocity.y, 0f) * Time.deltaTime);

        /*
        fallSpeed defines how much  * o o m p h *  the camera headbobbing should have upon landing. It should only be
        tracked when mid-air for bug-fixing. (It doesn't work well when it tracks even when grounded)
        */
		if(!playerManager.controller.isGrounded) moveData.fallSpeed = playerManager.controller.velocity.y;
    }

    private void GroundChecker()
    {
        /**
        These two are for debugging only, because controller.isGrounded and coyoteGrounded() does not appear
        in inspector.
        */
        moveData.ccGrounded = playerManager.controller.isGrounded;
        moveData.Grounded = moveData.coyoteGrounded();

        // Extra sprinting logic, bug-fixing and etc
        QueueSprint = moveData.coyoteGrounded() && moveData.isCrouching && moveData.isMoving;
        if(moveData.isSprinting && moveData.isCrouching)
        {
            moveData.isSprinting = false;
        }

        // Extra crouching logic, bug-fixing and etc
        QueueCrouch = playerManager.controller.isGrounded;
        if(moveData.isCrouching && !moveData.coyoteGrounded())
        {
            StopCrouch();
        }

        // Manage coyote time. Coyote time is the extra time that the controller has to be able to jump in mid-air.
        if (playerManager.controller.isGrounded)
        {
            moveData.CoyoteTime = 0;
            if(moveData.Fell) Invoke(nameof(ResetFall), Time.fixedDeltaTime);
        }
        else
        {
            moveData.CoyoteTime += Time.deltaTime;
        }

        /**
        It was written intentionally like this way instead of moveData.Fell = moveData.isFalling() because you
        need to delay along the Invoke for ResetFall() above.
        */
        if(moveData.isFalling())
        {
            moveData.Fell = true;
        }
    }

    private void ResetFall() // Again, you need the delay.
    {
        moveData.Fell = false;
    }
    #endregion
    
    #region Movement Logic
    private void ChangeSpeed()
    {
        var speedCondition = moveData.isCrouching && moveData.isSprinting;

        // Check PlayerMoveData for these two.
        moveData.groundSmoothen(speedCondition);
        moveData.desiredSpeed(speedCondition);

        moveData.isWalking = !moveData.isSprinting && !moveData.isCrouching;
    }

    Vector3 moveDir;

    #region Slope Movement
    private RaycastHit slopeHit;
    private bool OnSlope() // Checks using a Raycast below the controller to see if it's stepping on a slope (duh)
    {
        if(!moveData.coyoteGrounded()) return false;

        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, (playerManager.controller.height / 2) + moveData.groundDistance))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            if(slopeAngle > playerManager.controller.slopeLimit) return true;
        }

        return false;
    }
    private void SlopeMovement() // Overrides the moveDir Vector3
    {
        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        float slideSpeed = moveData.speed + moveData.slopeSlideSpeed + Time.deltaTime;

        moveDir = slopeDirection * -slideSpeed;
        moveDir.y = moveDir.y - slopeHit.point.y;
    }
    #endregion
    private void Move()
    {
        // Input Vector3 multiplied with the character's transform directions.
        moveDir = (playerManager.orientation.forward * moveInput.z + playerManager.orientation.right * moveInput.x).normalized;

        // Checks if controller is moving, this makes sure that sprinting is bug-free
        moveData.isMoving = (new Vector3(moveData.velocity.x, 0f, moveData.velocity.z) != Vector3.zero);

        if(OnSlope())
        {
            SlopeMovement();
        }

        // antiBump makes sure that slope movement is not bumpy and that the controller stays in place on the y-axis when grounded.
        if(moveData.velocity.y < 0 && playerManager.controller.isGrounded) moveData.velocity.y = -moveData.antiBump;
        // Probably makes it so the controller can't do some quasi-vaulting, might remove later because I barely notice any difference
        playerManager.controller.stepOffset = moveData.coyoteGrounded() ? 0.3f : 0f;
        // moveSmoothen is responsible for creating some sort of inertia and overall makes movement smoother.
        moveData.moveSmoothen = Vector3.MoveTowards(moveData.moveSmoothen, moveDir, (playerManager.controller.isGrounded ? moveData._groundSmoothen : moveData.airSmoothen) * Time.deltaTime);

        playerManager.controller.Move(moveData.moveSmoothen * moveData.speed * Time.deltaTime);

        // Will be useful for PlayerAudio, remove if you do not need PlayerAudio.
        moveData.velocity.x = playerManager.controller.velocity.x;
        moveData.velocity.z = playerManager.controller.velocity.z;
    }
    #endregion

    /**
        QUEUEING - A feature I implemented for sprinting and crouching because as far as I know event-based input (22-51)
        does not actually track whether you're pressing a key or not.
    */

    #region Sprint Logic
    // These two handles the input for sprinting, they're separated from the actual sprinting code to make queing possible.
    private void SprintInput(InputAction.CallbackContext ctx)
    {
        queueSprint = true;
        StartSprint();
    }
    private void WalkInput(InputAction.CallbackContext ctx)
    {
        queueSprint = false;
        StopSprint();
    }
    private bool queueSprint;
    /**
    Checks if the following conditions are met while SprintInput() is being called.
    (Check the GroundChecker() method.)
    */
    public bool QueueSprint
    {
        get { return moveData.coyoteGrounded() && moveData.isCrouching && moveData.isMoving; }
        set
        {
            var sprintCondition = (moveData.coyoteGrounded() && !moveData.isCrouching && moveData.isMoving);
            if(moveData.isSprinting) return;
            value = sprintCondition;
            if(value == sprintCondition && queueSprint)
            {
                StartSprint();
            }
        }
    }

    // These two are responsible for the actual sprinting feature
    private void StartSprint()
    {
        if(moveData.coyoteGrounded() && !moveData.isCrouching && moveData.isMoving) moveData.isSprinting = true;
    }
    private void StopSprint()
    {
        moveData.isSprinting = false;
    }
    #endregion

    #region Jump Logic
    // Mmm yes... jump
    private void JumpInput(InputAction.CallbackContext ctx)
    {
        Jump();
    }
    private void Jump()
    {
        if(!moveData.coyoteGrounded()) return;

        /**
        These three lines of code calls for the playerAudio script to play jump sound effects.
        Remove it if you do not need it.
        */
        playerManager.playerAudio.source.pitch = 1;
        playerManager.playerAudio.source.clip = playerManager.playerAudio.jumpSound;
        playerManager.playerAudio.source.PlayOneShot(playerManager.playerAudio.source.clip);

        moveData.velocity.y = Mathf.Sqrt(moveData.jumpForce * -3.0f * Physics.gravity.y);
    }
    #endregion

    #region  Crouch Logic
    // These two handles the input for crouching, they're separated from the actual crouching code to make queing possible.
    private void CrouchInput(InputAction.CallbackContext ctx)
    {
        queueCrouch = true;
        StartCrouch();
    }
    private void StandInput(InputAction.CallbackContext ctx)
    {
        queueCrouch = false;
        StopCrouch();
    }
    private bool queueCrouch;
    /**
    Checks if the following conditions are met while CrouchInput() is being called.
    (Check the GroundChecker() method.)
    */
    public bool QueueCrouch
    {
        get { return moveData.coyoteGrounded(); }
        set
        {
            if(moveData.isCrouching) return;
            value = moveData.coyoteGrounded();
            if(value == true && queueCrouch)
            {
                StartCrouch();
            }
        }
    }

    // These two are responsible for the actual crouching feature
    Coroutine crouchRoutine;
    private void StartCrouch()
    {
        if(!moveData.coyoteGrounded()) return; // Only crouch when grounded
        switch(moveData.toggleCrouch)
        {
            case true:
                moveData.isCrouching = !moveData.isCrouching;
            break;
            case false:
                moveData.isCrouching = true;
            break;
        }
        if(crouchRoutine != null) StopCoroutine(crouchRoutine); // Bug-fix for the forever looping AdjustHeight() Coroutine bug.
        
        // Picks between crouchHeight and standHeight because StopCrouch() only works when toggleCrouch is disabled.
        crouchRoutine = StartCoroutine(AdjustHeight(moveData.isCrouching ? moveData.crouchHeight : moveData.standHeight));
    }
    private void StopCrouch()
    {
        if(moveData.toggleCrouch) return;
        moveData.isCrouching = false;
        if(crouchRoutine != null) StopCoroutine(crouchRoutine); // Bug-fix for the forever looping AdjustHeight() Coroutine bug.
        crouchRoutine = StartCoroutine(AdjustHeight(moveData.standHeight));
    }
    private IEnumerator AdjustHeight(float height) // Shortens and elongates the collider depending on the provided float value.
    {
        while(playerManager.controller.height != height)
        {
            playerManager.controller.height = Mathf.Lerp(playerManager.controller.height, height, moveData.heightLerp * Time.deltaTime);
            playerManager.controller.center = Vector3.Lerp(playerManager.controller.center, new Vector3(0, height * 0.5f, 0), moveData.heightLerp * Time.deltaTime);
            playerManager.orientation.transform.localPosition = playerManager.controller.center;
            yield return null;
        }
    }
    #endregion
}