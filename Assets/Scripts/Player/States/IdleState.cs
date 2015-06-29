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
        private bool jump = false;
        private bool action1 = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            jump = false;
            action1 = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Jump code
            if (jump)
            {
                if (playerController.CheckForGround())
                {
                    animator.SetTrigger("Jump");
                }
            }
            jump = false;

            // Attack code
            if (action1)
            {
                animator.SetTrigger("Action1");
            }
            action1 = false;

            // Movement
            playerController.SetVelocityX(move.x * playerController.maxSpeedX);

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

        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        public override void Jump()
        {
            this.jump = true;
        }

        public override string GetName()
        {
            return "Idle";
        }

        public override void Move(float x, float y)
        {
            this.move = new Vector2(x, y);
        }

        public override void Action1(float x, float y)
        {
            this.action1 = true;
        }

        public override void Action2(float x, float y)
        {
        }

        public override void Block()
        {
            throw new System.NotImplementedException();
        }

        public override void Throw()
        {
            throw new System.NotImplementedException();
        }

        public override void Up()
        {
            jump = true;
        }

        public override void Down()
        {
        }

        public override void Left()
        {
            throw new System.NotImplementedException();
        }

        public override void Right()
        {
            throw new System.NotImplementedException();
        }
    }
}