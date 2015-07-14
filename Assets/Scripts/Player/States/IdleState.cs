using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Assets.Scripts;

namespace Assets.Scripts.Player.States
{
    // This is the set of states the player will be in while on the ground and not in a special action. Includes idle, walking, and running
    public class IdleState : PlayerState
    {
        private Vector2 move;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            playerController.canAirJump = true;
            playerController.canRecover = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            // Movement
            // TODO: Set speed absolutely rather than relative to move
            if (playerController.run)
            {
                if (move.x > 0)
                {
                    playerController.SetVelocityX(playerController.runSpeedX);
                }
                else if (move.x < 0)
                {
                    playerController.SetVelocityX(-playerController.runSpeedX);
                }
                else
                {
                    playerController.SetVelocityX(0f);
                }
            }
            else 
            {
                if (move.x > 0)
                {
                    playerController.SetVelocityX(playerController.maxSpeedX);
                }
                else if (move.x < 0)
                {
                    playerController.SetVelocityX(-playerController.maxSpeedX);
                }
                else
                {
                    playerController.SetVelocityX(0f);
                }
            }

            // Flip code
            if (move.x > 0 && !playerController.facingRight)
            {
                playerController.Flip();
            }
            else if (move.x < 0 && playerController.facingRight)
            {
                playerController.Flip();
            }


            // Should the player be falling?
            playerController.CheckForGround(); // -> FallState
        }

        public override string GetName()
        {
            return "Idle";
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

        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override void Down()
        {
            base.Down();
            playerController.CheckForGround();
            if (playerAnimator.GetBool("CanFallThroughFloor"))
            {
                // TODO: I need to be handling passing through the floor only while actually passing through the floor
                playerController.passThroughFloor = true;
                playerAnimator.SetTrigger("FallThroughFloor");
            }
        }
    }
}