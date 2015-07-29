using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class BlockState : PlayerState
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            playerController.Blocking = true;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (!PlayerInputController.ButtonActive("Block"))
            {
                playerAnimator.SetTrigger("BlockReleased");
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            playerController.Blocking = false;
        }

        public override string GetName()
        {
            return "Block";
        }

        public override void Up()
        {
            base.Up();
        }
    }
}