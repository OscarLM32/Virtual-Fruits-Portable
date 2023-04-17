using System.Collections.Generic;

/// <summary>
/// Factory pattern of the different states in the PlayerStateMachine
/// </summary>
/// <remarks>The pattern was a proper factory in previous version. However, currently it works out of the pattern's
/// scope, since it only creates all the states once, and after that it simply returns a reference to these created
/// states</remarks>
public class PlayerStateFactory
{
    /// <summary>
    /// "Cache" of the different states. Stores an already created state to avoid creating and destroying states
    /// continuously and thus, improving performance
    /// </summary>
    private Dictionary<States, PlayerBaseState> _cache = new Dictionary<States, PlayerBaseState>();

    /// <summary>
    /// List of all the different states
    /// </summary>
    private enum States
    {
        Grounded,
        Jumping,
        Falling,
        VerticalDashing
    }
   
    //The addition of this methodology breaks the Factory pattern but it is far more lightweight
    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        var context = currentContext;
        
        _cache[States.Grounded] = new PlayerGroundState(context, this);
        _cache[States.Jumping] = new PlayerJumpingState(context, this);
        _cache[States.Falling] = new PlayerFallState(context, this);
        _cache[States.VerticalDashing] = new PlayerVerticalDashingState(context, this);
    }
    
    public PlayerBaseState Grounded()
    {
        return _cache[States.Grounded];
    }
    public PlayerBaseState Jumping()
    {
        return _cache[States.Jumping];
    }

    public PlayerBaseState Falling()
    {
        return _cache[States.Falling];
    }

    public PlayerBaseState VerticalDashing()
    {
        return _cache[States.VerticalDashing];
    }
}
