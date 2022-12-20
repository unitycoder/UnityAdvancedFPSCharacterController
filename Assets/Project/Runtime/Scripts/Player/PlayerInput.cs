using UnityEngine;
using UnityEngine.InputSystem;

/*
A supposed singleton with a bunch of stuff that handles input. As much as possible, implement features
that need player's input using this class.

Reference - https://www.youtube.com/watch?v=8Yih0p2Kvy0&t=3s&ab_channel=Hero3D

SETUP: I don't have time to explain, just use the "Managers" GameObject at Assets\Project\Runtime\Prefabs\
The inputs can be found at the Input System's Player Input script, make sure to set Behaviour to "Invoke Unity Events"
then a new dropdown should appear. Just do some tinkering with it and you'll understand how this asset does input.
*/
public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;
    PlayerInputActions playerInputActions;
    public Vector2 move { get {return _move; } set { _move = value; } }
    private Vector2 _move;
    public Vector2 look { get {return _look; } set { _look = value; } }
    private Vector2 _look;
    public bool jump { get {return _jump; } set { _jump = value; } }
    private bool _jump;
    public InputAction jumpAction { get {return _jumpAction; } set { _jumpAction = value; } }
    private InputAction _jumpAction;
    public bool crouchHold { get {return _crouchHold; } set { _crouchHold = value; } }
    private bool _crouchHold;
    public InputAction crouchAction { get {return _crouchAction; } set { _crouchAction = value; } }
    private InputAction _crouchAction;
    public bool sprint { get {return _sprint;}  set { _sprint = value; } }
    private bool _sprint;
    public InputAction sprintAction { get {return _sprintAction; } set { _sprintAction = value; } }
    private InputAction _sprintAction;
    public InputAction reloadAction { get {return _reloadAction; } set { _reloadAction = value; } }
    private InputAction _reloadAction;
    public bool shoot { get {return _shoot;}  set { _shoot = value; } }
    private bool _shoot;
    public InputAction primaryAction { get {return _primaryAction; } set { _primaryAction = value; } }
    private InputAction _primaryAction;
    public InputAction secondaryAction { get {return _secondaryAction; } set { _secondaryAction = value; } }
    private InputAction _secondaryAction;

    /*
    Some stuff can't be done with just event-based input like sending contexts using .performed and .canceled
    because I can't find a workaround for them. So I use InputAction.triggered and InputAction.IsPressed() instead.
    */
    private void Awake() {
        Instance = this;
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        jumpAction = playerInputActions.Default.Jump;
        crouchAction = playerInputActions.Default.Crouch;
        sprintAction = playerInputActions.Default.Sprint;
        reloadAction = playerInputActions.Default.Reload;
        primaryAction = playerInputActions.Default.PrimaryAction;
        secondaryAction = playerInputActions.Default.SecondaryAction;
    }

    private void OnEnable()
    {
        playerInputActions.Default.Enable();
    }

    public void MoveInput(InputAction.CallbackContext ctx)
    {
        _move = ctx.ReadValue<Vector2>();
    }

    public void LookInput(InputAction.CallbackContext ctx)
    {
        _look = ctx.ReadValue<Vector2>();
    }

    public void JumpInput(InputAction.CallbackContext ctx)
    {
        _jump = ctx.ReadValueAsButton();
    }

    public void HoldCrouch(InputAction.CallbackContext ctx)
    {
        _crouchHold = ctx.ReadValueAsButton();
    }
    
    public void SprintInput(InputAction.CallbackContext ctx)
    {
        _sprint = ctx.ReadValueAsButton();
    }

    public void ShootInput(InputAction.CallbackContext ctx)
    {
        _shoot = ctx.ReadValueAsButton();
    }

    private void OnDisable()
    {
        playerInputActions.Default.Disable();
    }
}