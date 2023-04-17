/// <summary>
/// Abstract class defining the basic behaviour of any state part of the PlayerStateMachine
/// </summary>
public abstract class PlayerBaseState
{
    /// <summary>
    /// Updated context of the state of the whole state machine
    /// </summary>
    protected PlayerStateMachine Context;
    
    /// <summary>
    /// Reference to the factory of the states in the state machine
    /// </summary>
    protected PlayerStateFactory Factory;

    protected PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        Context = currentContext;
        Factory = playerStateFactory;
    }

    /// <summary>
    /// Enter state is called once when the state is first accessed
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Update state is called once per frame while the state is active
    /// </summary>
    /// <remarks>I could be called more than once if the method is called in FixedUpdate instead of Update
    /// in PlayerStateMachine</remarks>
    /// <seealso cref="PlayerStateMachine"/>
    public abstract void UpdateState();

    /// <summary>
    /// Exit state is called once when the state is going to be switched
    /// </summary>
    public abstract void ExitState();
    
    /// <summary>
    /// Checks if the states should be switched and in that case it switches them
    /// </summary>
    public abstract void CheckSwitchStates();
    
    /// <summary>
    /// Switches the current state with a new state
    /// </summary>
    /// <param name="newState">The new state to switch to</param>
    protected void SwitchState(PlayerBaseState newState)
    {
        //Current state exits state
        ExitState();
        //New state enters state
        newState.EnterState();
        //Switch current state of context
        Context.CurrentState = newState;
    }

    /// <summary>
    /// Handles the gravity by the specifics of the state
    /// </summary>
    public abstract void HandleGravity();

    /// <summary>
    /// Handles the animations of the state
    /// </summary>
    public abstract void HandleAnimation();
}
