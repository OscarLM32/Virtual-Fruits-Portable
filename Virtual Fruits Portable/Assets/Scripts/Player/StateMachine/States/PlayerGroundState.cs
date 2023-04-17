using UnityEngine;

/// <summary>
/// State that handles the logic of the player when it is grounded
/// </summary>
public class PlayerGroundState : PlayerBaseState
{
    private static class GroundedAnimations
    {
        public static readonly string Walk = "PlayerWalk";
        public static readonly string Run = "PlayerRun";
        public static readonly string Sprint = "PlayerSprint";
    }

    public PlayerGroundState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Context.RequireDashReset = false;
        Context.Rb2D.velocity *= Vector2.right;
    }

    public override void UpdateState()
    {
        HandleAnimation();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
    }


    public override void CheckSwitchStates()
    {
        if (Context.JumpPressed + Context.JumpGraceTime >= Time.time)
        {
            SwitchState(Factory.Jumping());
            return;
        }

        //We don't want to be able to do the downwards dash while grounded
        if (Context.DashPressed + Context.DashGraceTime >= Time.time && Context.IsUpwardsDash)
        {
            SwitchState(Factory.VerticalDashing());
            return;
        }

        if (!Context.IsGrounded)
        {
            SwitchState(Factory.Falling());
        }
    }

    public override void HandleAnimation()
    {
        //All animation are the same with different speeds, but will be left like this
        //So in the future, whenever we have an animation for each state the change is easier
        switch (Context.CurrentMovementType)
        {
            case PlayerMovementType.Walk:
                Context.PlayerAnimator.Play(GroundedAnimations.Walk);
                break;
            case PlayerMovementType.Run:
                Context.PlayerAnimator.Play(GroundedAnimations.Run);
                break;
            case PlayerMovementType.Sprint:
                Context.PlayerAnimator.Play(GroundedAnimations.Sprint);
                break;
        }
    }

    public override void HandleGravity()
    {
        Context.Rb2D.gravityScale = 1;
    }
}