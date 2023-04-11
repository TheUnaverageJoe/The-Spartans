using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : HopliteBaseState
{
    //CONSTRUCTOR
    public FallState(HopliteStateMachine context) : base (context){}

    public override void EnterState()
    {
        Debug.Log("Entered Fall State");
        StateMachine.AnimationManager.SetParameter("grounded", false);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if(currentSubState != null) currentSubState.UpdateState();
    }
    public override void ExitState()
    {
        Debug.Log("Exiting Fall State");
    }
    public override void CheckSwitchState()
    {
        if(StateMachine.IsGrounded && StateMachine.CurrentMovement != Vector2.zero){
            StateMachine.SwitchState(HopliteStateMachine.States.Walk);
        }
        else if(StateMachine.IsGrounded){
            StateMachine.SwitchState(HopliteStateMachine.States.Idle);
        }
    }
    public override void InitSubState()
    {
        
    }

}
