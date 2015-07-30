using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Player.States;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof (PlayerController))]
    public class AIInputController : MonoBehaviour, IInputController
    {
        [SerializeField] public bool TapJump = true;
        [SerializeField] public bool Vibration = true;
        private PlayerController playerController;
        private AIBehaviourController brain;
        private bool xActive = false;
        private bool yActive = false;

        private const float thresholdX = 0.1f;
        private const float thresholdY = 1f;

        private float x = 0f;
        private float y = 0f;
        private bool upPressed = false;
        private bool downPressed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool jump = false;
        private bool run = false;
        private bool primary = false;
        private bool secondary = false;
        private bool block = false;

        private int frame = 0;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            brain = gameObject.AddComponent<AIBehaviourController>();
        }

        public void Init(Menu.MenuInputController menuInputController)
        {
            brain.Init(this, playerController);
            TapJump = false;
        }

        public void FixedUpdate()
        {
//            frame++;
//            if (frame < 60)
//            {
//                x = -1f;
//            }
//            else if (frame < 118)
//            {
//                x = 1f;
//            }
//            else
//            {
//                frame = 0;
//            }

            // Execute playerController moves
            if (playerController.GetState() != null)
            {
                playerController.GetState().Move(x, y);
                if (upPressed)
                {
                    playerController.GetState().Up();
                    upPressed = false;
                }
                if (downPressed)
                {
                    playerController.GetState().Down();
                    downPressed = false;
                }
                if (leftPressed)
                {
                    playerController.GetState().Left();
                    leftPressed = false;
                }
                if (rightPressed)
                {
                    playerController.GetState().Right();
                    rightPressed = false;
                }
                if (jump)
                {
                    playerController.GetState().Jump();
                    jump = false;
                }
                playerController.Run = run;
                if (primary)
                {
                    playerController.GetState().Primary(x, y);
                    primary = false;
                }
                if (secondary)
                {
                    playerController.GetState().Secondary(x, y);
                    secondary = false;
                }
                if (block)
                {
                    playerController.GetState().Block();
                }
                x = 0f;
                y = 0f;
            }
        }

        public void Move(float moveX, float moveY)
        {
            x = moveX;
            y = moveY;
        }

        public void Up()
        {
            upPressed = true;
        }

        public void Down()
        {
            downPressed = true;
        }

        public void Left()
        {
            leftPressed = true;
        }

        public void Right()
        {
            rightPressed = true;
        }

        public void Primary()
        {
            primary = true;
        }

        public void Secondary()
        {
            secondary = true;
        }

        public void SetBlock(bool value)
        {
            block = value;
        }

        public void Jump()
        {
            jump = true;
        }

        public void Run(bool newRun)
        {
            run = newRun;
        }

        public bool ButtonActive(string name)
        {
            if (name == "Block")
            {
                return block;
            }
            else
            {
                return false;
            }
        }

        public bool AxisActive(string name)
        {
            throw new NotImplementedException();
        }

        public bool AxisPositive(string name)
        {
            throw new NotImplementedException();
        }

        public bool AxisNegative(string name)
        {
            throw new NotImplementedException();
        }

        public void VibrateController(float leftMotor, float rightMotor)
        {
        }

        public void StopVibration()
        {
        }


        public bool GetTapJump()
        {
            return false;
        }
    }
}
