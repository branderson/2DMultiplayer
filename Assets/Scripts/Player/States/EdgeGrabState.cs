using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class EdgeGrabState : PlayerState
    {
        private int maxHoldFrames = 300;
        private int holdTimer = 0;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            holdTimer = maxHoldFrames;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (holdTimer > 0)
            {
                holdTimer--;
            }
            else
            {
                playerAnimator.SetTrigger("LetGo");
            }
        }

        public override string GetName()
        {
            return "EdgeGrabState";
        }

        public override void Up()
        {
            base.Up();
            playerController.IncrementVelocity(-playerController.GetVelocity());
        }

        public override void Down()
        {
            base.Down();
            playerAnimator.SetTrigger("LetGo");
        }

        public override void Jump()
        {
            base.Jump();
            playerController.IncrementVelocity(-playerController.GetVelocity());
        }

        public override void Primary(float x, float y)
        {
            base.Primary(x, y);
            playerController.IncrementVelocity(-playerController.GetVelocity());
        }
    }
}