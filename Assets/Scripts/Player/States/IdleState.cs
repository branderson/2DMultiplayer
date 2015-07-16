using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Assets.Scripts;

namespace Assets.Scripts.Player.States
{
    // This is the set of states the player will be in while on the ground and not in a special action. Includes idle, walking, and running
    public class IdleState : PlayerState
    {
        private Vector2 move;
        private float moveSpeed = 6f;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            // TODO: Sometimes jumping through platforms resets canAirJump
            // To avoid resetting jumps while jumping up through platforms
            if (animator.GetComponentInChildren<Rigidbody2D>().velocity.y <= 0)
            {
                playerController.canAirJump = true;
                playerController.canRecover = true;
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            // Movement
            // TODO: Set multiple levels of movement (Possibly even modify input to have stages of movement controls)
            if (playerController.run)
            {
                if (move.x > 0)
                {
                    if (playerController.GetVelocityX() < playerController.runSpeedX)
                    {
                        playerController.IncrementVelocityX(moveSpeed*1.5f);
                    }
                }
                else if (move.x < 0)
                {
                    if (playerController.GetVelocityX() > -playerController.runSpeedX)
                    {
                        playerController.IncrementVelocityX(-moveSpeed*1.5f);                       
                    }
                }
//                else
//                {
//                    playerController.IncrementSpeedX(-moveSpeed);
//                }
            }
            else 
            {
                if (move.x > 0)
                {
                    if (playerController.GetVelocityX() < playerController.maxSpeedX)
                    {
                        playerController.IncrementVelocityX(moveSpeed);
                    }
                }
                else if (move.x < 0)
                {
                    if (playerController.GetVelocityX() > -playerController.maxSpeedX)
                    {
                        playerController.IncrementVelocityX(-moveSpeed);                       
                    }
                }
//                else
//                {
//                    playerController.IncrementSpeedX(-moveSpeed);
//                }
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

        public override void Jump()
        {
            if (playerController.velocityY <= 0)
            {
                playerAnimator.SetTrigger("Jump");
            }
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
                if (playerController.velocityY <= 0)
                {
                    playerAnimator.SetTrigger("Jump");
                }
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
            playerController.passThroughFloor = true;
            //            playerController.CheckForGround();
            //            if (playerAnimator.GetBool("CanFallThroughFloor"))
            //            {
            //                // TODO: I need to be handling passing through the floor only while actually passing through the floor
            //                playerController.passThroughFloor = true;
            //                playerAnimator.SetTrigger("FallThroughFloor");
            //            }
        }
    }
}