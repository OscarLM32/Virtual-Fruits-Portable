
public abstract class PlayerBaseState
{
    protected PlayerStateMachine Context;
    protected PlayerStateFactory Factory;

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        Context = currentContext;
        Factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();
    
    public abstract void CheckSwitchStates();
    
    protected void SwitchState(PlayerBaseState newState)
    {
        //Current state exits state
        ExitState();
        //New state enters state
        newState.EnterState();
        //Switch current state of context
        Context.CurrentState = newState;
    }

    public abstract void HandleGravity();

    public abstract void HandleAnimation();
}
