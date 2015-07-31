using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class BlockState : PlayerState
    {
        private int waitFrames = 2;
        private int moveAttackCountdown = 0;
        private bool rightSmashed = false;
        private bool leftSmashed = false;
        private bool downSmashed = false;
        private bool blockReleased = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            playerController.Blocking = true;

            moveAttackCountdown = waitFrames;
            rightSmashed = false;
            leftSmashed = false;
            downSmashed = false;
            blockReleased = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (!PlayerInputController.ButtonActive("Block"))
            {
                playerAnimator.SetTrigger("BlockReleased");
                blockReleased = true;
            }

            if (!(blockReleased) && (moveAttackCountdown > 0) && (rightSmashed || downSmashed || leftSmashed))
            {
                // Repeats smashed inputs for set number of frames
                moveAttackCountdown--;
                if (rightSmashed)
                {
                    base.Right();
                    if (!playerController.facingRight)
                    {
                        playerAnimator.SetTrigger("TurnAround");
                    }
                }
                else if (leftSmashed)
                {
                    base.Left();
                    if (playerController.facingRight)
                    {
                        playerAnimator.SetTrigger("TurnAround");
                    }
                }
                else if (downSmashed)
                {
                    base.Down();
                }
            }
            else if (rightSmashed || leftSmashed || downSmashed)
            {
                // After countdown execute all inputs, including smashes executed in single frame
                if (rightSmashed)
                {
                    base.Right();
                    if (!playerController.facingRight)
                    {
                        playerAnimator.SetTrigger("TurnAround");
                    }
                }
                else if (leftSmashed)
                {
                    base.Left();
                    if (playerController.facingRight)
                    {
                        playerAnimator.SetTrigger("TurnAround");
                    }
                }
                else if (downSmashed)
                {
                    base.Down();
                    if (!PlayerInputController.ButtonActive("Primary") && !PlayerInputController.ButtonActive("Secondary"))
                    {
                        playerController.passThroughFloor = true;
                    }
                }
                playerAnimator.SetTrigger("Dodge");
                rightSmashed = false;
                leftSmashed = false;
                downSmashed = false;
            }
            else
            {
                // If no input, reset countdown and smashed inputs
                moveAttackCountdown = waitFrames;
                rightSmashed = false;
                leftSmashed = false;
                downSmashed = false;
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
            if (PlayerInputController.GetTapJump())
            {
                playerAnimator.SetTrigger("Jump");
            }
        }

        public override void Down()
        {
            downSmashed = true;
        }

        public override void Left()
        {
            leftSmashed = true;
        }

        public override void Right()
        {
            rightSmashed = true;
        }
    }
}