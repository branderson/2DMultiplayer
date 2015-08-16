using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.AI;
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
        private int xActiveFrames = 0;
        private int yActiveFrames = 0;

        private const float thresholdX = 0.5f;
        private const float thresholdY = 0.5f;

        private List<string> activeButtons = new List<string>(); 
        private List<string> pressedButtons = new List<string>();
        private float x = 0f;
        private float y = 0f;
        private bool upPressed = false;
        private bool downPressed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool jump = false;
        private bool run = false;
        private bool tiltLock = false;
        private bool primary = false;
        private bool secondary = false;
        private bool block = false;
        private bool grab = false;

        private int frame = 0;
        private int runDelay = 10;

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

        public void Update()
        {
            if (ButtonActive("TiltLock"))
            {
                if (x > .9f)
                {
                    x = .9f;
                }
                else if (x < -.9f)
                {
                    x = -.9f;
                }
            }

            // Check if axes have just been pressed
            if (Mathf.Abs(x) > .1)
            {
                if (xActiveFrames < 3)
                {
                    if (x < -thresholdX)
                    {
                        if (!ButtonActive("TiltLock"))
                        {
                            leftPressed = true;
                        }
                    }
                    else if (x > thresholdX)
                    {
                        if (!ButtonActive("TiltLock"))
                        {
                            rightPressed = true;
                        }
                    }
                }
                xActiveFrames++;
            }
            else
            {
                xActiveFrames = 0;
            }

            if (Mathf.Abs(y) > .1)
            {
                if (yActiveFrames < 3)
                {
                    if (y < -thresholdY)
                    {
                        if (!ButtonActive("TiltLock"))
                        {
                            downPressed = true;
                        }
                    }
                    else if (y > thresholdY)
                    {
                        if (!ButtonActive("TiltLock"))
                        {
                            upPressed = true;
                        }
                    }
                }
                yActiveFrames++;
            }
            else
            {
                yActiveFrames = 0;
            }

            x = 0;
            y = 0;

            foreach (string button in pressedButtons)
            {
                switch (button)
                {
                    case "Primary":
                        Primary();
                        break;
                    case "Secondary":
                        Secondary();
                        break;
                    case "Jump":
                        Jump();
                        break;
                    case "Block":
                        SetBlock(true);
                        break;
                    case "Grab":
                        Grab();
                        break;
                    case "Run":
                        Run(true);
                        break;
                    case "TiltLock":
                        TiltLock(true);
                        break;
                }              
            }
            pressedButtons.Clear();
        }

        public void FixedUpdate()
        {
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
                if (run)
                {
                    playerController.Run = true;
                }
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
                    block = false;
                }
                if (grab)
                {
                    playerController.GetState().Grab();
                    grab = false;
                }
            }
        }

        public void Move(float moveX, float moveY)
        {
            x = moveX;
            y = moveY;
        }

        public void MoveX(float moveX)
        {
            x = moveX;
        }

        public void MoveY(float moveY)
        {
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

        // TODO: Reinplement AI block
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
            if (runDelay <= 0)
            {
                run = newRun;
                runDelay = 10;
            }
            else
            {
                runDelay -= 1;
            }
        }

        // TODO: AI doesn't actually use tilt lock (or does it)
        public void TiltLock(bool newTiltLock)
        {
            tiltLock = newTiltLock;
        }

        public void Grab()
        {
            grab = true;
        }

        public void ClearActiveButtons()
        {
            activeButtons.Clear();
        }

        public void SetButtonActive(string name)
        {
//            print("Setting " + name + " active");
            if (!(activeButtons.Contains(name)))
            {
                activeButtons.Add(name);
                pressedButtons.Add(name);
            }
        }

        public void SetButtonInactive(string name)
        {
//            print("Setting " + name + " inactive");
            if (activeButtons.Contains(name))
            {
                activeButtons.Remove(name);
                if (name == "Block")
                {
                    SetBlock(false);
                }
                else if (name == "Run")
                {
                    Run(false);
                }
                else if (name == "TiltLock")
                {
                    TiltLock(false);
                }
            }
        }

        public bool ButtonActive(string name)
        {
            if (name == "Block")
            {
                return block;
            }
            return activeButtons.Contains(name);
        }

        public bool AxisActive(string name)
        {
            if (name == "Vertical")
            {
                return Mathf.Abs(y) > thresholdY;
            }
            if (name == "Horizontal")
            {
                return Mathf.Abs(x) > thresholdX;
            }
            return false;
        }

        public bool AxisPositive(string name)
        {
            if (name == "Vertical")
            {
                return y > thresholdY;
            }
            if (name == "Horizontal")
            {
                return x > thresholdX;
            }
            return false;
        }

        public bool AxisNegative(string name)
        {
            if (name == "Vertical")
            {
                return y < -thresholdY;
            }
            if (name == "Horizontal")
            {
                return x < -thresholdX;
            }
            return false;
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

        public List<byte> ControllerButtonPressState()
        {
            throw new NotImplementedException();
        }

        public sbyte[] ControllerAnalogState()
        {
            throw new NotImplementedException();
        }


        public List<byte> ControllerButtonHoldState()
        {
            throw new NotImplementedException();
        }
    }
}
