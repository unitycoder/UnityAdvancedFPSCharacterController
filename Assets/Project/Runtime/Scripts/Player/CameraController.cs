using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    [Header("References")]
    [SerializeField] private CameraManager cameraManager;

    /*
    Put the parent camera object here.
    */
    [SerializeField] private Transform camHolder;
    
    [Header("Camera")]
    /*
    Check if camera is aiming at Up/Down clamp, good for bug-fixing
    (You do not want weapon swaying to occur when you can't even move your cam further up or down, something like that)
    */
    [SerializeField] public bool Clamping;
    [SerializeField] float UpClamp, DownClamp; // Limits the X axis rotation of the camera

    // Don't touch, I don't know either it just works (probably for more precise camera movement)
    [SerializeField] float multiplier = 0.01f;
    [SerializeField] float xRotation, yRotation; // Both of these are the raw output of the mouse input

    /*
    the first two tries to follow xRotation and yRotation using Mathf.Lerp, this makes cinematic smoothing
    possible by lowering the value of damp.
    */
    [SerializeField] float x, y, damp; 

    private void Update()
    {
        // This one does the actual clamping of the X axis rotation of the camera
        xRotation = cameraManager.clampCam ? Mathf.Clamp(xRotation, UpClamp, DownClamp) : xRotation;

        /*
        Assuming that the PlayerManager component is in the parent/root of the player gameObject, then rotate the said gameObject.
        You should only rotate the Player gameObject's Y axis.
        There should be a separate implementation for visually aiming vertically.
        */
        cameraManager.playerManager.transform.localEulerAngles = new Vector3(0, y, 0);

        // Rotate both the camera's X and Y axis, for Z axis check CameraManager
        camHolder.transform.localEulerAngles = new Vector3(x, y, !cameraManager.reduceMotion ? cameraManager.targetMoveTilt + cameraManager.footstepShakeRot.z : 0f);

        Look();
    }

    private void Look()
    {
        // Does the actual checking whether camera is aiming right at the Up/Down clamps.
        Clamping = ((xRotation == UpClamp) || (xRotation == DownClamp));

        // Camera smoothing, 10-20 makes it unnoticeable
        x = Mathf.Lerp(x, xRotation, damp * 10f * Time.deltaTime);
        y = Mathf.Lerp(y, yRotation, damp * 10f * Time.deltaTime);

        /*
        This is important, you should flip y and x to each other and vice versa

        Mouse input is simply a Vector2, Y is when you aim your mouse up and down and X is when
        you aim your mouse left and right.
        But for rotation it is different, the X rotation of the camera makes it look/rotate up and down
        and Y rotation of the camera makes it look/rotate left and right.

        So Mouse.Y = Rotate camera's X axis and Mouse.X = Rotate camera's Y axis.

        Hope this makes sense.
        */
        yRotation += PlayerInput.Instance.look.x * cameraManager.sensX * multiplier;

        switch(cameraManager.invertYAxis) // Invert Y axis for weirdos
        {
            case true:
                xRotation += PlayerInput.Instance.look.y * cameraManager.sensY * multiplier;
            break;
            case false:
                xRotation -= PlayerInput.Instance.look.y * cameraManager.sensY * multiplier;
            break;
        }
    }
}