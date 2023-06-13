using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HopliteStateMachine : NetworkBehaviour
{
    //Grounded
        //idle
            // attack
        //walking
            //primary attack
            //special attack ( Spartan Leap )
        //running
            //primary attack (Lunge attack?)
            //

    //Airborn
        //jumping
        //leaping
        //falling

    //HoldingFlag
        // attack?
        // no attacking?

    //Dead
        //timer for respawn

    //BeingExecuted IE Finisher
    //VALID STATES
    public enum States{
        Idle,
        Walk,
        Run,
        Jump,
        Fall
    }

    // CONSTRUCTOR
    public HopliteStateMachine()
    {
        _concreteStates = new Dictionary<States, HopliteBaseState>()
        {
            {States.Idle, new IdleState(this)},
            {States.Walk, new WalkState(this)},
            {States.Jump, new JumpState(this)},
            {States.Fall, new FallState(this)}
        };
    }

    //State Machine PROPERTIES
    private HopliteBaseState _currentState;
    private Dictionary<States, HopliteBaseState> _concreteStates;
    
    

    // Context Variables
    private Vector2 _currentMovement;
    private Vector2 _currentLook;
    private bool _isSprinting;
    private bool _grounded = true;
    private bool _canJump = true;
    public AnimationManager AnimationManager {get; private set;}

    //Getters and Setters
    public Vector2 CurrentMovement {get {return _currentMovement;}}
    public Vector2 CurrentLook {get {return _currentLook;}}
    public bool IsGrounded {get {return _grounded;}}
    public bool JumpReady {get {return _canJump;}}

    void Awake()
    {
        //Get References
        AnimationManager = GetComponent<AnimationManager>();

        // Subscribe to relevant Events
         InputManager.Instance.OnMove += UpdateMoveInput;
         InputManager.Instance.OnLook += UpdateLookInput;
         InputManager.Instance.OnJump += TryJump;
    }
    void Start()
    {
        _currentState = _concreteStates[States.Idle];
        AnimationManager.SetParameter("grounded", true);
    }
    void Update()
    {
        _currentState.UpdateState();
    }
    void FixedUpdate()
    {
        if(IsServer)
        {
            CheckGrounded();
        }
    }

    public void SwitchState(States state)
    {
        _currentState.ExitState();
        _currentState = _concreteStates[state];
        _currentState.EnterState();
    }

    private void UpdateMoveInput(Vector2 input)
    {
        _currentMovement = input;
    }
    private void UpdateLookInput(Vector2 input)
    {
        _currentLook = input;
    }
    private void CheckGrounded()
    {
        if(!_canJump) return;
            RaycastHit hit;
            bool hitOccured = Physics.Raycast(transform.position-(Vector3.down*0.5f), Vector3.down, out hit, 0.6f, 1);
            Debug.DrawRay(transform.position-(Vector3.down*0.5f), Vector3.down * 0.6f, Color.blue);

            if(hitOccured && !_grounded){
                //check if we are the server because This func gets run on server and client
                //thus it would produce a duplicate instruction to change the Animator parameter
                //if(IsServer){
                  AnimationManager.SetParameter("grounded", true);
                //}
                _grounded = true;
            }else if(!hitOccured){
                _grounded = false;
            }else{
                //ALREADY ON GROUND IN THIS CASE
                //print("Unanticipated condition occured");
            }
    }
    private void TryJump()
    {
        if(_canJump && _grounded)
        {
            
        }
    }
}
