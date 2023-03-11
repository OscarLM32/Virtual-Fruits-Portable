using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    private static class JumpingAnimations
    {
        public static readonly string Jump = "PlayerJump";
    }

    public PlayerJumpingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        HandleGravity();
        HandleJump();
        HandleAnimation();
        AudioManager.Instance.Play(Context.gameObject,SoundList.PlayerJump);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
    }


    public override void CheckSwitchStates()
    {
        if (Context.IsGrounded)
        {
            SwitchState(Factory.Grounded());
            return;
        }
        
        if (Context.DashPressed + Context.DashGraceTime >= Time.time && !Context.RequireDashReset)
        {
            SwitchState(Factory.VerticalDashing());
            return;
        }

        if (Context.Rb2D.velocity.y <= 0)
        {
            SwitchState(Factory.Falling());
        }
    }
    
    public override void HandleGravity()
    {
        Context.Rb2D.gravityScale = Context.JumpingGravityFactor;
    }
    

    public override void HandleAnimation()
    {
        Context.PlayerAnimator.Play(JumpingAnimations.Jump);
    }

    private void HandleJump()
    {
        Context.Rb2D.velocity = new Vector2(Context.Rb2D.velocity.x, Context.InitialJumpVelocity);
    }
}