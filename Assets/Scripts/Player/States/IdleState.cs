﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Assets.Scripts;

namespace Assets.Scripts.Player.States
{
    // This is the set of states the player will be in while on the ground and not in a special action. Includes idle, walking, and running
    public class IdleState : PlayerState
    {
        [SerializeField] private bool idle = false;
        private int waitFrames = 2;
        private int stopFrames = 0;
        private Vector2 move;
        private float threshold = .9f;
        private float moveSpeed = 240f;
        private int moveAttackCountdown = 0;
        private bool firstFrame = true;
        private bool rightSmashed = false;
        private bool leftSmashed = false;
        private bool downSmashed = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            move.x = animator.GetFloat("xInput");
            move.y = animator.GetFloat("yInput");
            moveAttackCountdown = waitFrames;
            stopFrames = 0;
            firstFrame = true;
            rightSmashed = false;
            leftSmashed = false;
            downSmashed = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            // To avoid resetting jumps while jumping up through platforms
            if (playerController.GetVelocityY() <= 0 && playerController.OnGround)
            {
                playerController.ResetAirJumps();
                playerController.canRecover = true;
            }

            if (move.x != animator.GetFloat("xInput"))
            {
//                MonoBehaviour.print("Moving x without input: " + move.x + " " + animator.GetFloat("xInput"));
            }

            // Allow changing direction while running before stopping
            if (Mathf.Approximately(move.x, 0))
            {
                if (stopFrames == 2)
                {
                    animator.SetTrigger("SlideOut");
                    stopFrames = 0;
                }
                else
                {
                    stopFrames++;
                }
            }
            else
            {
                stopFrames = 0;
            }

             // Flip code
            if (move.x > 0 && !playerController.facingRight)
            {
                animator.SetTrigger("TurnAround");
            }
            else if (move.x < 0 && playerController.facingRight)
            {
                animator.SetTrigger("TurnAround");
            }
            else if (Mathf.Abs(animator.GetFloat("xInput")) > 0)
            {
                if (!idle)
                {
                    // Movement
                    if (playerController.Run)
                    {
                        // Run
                        if (move.x > 0)
                        {
                            if (playerController.GetVelocityX() < playerController.runSpeedX)
                            {
                                playerController.IncrementVelocityX(moveSpeed*Time.fixedDeltaTime*1.5f/playerController.WeightRatio);
                            }
                        }
                        else if (move.x < 0)
                        {
                            if (playerController.GetVelocityX() > -playerController.runSpeedX)
                            {
                                playerController.IncrementVelocityX(-moveSpeed*Time.fixedDeltaTime*1.5f/playerController.WeightRatio);
                            }
                        }
                    }
                    else
                    {
                        if (move.x > 0)
                        {
                            // Fast walk
                            if (move.x > threshold)
                            {
                                if (playerController.GetVelocityX() < playerController.maxSpeedX)
                                {
                                    playerController.IncrementVelocityX(moveSpeed*Time.fixedDeltaTime*1/playerController.WeightRatio);
                                }
                            }
                            // Slow walk
                            else
                            {
                                if (playerController.GetVelocityX() < .5f*playerController.maxSpeedX &&
                                    !playerController.onEdgeRight)
                                {
                                    playerController.IncrementVelocityX(moveSpeed*Time.fixedDeltaTime/playerController.WeightRatio);
                                }
                            }
                        }
                        else if (move.x < 0)
                        {
                            // Fast walk
                            if (move.x < -threshold)
                            {
                                if (playerController.GetVelocityX() > -playerController.maxSpeedX)
                                {
                                    playerController.IncrementVelocityX(-moveSpeed*Time.fixedDeltaTime/playerController.WeightRatio);
                                }
                            }
                            // Slow walk
                            else
                            {
                                if (playerController.GetVelocityX() > -.5f*playerController.maxSpeedX &&
                                    !playerController.onEdgeLeft)
                                {
                                    playerController.IncrementVelocityX(-moveSpeed*Time.fixedDeltaTime/playerController.WeightRatio);
                                }
                            }
                        }
                    }
                }
            }

            // Should the player be falling?
            move.x = 0;
            move.y = 0;
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
//                MonoBehaviour.print(GetName());
                playerAnimator.SetTrigger("Jump");
            }
        }

        public override void Move(float x, float y)
        {
            // For waiting for a smash from idle
            if (idle)
            {
                // Don't delay inputs if already holding move on state enter
                if (Mathf.Abs(x) > 0 && firstFrame)
                {
                    moveAttackCountdown = 0;
                }
                firstFrame = false;

                if ((moveAttackCountdown > 0) && (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0))
                {
                    // Repeats smashed inputs for set number of frames
                    moveAttackCountdown--;
//                    MonoBehaviour.print(moveAttackCountdown);
                    if (rightSmashed)
                    {
                        base.Right();
                        if (!playerController.facingRight)
                        {
                            playerAnimator.SetTrigger("TurnAround");
//                            playerController.Flip();
                        }
                    }
                    else if (leftSmashed)
                    {
                        base.Left();
                        if (playerController.facingRight)
                        {
                            playerAnimator.SetTrigger("TurnAround");
//                            playerController.Flip();
                        }
                    }
                    else if (downSmashed)
                    {
                        base.Down();
                        //                        playerController.passThroughFloor = true;
                    }
                    else
                    {
                        playerController.passThroughFloor = false;
                    }
                    base.Move(0, 0);
                    move.x = 0;
                    move.y = 0;
//                    move.x = x;
//                    move.y = y;
                }
                else if (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0)
                {
                    // After countdown execute all inputs, including smashes executed in single frame
                    if (rightSmashed)
                    {
                        base.Right();
                        if (!playerController.facingRight)
                        {
                            playerAnimator.SetTrigger("TurnAround");
//                            playerController.Flip();
                        }
                    }
                    else if (leftSmashed)
                    {
                        base.Left();
                        if (playerController.facingRight)
                        {
                            playerAnimator.SetTrigger("TurnAround");
//                            playerController.Flip();
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
                    else
                    {
                        playerController.passThroughFloor = false;
                    }
                    base.Move(x, y);
                    move.x = x;
                    move.y = y;
                    rightSmashed = false;
                    leftSmashed = false;
                    downSmashed = false;
                }
                else
                {
                    // If no input, reset countdown and smashed inputs
                    moveAttackCountdown = waitFrames;
                    base.Move(x, y);
                    if (rightSmashed)
                    {
                        base.Right();
                        rightSmashed = false;
                    }
                    if (leftSmashed)
                    {
                        base.Left();
                        leftSmashed = false;
                    }
                    if (downSmashed)
                    {
                        base.Down();
                        playerController.passThroughFloor = true;
                    }
                    else
                    {
                        playerController.passThroughFloor = false;
                    }
                    base.Move(x, y);
                    move.x = x;
                    move.y = y;
                    rightSmashed = false;
                    leftSmashed = false;
                    downSmashed = false;
                }
            }
            else
            {
                if (rightSmashed)
                {
                    base.Right();
                    rightSmashed = false;
                }
                if (leftSmashed)
                {
                    base.Right();
                    leftSmashed = false;
                }
                if (downSmashed)
                {
                    base.Right();
                    playerController.passThroughFloor = true;
                    downSmashed = false;
                }
                else
                {
                    playerController.passThroughFloor = false;
                }
                base.Move(x, y);
                move.x = x;
                move.y = y;
            }
        }
        
        public override void Up()
        {
            base.Up();
            if (PlayerInputController.GetTapJump())
            {
                if (playerController.velocityY <= 0)
                {
                    playerAnimator.SetTrigger("Jump");
                }
            }
        }

        public override void Down()
        {
            downSmashed = true;
//            base.Down();
//            playerController.passThroughFloor = true;
            // TODO: Need to make down smash turn off pass through floor
            //            playerController.CheckForGround();
            //            if (playerAnimator.GetBool("CanFallThroughFloor"))
            //            {
            //                // TODO: I need to be handling passing through the floor only while actually passing through the floor
            //                playerController.passThroughFloor = true;
            //                playerAnimator.SetTrigger("FallThroughFloor");
            //            }
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