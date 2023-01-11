using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerControls Controls;

    //public Vector2 MoveInput = new Vector2();
    //public Vector2 LookInput = new Vector2();
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnInteract;
    public event Action OnEscape;
    public event Action OnJump;
    public event Action OnPrimary;
    public event Action OnSecondary;
    public event Action OnSpecial;
    public event Action OnSprint;
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
    // Start is called before the first frame update
    void Start()
    {
        Controls.Player.Enable();

        Controls.Player.Move.performed += Move;
        Controls.Player.Move.canceled += Move;
        Controls.Player.Look.performed += Look;
        Controls.Player.Look.canceled += Look;

        Controls.Player.Primary.performed += Primary;
        Controls.Player.Secondary.performed += Secondary;
        Controls.Player.Special.performed += Special;
        Controls.Player.Jump.performed += Jump;
        Controls.Player.Interact.performed += Interact;
        Controls.Player.Escape.performed += Escape;
        Controls.Player.Sprint.performed += Sprint;
        Controls.Player.Sprint.canceled += Sprint;

        Controls.UI.Exit.performed += Escape;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void Escape(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            OnEscape?.Invoke();
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            OnInteract?.Invoke();
        }
    }

    private void Primary(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            //print("Fired");
            OnPrimary?.Invoke();
        }
    }
    private void Secondary(InputAction.CallbackContext context)
    {
        OnSecondary?.Invoke();
    }
    private void Special(InputAction.CallbackContext context)
    {
        OnSpecial?.Invoke();
    }
    private void Sprint(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {

            OnSprint?.Invoke();
            print("Sprinting");
        }
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            OnSprint?.Invoke();
            print("sprint stopped");
        }
    }

    private void Look(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            OnLook?.Invoke(context.ReadValue<Vector2>());
        }
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            
        }
    }

    private void Move(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            OnMove?.Invoke(context.ReadValue<Vector2>());
        }
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            OnMove?.Invoke(context.ReadValue<Vector2>());
        }
    }


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

    private void OnDisable()
    {
        if(this != Instance)
        {
            //Debug.Log("nothing to do this time");
            return;
        }
        Controls.Player.Move.performed -= Move;
        Controls.Player.Move.canceled -= Move;
        Controls.Player.Look.performed -= Look;
        Controls.Player.Look.canceled -= Look;

        Controls.Player.Primary.performed -= Primary;
        Controls.Player.Secondary.performed -= Secondary;
        Controls.Player.Special.performed -= Special;
        Controls.Player.Jump.performed -= Jump;
        Controls.Player.Interact.performed -= Interact;
        Controls.Player.Escape.performed -= Escape;
        
        Controls.UI.Exit.performed -= Escape;
    }
}
