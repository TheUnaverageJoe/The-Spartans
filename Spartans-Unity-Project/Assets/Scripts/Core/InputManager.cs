using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerControls Controls;

    private Vector2 _moveInput;
    private Vector2 _lookInput;

    //Getters and Setters
    public Vector2 MoveInput {get {return _moveInput;}}
    public Vector2 LookInput {get {return _lookInput;}}

    //Public events
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnInteract;
    public event Action OnEscape;
    public event Action OnJump;
    public event Action OnPrimary;
    public event Action OnSecondary;
    public event Action OnSpecial;
    public event Action OnSprint;

    // Singleton Structure
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            Controls = new PlayerControls();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Init input listeners
    void Start()
    {
        Controls.Player.Enable();

        Controls.Player.Move.performed += MoveHandler;
        Controls.Player.Move.canceled += MoveHandler;
        Controls.Player.Look.performed += LookHandler;
        Controls.Player.Look.canceled += LookHandler;

        Controls.Player.Primary.performed += PrimaryHandler;
        Controls.Player.Secondary.performed += SecondaryHandler;
        Controls.Player.Special.performed += SpecialHandler;
        Controls.Player.Jump.performed += JumpHandler;
        Controls.Player.Interact.performed += InteractHandler;
        Controls.Player.Escape.performed += EscapeHandler;
        Controls.Player.Sprint.performed += SprintHandler;
        Controls.Player.Sprint.canceled += SprintHandler;

        Controls.UI.Exit.performed += EscapeHandler;
    }

    //Event Handlers
    private void MoveHandler(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            OnMove?.Invoke(_moveInput);
        }
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            _moveInput = context.ReadValue<Vector2>();
            OnMove?.Invoke(_moveInput);
        }
    }
    private void LookHandler(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            _lookInput = context.ReadValue<Vector2>();
            OnLook?.Invoke(_lookInput);
        }
        //else if(context.action.phase == InputActionPhase.Canceled)
        {
            //Debug.LogWarning("Looking stopped!");
            //_lookInput = context.ReadValue<Vector2>();
            //OnLook?.Invoke(_lookInput);
        }
    }
    private void JumpHandler(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }
    private void EscapeHandler(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            OnEscape?.Invoke();
        }
    }
    private void InteractHandler(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            OnInteract?.Invoke();
        }
    }

    private void PrimaryHandler(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            //print("Fired");
            OnPrimary?.Invoke();
        }
    }
    private void SecondaryHandler(InputAction.CallbackContext context)
    {
        OnSecondary?.Invoke();
    }
    private void SpecialHandler(InputAction.CallbackContext context)
    {
        OnSpecial?.Invoke();
    }
    private void SprintHandler(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {

            OnSprint?.Invoke();
            //print("Sprinting");
        }
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            OnSprint?.Invoke();
            //print("sprint stopped");
        }
    }

    

    // Getter for action map just in case
    public InputActionMap CurrentActionMap()
    {
        
        if(Controls.Player.enabled && !Controls.UI.enabled)
        {
            return Controls.Player;
        }
        else if(Controls.UI.enabled && !Controls.Player.enabled)
        {
            return Controls.UI;
        }
        else
        {
            Debug.LogError("Either both or none action maps are active");
            return null;
        }
    }
    //
    public void PauseInput()
    {
        Controls.Player.Disable();
        Controls.UI.Enable();
    }
    public void ResumeInput()
    {
        Controls.Player.Enable();
        Controls.UI.Disable();
    }

    //Handle Unsubbing listeners
    private void OnDisable()
    {
        if(this != Instance)
        {
            //Debug.Log("nothing to do this time");
            return;
        }

        Controls.Player.Move.performed -= MoveHandler;
        Controls.Player.Move.canceled -= MoveHandler;
        Controls.Player.Look.performed -= LookHandler;
        Controls.Player.Look.canceled -= LookHandler;

        Controls.Player.Primary.performed -= PrimaryHandler;
        Controls.Player.Secondary.performed -= SecondaryHandler;
        Controls.Player.Special.performed -= SpecialHandler;
        Controls.Player.Jump.performed -= JumpHandler;
        Controls.Player.Interact.performed -= InteractHandler;
        Controls.Player.Escape.performed -= EscapeHandler;
        Controls.Player.Sprint.performed -= SprintHandler;
        Controls.Player.Sprint.canceled -= SprintHandler;
        
        Controls.UI.Exit.performed -= EscapeHandler;
    }
}
