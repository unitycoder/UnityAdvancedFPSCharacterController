using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Although worded the same way as PlayerManager, it functions differently.
/// Instead of being just a "parent" class, it decides how the camera behaves.
/// Movement Headbobbing is implemented and modified from - https://www.youtube.com/watch?v=5MbR2qJK8Tc&ab_channel=Hero3D
/// </summary>
public class CameraManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] public PlayerManager playerManager;
    [SerializeField] public CameraController cameraController;

    [Header("Values")]
    
    /// <summary>
    /// For cinematics.
    /// </summary>
    [SerializeField] public bool clampCam, moveCamera;
    /// <summary>
    /// For weirdos.
    /// </summary>
    [SerializeField] public bool invertYAxis;
    /// <summary>
    /// Disables headbobbing and tilting.
    /// </summary>
    [SerializeField] public bool reduceMotion;
    /// <summary>
    /// Mouse sensitivity.
    /// </summary>
    [SerializeField] public float sensX, sensY;
    
    // There are two headbobbing features here, one is for movement, and one is when the controller lands from a fall.
    [Header("Headbobbing")]
    /// <summary>
    /// Bob smoothing 1
    /// </summary>
	[SerializeField] private float landBobSpeed = 15f;
    /// <summary>
    /// Bob smoothing 2
    /// </summary>
	[SerializeField] private float landBobMultiplier = 1f;
	[SerializeField] public Vector3 desyncOffset, desiredLandBob, landBobOffset, moveBobPos, moveBobRot;
    /// <summary>
    /// Checks if you want headbobbing for specific axis.
    /// </summary>
    [SerializeField] private bool moveBobX, moveBobY;
    /// <summary>
    /// How strong the headbobbing is.
    /// </summary>
    [SerializeField, Range(0, 2f)] private float amplitude;
    /// <summary>
    /// How frequent the headbobbing is.
    /// </summary>
    [SerializeField, Range(0, 30f)] private float frequency;
    /// <summary>
    /// How frequent the headbobbing is depending on player's movement state.
    /// </summary>
    [SerializeField] public float walkFreq, sprintFreq, crouchFreq;

    /// <summary>
    /// A method where we send booleans to define how frequent the camera bobs side-to-side.
    /// </summary>
    [SerializeField] float desiredFreq(bool condition)
    {
        if(condition = playerManager.playerMovement.moveData.isCrouching) return frequency = crouchFreq;
        else if(condition = playerManager.playerMovement.moveData.isSprinting) return frequency = sprintFreq;
        else return frequency = walkFreq;
    }

    /// <summary>
    /// How fast the controller should be for the headbobbing to occur.
    /// </summary>
    [SerializeField] private float toggleSpeed = 3.0f;
    private Vector3 startPos;
    private Vector3 startRot;
    /// <summary>
    /// How fast the camera goes back to original pos and rot.
    /// </summary>
    [SerializeField] public float moveBobReturnSpeed;

    /// [Header("Field of View")]
    /// public float walkFov, sprintFov, crouchFov;

    [Header("Tilts")] // Gives that Half-life/Quake side tilts when strafing.
    [SerializeField] public float moveTilt, tiltMultiplier;
    [SerializeField] public float targetMoveTilt { get; set; }

    private void Start()
    {
        startPos = Vector3.zero;
        startRot = Vector3.zero;
    }

    private void Update()
    {
        // Tells how or where the head that the camera follows should move to.
        playerManager.head.transform.localPosition = new Vector3(playerManager.cameraManager.moveCamera? 0 : playerManager.head.transform.localPosition.x,
        playerManager.cameraManager.moveCamera? 0 + playerManager.controller.height: playerManager.head.transform.localPosition.y,
        playerManager.cameraManager.moveCamera? 0 : playerManager.head.transform.localPosition.z);
    }

    private void LateUpdate()
    {
        // Tells how or where the camera should move to.
        transform.localPosition = moveCamera ? playerManager.head.transform.position + desyncOffset + (!reduceMotion ? moveBobPos + landBobOffset : Vector3.zero) : transform.localPosition;
		desyncOffset = Vector3.Lerp(desyncOffset, Vector3.zero, Time.deltaTime * 15f);

        // Self-explanatory
        TiltCamera();
        Headbob();
    }

    #region Camera Tilting
    private void TiltCamera()
    {
        if(playerManager.playerMovement.moveData.isCrouching) // Dont tilt when crouching
        {
            targetMoveTilt = Mathf.Lerp(targetMoveTilt, 0f, tiltMultiplier * Time.deltaTime);
            return;
        }

        float tilt;

        /**
        Check for X input, do not use == 1 and == -1 because it will only work for keyboard input.
        Use > 0 and < 0 because this will help future-proof it just in case you want cross-platform.
        */
        switch(playerManager.playerMovement.moveData.x)
        {
            case < 0f:
                tilt = moveTilt;
            break;
            case > 0f:
                tilt = -moveTilt;
            break;
            default:
                tilt = 0f;
            break;
        }

        // Finally do the math. See CameraController line 50
        targetMoveTilt = Mathf.Lerp(targetMoveTilt, playerManager.playerMovement.moveData.z != 0 ? tilt / 2 : tilt, tiltMultiplier * Time.deltaTime);
    }
    #endregion

    #region Headbob Logic
    private void Headbob()
    {
        if(reduceMotion) // Not an epic gamer? Reset the position and rotation of camera to clear the effects of headbobbing.
        {
            desiredLandBob = desiredLandBob == Vector3.zero ? desiredLandBob : Vector3.zero;
            landBobOffset = landBobOffset == Vector3.zero ? landBobOffset : Vector3.zero;
            moveBobPos = moveBobPos == Vector3.zero ? moveBobPos : Vector3.zero;
            moveBobRot = moveBobRot == Vector3.zero ? moveBobRot : Vector3.zero;
            return;
        }

        // This relies on the frame-dependent delay we have put in GroundChecker() at PlayerMovement.
        if(playerManager.playerMovement.moveData.Fell && playerManager.playerMovement.moveData.coyoteGrounded())
        {
			BobOnce(new Vector3(0f, playerManager.playerMovement.moveData.fallSpeed, 0f));
        }

        var bobCondition = playerManager.playerMovement.moveData.isCrouching && playerManager.playerMovement.moveData.isSprinting;

        desiredFreq(bobCondition); // See 31 - 36

		desiredLandBob = Vector3.Lerp(desiredLandBob, Vector3.zero, Time.deltaTime * landBobSpeed * 0.5f);
		landBobOffset = Vector3.Lerp(landBobOffset, desiredLandBob, Time.deltaTime * landBobSpeed);

        CheckMotion();
    }

    public void BobOnce(Vector3 bobDirection) // Makes landing from mid-air feel better.
	{
		Vector3 bob = ClampVector(bobDirection * 0.15f, -3f, 3f);
		desiredLandBob = bob * landBobMultiplier;
	}

	private Vector3 ClampVector(Vector3 vec, float min, float max) // Makes sure the headbobbing doesn't go nuts.
	{
		return new Vector3(Mathf.Clamp(vec.x, min, max), Mathf.Clamp(vec.y, min, max), Mathf.Clamp(vec.z, min, max));
	}

    private void CheckMotion() // Calculates the player's controller velocity.
    {
        float speed = new Vector3(playerManager.controller.velocity.x, 0, playerManager.controller.velocity.z).magnitude;

        // Reset the Z axis headbob/rotation when not sprinting and not grounded.
        if(!playerManager.playerMovement.moveData.isSprinting || !playerManager.playerMovement.moveData.coyoteGrounded()) ResetRotation();

        if(speed < toggleSpeed) // Not enough speed? Reset the headbobbing.
        {
            ResetPosition();
            return;
        }
        if(!playerManager.playerMovement.moveData.coyoteGrounded()) return;

        PlayMotion(FootstepMotion(), FootstepRotation()); // Do the headbobbing if it passes the criteria.
    }

    private void ResetPosition() // Smoothly reset the position.
    {
        if(moveBobPos == startPos) return;
        moveBobPos = Vector3.Lerp(moveBobPos, startPos, moveBobReturnSpeed * Time.deltaTime);
    }

    private void ResetRotation() // Smoothly reset the rotation.
    {
        if(moveBobRot == startRot) return;
        moveBobRot = Vector3.Lerp(moveBobRot, startRot, moveBobReturnSpeed * Time.deltaTime);
    }

    // Manipulates the values of moveBobPos and moveBobRot
    private void PlayMotion(Vector3 motion, Vector3 rotation)
    {
        moveBobPos += motion; // See line 67
        if(!playerManager.playerMovement.moveData.isSprinting) return;
        moveBobRot += rotation; // See CameraController line 44
    }

    // These two uses advanced gamer mathematics to calculate for satisfying headbob movement.
    private Vector3 FootstepMotion()
    {
        Vector3 pos = Vector3.zero;
        if(moveBobY) pos.y += (Mathf.Sin(Time.time * frequency) * amplitude) * Time.deltaTime;
        if(moveBobX) pos.x += (Mathf.Cos(Time.time * frequency * 0.5f) * amplitude / 1.75f) * Time.deltaTime * playerManager.transform.right.x;
        if(moveBobX) pos.z += (Mathf.Cos(Time.time * frequency * 0.5f) * amplitude / 1.75f) * Time.deltaTime * playerManager.transform.right.z;
        return pos;
    }
    private Vector3 FootstepRotation()
    {
        Vector3 rot = Vector3.zero;
        if(moveBobX) rot.z += (Mathf.Cos(Time.time * frequency * 0.5f) * amplitude * 5) * Time.deltaTime;
        return rot;
    }
    #endregion
}