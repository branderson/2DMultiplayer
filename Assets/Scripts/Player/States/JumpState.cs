using UnityEngine;
using System.Collections;
using Assets.Scripts.Player.States;

namespace Assets.Scripts.Player.States
{
    public class JumpState : PlayerState
    {
        // TODO: have member variable for animator, set on state enter. Set triggers in function calls
        private Vector2 move;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            // Air movement control
            // Only add to speed if below max speed or slowing down from above max speed
            // TODO: Jumps between absolute values at max
            if (move.x > 0)
            {
                if (animator.GetFloat("xVelocity") + playerController.airControlSpeed < playerController.maxAirSpeedX)
                {
                    playerController.IncrementVelocityX(playerController.airControlSpeed);
                }
                else if (animator.GetFloat("xVelocity") + playerController.airControlSpeed > playerController.maxAirSpeedX && animator.GetFloat("xSpeed") < playerController.maxAirSpeedX)
                {
                    playerController.SetVelocityX(playerController.maxAirSpeedX);
                }
            }
            else if (move.x < 0)
            {
                if (animator.GetFloat("xVelocity") - playerController.airControlSpeed > -playerController.maxAirSpeedX)
                {
                    playerController.IncrementVelocityX(-playerController.airControlSpeed);
                }
                else if (animator.GetFloat("xVelocity") - playerController.airControlSpeed < -playerController.maxAirSpeedX && animator.GetFloat("xSpeed") < playerController.maxAirSpeedX)
                {
                    playerController.SetVelocityX(-playerController.maxAirSpeedX);
                }
            }

//            if (playerController.speedX > playerController.maxAirSpeedX)
//            {
//                playerController.SetVelocityX(playerController.maxAirSpeedX);
//            }
//            else if (playerController.speedX < -playerController.maxAirSpeedX)
//            {
//                playerController.SetVelocityX(-playerController.maxAirSpeedX);
//            }

            playerController.CheckForGround();
        }

        public override string GetName()
        {
            return ("Jump");
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);
            move = new Vector2(x, y);
        }

        public override void Up()
        {
            base.Up();
            if (PlayerInputController.TapJump)
            {
                playerAnimator.SetTrigger("Jump");
            }
        }

        public override void Down()
        {
            // Fast fall
            if (playerController.speedY < 0)
            {
                playerController.fastFall = true;
                playerController.canAirJump = false;
            }
        }
    }
}
