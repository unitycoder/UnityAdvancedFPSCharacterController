using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    [Header("References")]
    [SerializeField] private CameraManager cameraManager;

    /// <summary>
    /// The root/parent object that holds the cameras.
    /// </summary>
    [SerializeField] private Transform camHolder;
    
    [Header("Camera")]
    /// <summary>
    /// Checks if camera is looking at either the UpClamp and DownClamp values.
    /// </summary>
    [SerializeField] public bool Clamping;
    /// <summary>
    /// Float values that limits the range for looking up and down.
    /// </summary>
    [SerializeField] float UpClamp, DownClamp;

    /// <summary>
    /// For precise smoothing and calculation, I don't know it just works.
    /// </summary>
    [SerializeField] float multiplier = 0.01f;
    /// <summary>
    /// Raw output of mouse look input.
    /// </summary>
    [SerializeField] float xRotation, yRotation;

    /// <summary>
    /// x and y are the calculated versions of xRotation and yRotation, the former follows the latter using a Lerp function that relies on the value of damp.
    /// </summary>
    [SerializeField] public float x, y, damp; 

    private void Update()
    {
        // This one does the actual clamping of the X axis rotation of the camera
        xRotation = cameraManager.clampCam ? Mathf.Clamp(xRotation, UpClamp, DownClamp) : xRotation;

        /**
        Assuming that the PlayerManager component is in the parent/root of the player gameObject, then rotate the said gameObject.
        You should only rotate the Player gameObject's Y axis.
        There should be a separate implementation for visually aiming vertically.
        */
        cameraManager.playerManager.transform.localEulerAngles = new Vector3(0, y, 0);

        // Rotate both the camera's X and Y axis, for Z axis check CameraManager
        camHolder.transform.localEulerAngles = new Vector3(x, y, !cameraManager.reduceMotion ? cameraManager.targetMoveTilt + cameraManager.moveBobRot.z : 0f);

        Look();
    }

    private void Look()
    {
        // Does the actual checking whether camera is aiming right at the Up/Down clamps.
        Clamping = ((xRotation == UpClamp) || (xRotation == DownClamp));

        // Camera smoothing, 10-20 makes it unnoticeable
        x = Mathf.Lerp(x, xRotation, damp * 10f * Time.deltaTime);
        y = Mathf.Lerp(y, yRotation, damp * 10f * Time.deltaTime);

        /**
        This is important, you should flip y and x to each other and vice versa

        Mouse input is simply a Vector2, Y is when you aim your mouse up and down and X is when
        you aim your mouse left and right.
        But for rotation it is different, the X rotation of the camera makes it look/rotate up and down
        and Y rotation of the camera makes it look/rotate left and right.

        So Mouse.Y = Rotate camera's X axis and Mouse.X = Rotate camera's Y axis.

        Hope this makes sense.
        */
        yRotation += PlayerInput.Instance.look.x * cameraManager.sensX * multiplier;

        // Invert Y axis for weirdos
        switch(cameraManager.invertYAxis) 
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