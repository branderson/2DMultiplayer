using UnityEngine;
using System.Collections;
using Assets.Scripts.Player.States;

namespace Assets.Scripts.Player.States
{
    public class JumpState : PlayerState
    {
        private bool jump = false;
        private Vector2 move;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            jump = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerController.canAirJump && jump)
            {
                animator.SetTrigger("Jump");
                playerController.canAirJump = false;
            }
            jump = false;

            // Air movement control
            playerController.IncrementVelocityX(move.x * playerController.airControlSpeed);

            if (playerController.speedX > playerController.maxAirSpeedX)
            {
                playerController.SetVelocityX(playerController.maxAirSpeedX);
            }
            else if (playerController.speedX < -playerController.maxAirSpeedX)
            {
                playerController.SetVelocityX(-playerController.maxAirSpeedX);
            }

            playerController.CheckForGround();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        public override string GetName()
        {
            return ("Jump");
        }

        public override void Jump()
        {
            jump = true;
        }

        public override void Move(float x, float y)
        {
            this.move = new Vector2(x, y);
        }

        public override void Action1(float x, float y)
        {
        }

        public override void Action2(float x, float y)
        {
            throw new System.NotImplementedException();
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
            // Fast fall
            if (playerController.speedY < 0)
            {
                playerController.fastFall = true;
            }
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
