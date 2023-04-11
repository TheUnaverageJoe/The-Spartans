public abstract class HopliteBaseState
{
    protected HopliteBaseState superState = null;
    protected HopliteBaseState currentSubState = null;
    protected HopliteStateMachine StateMachine;
    public HopliteBaseState(HopliteStateMachine machine)
    {
        StateMachine = machine;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchState();
    public abstract void InitSubState();

    //void UpdateStates(){}
    //void SwitchState(){}
    void SetSuperState(HopliteBaseState state){
        superState = state;
    }
    void SetSubState(HopliteBaseState state){
        currentSubState = state;
        currentSubState.SetSuperState(this);
    }
}
