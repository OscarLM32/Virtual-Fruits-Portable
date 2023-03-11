using System.Collections.Generic;
using Player.StateMachine.States;
using UnityEngine;

public class PlayerStateFactory
{
    private PlayerStateMachine _context;
    private Dictionary<States, PlayerBaseState> _cache = new Dictionary<States, PlayerBaseState>();

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
        _context = currentContext;
        
        _cache[States.Grounded] = new PlayerGroundState(_context, this);
        _cache[States.Jumping] = new PlayerJumpingState(_context, this);
        _cache[States.Falling] = new PlayerFallState(_context, this);
        _cache[States.VerticalDashing] = new PlayerVerticalDashingState(_context, this);
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
