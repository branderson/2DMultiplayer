using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class FlipPlayer : PlayerState
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInChildren<PlayerController>().Flip();
        }

        public override string GetName()
        {
            return "TurnAround";
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
    }
}