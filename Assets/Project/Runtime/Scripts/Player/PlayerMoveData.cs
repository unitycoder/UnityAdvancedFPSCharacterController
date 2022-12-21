using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

/// <summary>
/// Sub-class of PlayerMovement, configure the default values here if you don't have a prefab yet.
/// </summary>
[CreateAssetMenu(fileName = "PlayerMoveData", menuName = "Advanced FPS Controller/PlayerMoveData", order = 0)]
public class PlayerMoveData : ScriptableObject 
{   
    public bool allowMove = true;
    
    [Header("Speed")]
    public float x = 0f, z = 0f;
    public bool isMoving = false;
    public bool isWalking = false;
    public bool isSprinting = false;
    public float speed = 0f;

    // A method where PlayerMovement send booleans to define how fast the controller should move
    public float desiredSpeed(bool condition)
    {
        if(condition = isCrouching) return speed = Mathf.Lerp(speed, crouchSpeed, acceleration * Time.deltaTime);
        else if(condition = isSprinting) return speed = Mathf.Lerp(speed, sprintSpeed, acceleration * Time.deltaTime);
        else return speed = Mathf.Lerp(speed, walkSpeed, acceleration * Time.deltaTime);
    }
    public float acceleration = 10f;
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float walkDamp = 4.5f;
    public float sprintDamp = 3.95f;
    public float crouchDamp = 20f;
    public float _groundSmoothen;

    // A method where PlayerMovement send booleans to define how smooth or slipperly the floor is
    public float groundSmoothen(bool condition)
    {
        if(condition = isCrouching) return _groundSmoothen = crouchDamp;
        else if(condition = isSprinting) return _groundSmoothen = sprintDamp;
        else return _groundSmoothen = walkDamp;
    }
    public float airSmoothen = 2.25f;
    public Vector3 velocity = Vector3.zero;
    public Vector3 moveSmoothen = Vector3.zero;

    [Header("Crouching")]
    [Range(0, 20.0f)]
    public float heightLerp = 10f;
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public bool attemptingCrouch = false;
    public bool isCrouching = false;
    public bool toggleCrouch;

    [Header("Jumping")]
    public bool attemptingJump = false;
    public float jumpForce = 2.5f;

    [Header("Ground Detection")]
    public LayerMask groundMask;
    public float CoyoteTime;
    public float CoyoteTimeMax = 0.1f;
    public float FallTimeMax = 0.25f;
    public bool Grounded = false;
    public bool ccGrounded = false;
    public bool Fell = false;

    /**
    Only true if CoyoteTime has not exceeded the limit. Make sure CoyoteTimeMax only has a small value
    because this isn't for double-jumping.
    */
    public bool coyoteGrounded()
    {
        return CoyoteTime < CoyoteTimeMax;
    }

    /**
    Does the same for coyoteGrounded() but it should take a little longer.
    */
    public bool isFalling()
    {
        return CoyoteTime > FallTimeMax;
    }
    public float fallSpeed = 0f;
    public float groundDistance = 1f;
    public float slopeSlideSpeed = 1f;
    public float antiBump = 5f;
}
