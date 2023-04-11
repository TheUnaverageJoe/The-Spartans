using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : HopliteBaseState
{
    //CONSTRUCTOR
    public IdleState(HopliteStateMachine context) : base (context){}


    //METHOD OVERRIDES
    public override void EnterState()
    {
        Debug.Log("Entered Idle State");
        StateMachine.AnimationManager.SetParameter("speed", StateMachine.CurrentMovement.magnitude);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if(currentSubState != null) currentSubState.UpdateState();
    }
    public override void ExitState()
    {
        Debug.Log("Exiting Idle State");
    }
    public override void CheckSwitchState()
    {
        if(StateMachine.CurrentMovement != Vector2.zero)
        {
            StateMachine.SwitchState(HopliteStateMachine.States.Walk);
        }
    }
    public override void InitSubState()
    {
        throw new System.NotImplementedException();
    }

}
