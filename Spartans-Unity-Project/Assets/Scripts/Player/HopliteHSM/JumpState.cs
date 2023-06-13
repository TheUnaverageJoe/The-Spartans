using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : HopliteBaseState
{
    public JumpState(HopliteStateMachine context) : base(context){}
    public override void EnterState()
    {
        Debug.Log("Entered Jump State");
        StateMachine.AnimationManager.SetParameter("grounded", false);
    }
    public override void UpdateState()
    {
        CheckSwitchState();
        if(currentSubState != null) currentSubState.UpdateState();
    }
    public override void ExitState()
    {
        Debug.Log("Exiting Jump State");
    }
    public override void CheckSwitchState()
    {
        if(StateMachine.IsGrounded && StateMachine.CurrentMovement == Vector2.zero){
            StateMachine.SwitchState(HopliteStateMachine.States.Idle);
        }else if(!StateMachine.IsGrounded){
            StateMachine.SwitchState(HopliteStateMachine.States.Fall);
        }else{
            StateMachine.SwitchState(HopliteStateMachine.States.Walk);
        }
    }
    public override void InitSubState()
    {
        throw new System.NotImplementedException();
    }

}
