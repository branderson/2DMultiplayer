using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts.Player.States
{
    public class JumpStartState : PlayerState
    {
        [SerializeField] private int waitFrames = 4;
        [SerializeField] private bool directionalControl = true;
        private float maximumNegationVelocity = 25f;
        private int waitCounter;
        private int jumpDirection = 0;
        private float directionModifier = 1;
        private bool shortHop = false;
        private Vector2 move;

        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            base.OnStateEnter(animator, stateinfo, layerindex);
            waitCounter = waitFrames;
            jumpDirection = 0;
            directionModifier = 1;
            shortHop = false;
        }

        public virtual new void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            waitCounter -= 1;
            if (waitCounter == 0)
            {
                if (!PlayerInputController.ButtonActive("Jump") && (!PlayerInputController.TapJump || !PlayerInputController.AxisPositive("Vertical")))
                {
                    shortHop = true;
                }
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

            if (playerController.CheckForGround() && playerController.GetVelocityY() < .1f)
            {
                // Need to do this in this state for now because short hop information would not pass on to next state
                if (!directionalControl || jumpDirection == 0)
                {
                    if (shortHop)
                    {
                        float adjustSpeed = -playerController.GetVelocityX();
                        AdjustSpeed(adjustSpeed);
                        playerController.Jump(playerController.jumpSpeed*playerController.shortHopFactor);
                    }
                    else
                    {
                        float adjustSpeed = -playerController.GetVelocityX();
                        AdjustSpeed(adjustSpeed);
                        playerController.Jump(playerController.jumpSpeed);
                    }
                }
                else if (jumpDirection == 1)
                {
                    if (shortHop)
                    {
                        if (playerController.GetSpeedX() < playerController.maxAirSpeedX)
                        {
                            float adjustSpeed = playerController.sideJumpSpeedX*playerController.shortHopFactor*
                                                directionModifier - playerController.GetVelocityX();
                            AdjustSpeed(adjustSpeed);
                        }
                        playerController.Jump(playerController.sideJumpSpeedY*playerController.shortHopFactor);
                    }
                    else
                    {
                        if (playerController.GetSpeedX() < playerController.maxAirSpeedX) 
                        {
                            float adjustSpeed = playerController.sideJumpSpeedX*directionModifier - playerController.GetVelocityX();
                            AdjustSpeed(adjustSpeed);
                        }
                        playerController.Jump(playerController.sideJumpSpeedY);
                    }
                }
                else if (jumpDirection == -1)
                {
                    if (shortHop)
                    {
                        if (playerController.GetSpeedX() < playerController.maxAirSpeedX) 
                        {
                            float adjustSpeed = -playerController.sideJumpSpeedX*playerController.shortHopFactor*
                                                directionModifier - playerController.GetVelocityX();
                            AdjustSpeed(adjustSpeed);
                        }
                        playerController.Jump(playerController.sideJumpSpeedY*playerController.shortHopFactor);
                    }
                    else
                    {
                        if (playerController.GetSpeedX() < playerController.maxAirSpeedX) 
                        {
                            float adjustSpeed = -playerController.sideJumpSpeedX*directionModifier - playerController.GetVelocityX();
                            AdjustSpeed(adjustSpeed);
                        }
                        playerController.Jump(playerController.sideJumpSpeedY);
                    }
                }
            }
            // Air jump
            else
            {
                if (!directionalControl || jumpDirection == 0)
                {
                    float adjustSpeed = -playerController.GetVelocityX();
                    AdjustSpeed(adjustSpeed);
                    playerController.Jump(playerController.airJumpSpeed);
                }
                else if (jumpDirection == 1)
                {
                    if (playerController.GetSpeedX() < playerController.maxAirSpeedX) 
                    {
                        float adjustSpeed = playerController.airSideJumpSpeedX*directionModifier - playerController.GetVelocityX();
                        AdjustSpeed(adjustSpeed);
                    }
                    playerController.Jump(playerController.airSideJumpSpeedY);
                }
                else if (jumpDirection == -1)
                {
                    if (playerController.GetSpeedX() < playerController.maxAirSpeedX) 
                    {
                        float adjustSpeed = -playerController.airSideJumpSpeedX*directionModifier - playerController.GetVelocityX();
                        AdjustSpeed(adjustSpeed);
                    }
                    playerController.Jump(playerController.airSideJumpSpeedY);
                    // Do I want to be able to flip on air jump?
//                    PlayerController.Flip();
                }
                playerController.canAirJump = false;
            }
        }

        public override string GetName()
        {
            throw new System.NotImplementedException();
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