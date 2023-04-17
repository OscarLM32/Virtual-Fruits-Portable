using UnityEngine;

/// <summary>
/// State that handles the logic of the player when it is falling
/// </summary>
public class PlayerFallState : PlayerBaseState
{
    private static class FallingAnimations
    {
        public static readonly string Fall = "PlayerFall";
    }
    
    /// <summary>
    /// Max speed the player can fall from the air at
    /// </summary>
    private const float maxFallVelocity = -20;

    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        HandleAnimation();
        HandleGravity();
    }

    public override void UpdateState()
    {
        CheckMaxFallVelocity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void CheckSwitchStates()
    {
        if (Context.DashPressed + Context.DashGraceTime >= Time.time && (!Context.RequireDashReset || !Context.IsUpwardsDash))
        {
            SwitchState(Factory.VerticalDashing());
            return;
        }
        
        if (Context.IsGrounded)
        {
            SwitchState(Factory.Grounded());
            return;
        }
    }

    public override void HandleGravity()
    {
        //The jump height cannot be controlled in this game, so the fall gravity is the same as the jump
        Context.Rb2D.gravityScale = Context.JumpingGravityFactor;
    }

    public override void HandleAnimation()
    {
        Context.PlayerAnimator.Play(FallingAnimations.Fall);
    }
    
    /// <summary>
    /// Limits the max speed at which the player can fall down
    /// </summary>
    private void CheckMaxFallVelocity()
    {
        Vector2 velocity = Context.Rb2D.velocity;
        Context.Rb2D.velocity = new Vector2(velocity.x,Mathf.Max(velocity.y, maxFallVelocity));
    }
}