using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : HopliteBaseState
{
    //CONSTRUCTOR
    public WalkState(HopliteStateMachine context) : base(context) {}

    //METHOD OVERRIDES
    public override void EnterState()
    {
        Debug.Log("Entered Walk State");
        StateMachine.AnimationManager.SetParameter("speed", StateMachine.CurrentMovement.magnitude);
    }
    public override void UpdateState()
    {
        CheckSwitchState();
        if(currentSubState != null) currentSubState.UpdateState();
    }
    public override void ExitState()
    {
        Debug.Log("Exiting Walk State");
    }
    public override void CheckSwitchState()
    {
        if(StateMachine.CurrentMovement == Vector2.zero)
        {
            StateMachine.SwitchState(HopliteStateMachine.States.Idle);
        }
    }
    public override void InitSubState()
    {
        throw new System.NotImplementedException();
    }

}
