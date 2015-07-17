using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts.Player.States
{
    public class RecoveryState : PlayerState
    {
        [SerializeField] private int waitFrames = 4;
        [SerializeField] private bool directionalControl = true;
        private float maximumNegationVelocity = 40f;
        private int waitCounter;
        private int jumpDirection = 0;
        private float directionModifier = 1;
        private Vector2 move;

        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            base.OnStateEnter(animator, stateinfo, layerindex);
            playerController.canRecover = false;
            // Stop the player from fast falling
            playerController.fastFall = false;
            waitCounter = waitFrames;
            jumpDirection = 0;
            directionModifier = 1;
            playerController.SetVibrate(15, 1f, 1f);
        }

        public virtual new void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            waitCounter -= 1;
            if (waitCounter == 0)
            {
                if (jumpDirection == 1)
                    animator.SetTrigger("Forward");
                else if (jumpDirection == -1)
                    animator.SetTrigger("Backward");
                else if (jumpDirection == 0)
                    animator.SetTrigger("Jump");
            }
        }

        public virtual new void OnStateExit(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            // TODO: Air jumps need to use airJumpSpeed
            if (playerController.facingRight)
                directionModifier = 1;
            else
                directionModifier = -1;

            if (!directionalControl || jumpDirection == 0)
            {
                float adjustSpeed = -playerController.GetVelocityX();
                AdjustSpeed(adjustSpeed);
                playerController.Jump(playerController.recoverySpeed);
            }
            // TODO: Recovery side jumps should be set up
            else if (jumpDirection == 1)
            {
                if (playerController.GetSpeedX() < playerController.maxAirSpeedX) 
                {
                    float adjustSpeed = 2*playerController.airSideJumpSpeedX*directionModifier - playerController.GetVelocityX();
                    AdjustSpeed(adjustSpeed);
                }
                playerController.Jump(playerController.recoverySpeed);
            }
            else if (jumpDirection == -1)
            {
                if (playerController.GetSpeedX() < playerController.maxAirSpeedX) 
                {
                    float adjustSpeed = -2*playerController.airSideJumpSpeedX*directionModifier - playerController.GetVelocityX();
                    AdjustSpeed(adjustSpeed);
                }
                playerController.Jump(playerController.recoverySpeed);
                playerController.Flip();
            }
        }

        private void AdjustSpeed(float adjustSpeed)
        {
//            MonoBehaviour.print("Adjusting speed: Target speed is " + PlayerController.airSideJumpSpeedX);
//            MonoBehaviour.print("Current speed: " + PlayerController.GetVelocityX() + ", adjusting by: " + adjustSpeed);
//            MonoBehaviour.print("Final speed should be " + PlayerController.GetVelocityX() + " + " + adjustSpeed  + " = " + (PlayerController.GetVelocityX() + adjustSpeed));
            if (Mathf.Abs(adjustSpeed) < maximumNegationVelocity)
            {
                playerController.IncrementVelocityX(adjustSpeed);
            }
            else
            {
                playerController.IncrementVelocityX(maximumNegationVelocity*directionModifier);
//                MonoBehaviour.print("Adjustment of " + Mathf.Abs(adjustSpeed) + " exceeded maximum of " + maximumNegationVelocity);
            }
//            MonoBehaviour.print("Final velocity: " + PlayerController.GetVelocityX());
        }

        public override string GetName()
        {
            return "Recovery";
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);
            move = new Vector2(x, y);
            if (jumpDirection == 0)
            {
                if ((move.x > 0 && playerController.facingRight) || (move.x < 0 && !playerController.facingRight))
                    jumpDirection = 1;
                else if (move.x != 0)
                    jumpDirection = -1;
            }
        }

        public override void Left()
        {
        }

        public override void Right()
        {
        }
    }
}