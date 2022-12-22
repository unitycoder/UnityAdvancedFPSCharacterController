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
    [SerializeField] public PlayerMoveData moveData;
    [SerializeField] private PlayerManager playerManager;
    #endregion

    public bool allowMove = true;
    
    [Header("Speed")]
    public float speed = 0f;
    public Vector3 velocity = Vector3.zero;
    public Vector3 moveSmoothen = Vector3.zero;
    public float x = 0f, y = 0f;
    public bool isMoving = false;
    public bool isWalking = false;
    public bool isSprinting = false;
    // A method where we send booleans to define how fast the controller should move
    public float desiredSpeed(bool condition)
    {
        if (condition = isCrouching) return speed = moveData.crouchSpeed;
        else if (condition = isSprinting) return speed = moveData.sprintSpeed;
        else return speed = moveData.walkSpeed;
    }
    public float _groundSmoothen;
    // A method where PlayerMovement send booleans to define how smooth or slipperly the floor is
    public float groundSmoothen(bool condition)
    {
        if (condition = isCrouching) return _groundSmoothen = moveData.crouchDamp;
        else if (condition = isSprinting) return _groundSmoothen = moveData.sprintDamp;
        else return _groundSmoothen = moveData.walkDamp;
    }

    [Header("Crouching")]
    public bool attemptingCrouch = false;
    public bool isCrouching = false;
    public bool toggleCrouch;

    [Header("Jumping")]
    public bool useGravity = true;
    public bool attemptingJump = false;
    
    [Header("Ground Detection")]
    public bool Grounded = false;
    public bool ccGrounded = false;
    public bool Fell = false;
    public float CoyoteTime;
    /**
    Only true if CoyoteTime has not exceeded the limit. Make sure CoyoteTimeMax only has a small value
    because this isn't for double-jumping.
    */
    public bool coyoteGrounded()
    {
        return CoyoteTime < moveData.CoyoteTimeMax;
    }
    /**
    Does the same for coyoteGrounded() but it should take a little longer.
    */
    public bool isFalling()
    {
        return CoyoteTime > moveData.FallTimeMax;
    }
    public float fallSpeed = 0f;

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
        if (!playerInputInit) return;
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
    private void Update() // Self explanatory, come on man
    {
        Inputs();
        ApplyGravity();
        GroundChecker();
        ChangeSpeed();
        Move();
    }
    #endregion

    #region Inputs
    Vector2 moveInput;
    private void Inputs() // Keyboard / WASD inputs
    {
        attemptingJump = PlayerInput.Instance.jump;
        x = PlayerInput.Instance.move.x;
        y = PlayerInput.Instance.move.y;
        moveInput = new Vector2(x, y);
        if (moveInput.sqrMagnitude > 1)
        {
            moveInput.Normalize();
        }
    }
    #endregion

    #region Gravity and Detection Logic
    /**
    Built-in CharacterController component is not a Rigidbody! Define gravity by yourself using this method.
    */
    private void ApplyGravity()
    {
        velocity.y -= Physics.gravity.y * -2f * Time.deltaTime;
        
        playerManager.controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);

        /*
        fallSpeed defines how much  * o o m p h *  the camera headbobbing should have upon landing. It should only be
        tracked when mid-air for bug-fixing. (It doesn't work well when it tracks even when grounded)
        */
		if (!playerManager.controller.isGrounded) fallSpeed = playerManager.controller.velocity.y;
    }

    private void GroundChecker()
    {
        /**
        These two are for debugging only, because controller.isGrounded and coyoteGrounded() does not appear
        in inspector.
        */
        ccGrounded = playerManager.controller.isGrounded;
        Grounded = coyoteGrounded();

        // Extra sprinting logic, bug-fixing and etc
        QueueSprint = coyoteGrounded() && isCrouching && isMoving;
        if (isSprinting && isCrouching)
        {
            isSprinting = false;
        }

        // Extra crouching logic, bug-fixing and etc
        QueueCrouch = playerManager.controller.isGrounded;
        if (isCrouching && !coyoteGrounded())
        {
            StopCrouch();
        }

        // Manage coyote time. Coyote time is the extra time that the controller has to be able to jump in mid-air.
        if (playerManager.controller.isGrounded)
        {
            CoyoteTime = 0;
            if (Fell) Invoke(nameof(ResetFall), Time.fixedDeltaTime);
        }
        else
        {
            CoyoteTime += Time.deltaTime;
        }

        /**
        It was written intentionally like this way instead of Fell = isFalling() because you
        need to delay along the Invoke for ResetFall() above.
        */
        if (isFalling())
        {
            Fell = true;
        }
    }

    private void ResetFall() // Again, you need the delay.
    {
        Fell = false;
    }
    #endregion
    
    #region Movement Logic
    private void ChangeSpeed()
    {
        var speedCondition = isCrouching && isSprinting;

        // Check PlayerMoveData for these two.
        groundSmoothen(speedCondition);
        desiredSpeed(speedCondition);

        isWalking = !isSprinting && !isCrouching;
    }

    Vector3 moveDir;

    #region Slope Movement
    private RaycastHit slopeHit;
    private bool OnSlope() // Checks using a Raycast below the controller to see if it's stepping on a slope (duh)
    {
        if (!coyoteGrounded()) return false;

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, (playerManager.controller.height / 2) + moveData.groundDistance))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            if (slopeAngle > playerManager.controller.slopeLimit) return true;
        }

        return false;
    }
    private void SlopeMovement() // Overrides the moveDir Vector3
    {
        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        float slideSpeed = speed + moveData.slopeSlideSpeed + Time.deltaTime;

        moveDir = slopeDirection * -slideSpeed;
        moveDir.y = moveDir.y - slopeHit.point.y;
    }
    #endregion
    private void Move()
    {
        // Input Vector3 multiplied with the character's transform directions.
        moveDir = (playerManager.orientation.forward * moveInput.y + playerManager.orientation.right * moveInput.x).normalized;

        // Checks if controller is moving, this makes sure that sprinting is bug-free
        isMoving = (new Vector3(velocity.x, 0f, velocity.z) != Vector3.zero);

        if (OnSlope())
        {
            SlopeMovement();
        }

        // antiBump makes sure that slope movement is not bumpy and that the controller stays in place on the y-axis when grounded.
        if (velocity.y < 0 && playerManager.controller.isGrounded) velocity.y = -moveData.antiBump;
        // Probably makes it so the controller can't do some quasi-vaulting, might remove later because I barely notice any difference
        playerManager.controller.stepOffset = coyoteGrounded() ? 0.3f : 0f;
        // moveSmoothen is responsible for creating some sort of inertia and overall makes movement smoother.
        moveSmoothen = Vector3.MoveTowards(moveSmoothen, moveDir, (playerManager.controller.isGrounded ? _groundSmoothen : moveData.airSmoothen) * Time.deltaTime);
        
        playerManager.controller.Move(moveSmoothen * speed * Time.deltaTime);

        // Will be useful for PlayerAudio, remove if you do not need PlayerAudio.
        velocity.x = playerManager.controller.velocity.x;
        velocity.z = playerManager.controller.velocity.z;
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
        get { return coyoteGrounded() && isCrouching && isMoving; }
        set
        {
            var sprintCondition = (coyoteGrounded() && !isCrouching && isMoving);
            if (isSprinting) return;
            value = sprintCondition;
            if (value == sprintCondition && queueSprint)
            {
                StartSprint();
            }
        }
    }

    // These two are responsible for the actual sprinting feature
    private void StartSprint()
    {
        if (coyoteGrounded() && !isCrouching && isMoving) isSprinting = true;
    }
    private void StopSprint()
    {
        isSprinting = false;
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
        if (!coyoteGrounded()) return;

        /**
        These three lines of code calls for the playerAudio script to play jump sound effects.
        Remove it if you do not need it.
        */
        playerManager.playerAudio.source.pitch = 1;
        playerManager.playerAudio.source.clip = playerManager.playerAudio.jumpSound;
        playerManager.playerAudio.source.PlayOneShot(playerManager.playerAudio.source.clip);

        velocity.y = Mathf.Sqrt(moveData.jumpForce * -3.0f * Physics.gravity.y);
    }
    #endregion

    #region  Crouch Logic
    // These two handles the input for crouching, they're separated from the actual crouching code to make queing possible.
    private void CrouchInput(InputAction.CallbackContext ctx)
    {
        switch(toggleCrouch)
        {
            case true:
                queueCrouch = !queueCrouch;
            break;
            case false:
                queueCrouch = true;
            break;
        }
        StartCrouch();
    }
    private void StandInput(InputAction.CallbackContext ctx)
    {
        if(toggleCrouch) return;
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
        get { return coyoteGrounded(); }
        set
        {
            if (isCrouching) return;
            value = coyoteGrounded();
            if (value == true && queueCrouch)
            {
                StartCrouch();
            }
        }
    }

    // These two are responsible for the actual crouching feature
    Coroutine crouchRoutine;
    private void StartCrouch()
    {
        if (!coyoteGrounded()) return; // Only crouch when grounded
        switch(toggleCrouch)
        {
            case true:
                isCrouching = !isCrouching;
            break;
            case false:
                isCrouching = true;
            break;
        }
        if (crouchRoutine != null) StopCoroutine(crouchRoutine); // Bug-fix for the forever looping AdjustHeight() Coroutine bug.
        
        // Picks between crouchHeight and standHeight because StopCrouch() only works when toggleCrouch is disabled.
        crouchRoutine = StartCoroutine(AdjustHeight(isCrouching ? moveData.crouchHeight : moveData.standHeight));
    }
    private void StopCrouch()
    {
        isCrouching = false;
        if (crouchRoutine != null) StopCoroutine(crouchRoutine); // Bug-fix for the forever looping AdjustHeight() Coroutine bug.
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