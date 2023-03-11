using UnityEngine;

namespace Player.StateMachine.States
{
    public class PlayerVerticalDashingState : PlayerBaseState
    {
        //Future work: create new animations for each of the states of the dash
        private static class DashingAnimations
        {
            public static readonly string Upwards = "PlayerJump";
            public static readonly string Downwards = "PlayerFall";
        }
        
        public PlayerVerticalDashingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
            : base(currentContext, playerStateFactory)
        {
        }

        public override void EnterState()
        {
            HandleDash();
            HandleGravity();
            HandleAnimation();
            AudioManager.Instance.Play(Context.gameObject, SoundList.PlayerDash);
            Context.RequireDashReset = true;
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

            if (Context.Rb2D.velocity.y <= 0 && Context.IsUpwardsDash)
            {
                SwitchState(Factory.Falling());
            }
        }

        public override void HandleGravity()
        {
            if (Context.IsUpwardsDash)
                Context.Rb2D.gravityScale = Context.DashingGravityFactor;
            else
                Context.Rb2D.gravityScale = 0.1f;
        }

        public override void HandleAnimation()
        {
            if(Context.IsUpwardsDash)
                Context.PlayerAnimator.Play(DashingAnimations.Upwards);
            else
                Context.PlayerAnimator.Play(DashingAnimations.Downwards);
        }

        private void HandleDash()
        {
            float verticalVelocity = Context.IsUpwardsDash ? Context.InitialDashVelocity : -Context.InitialDashVelocity;
            Context.Rb2D.velocity = new Vector2(Context.Rb2D.velocity.x, verticalVelocity);
        }
    }
}