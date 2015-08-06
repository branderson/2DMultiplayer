using System;
using System.Collections.Generic;
using Assets.Scripts.Menu;
using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using XInputDotNetPure;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof (PlayerController))]
    public class PlayerXInputController : MonoBehaviour, IInputController
    {
        [SerializeField] public bool TapJump = true;
        [SerializeField] public bool Vibration = true;
        [SerializeField] public bool DPad = false;
        internal PlayerIndex XIndex;
        private PlayerController playerController;
        private int xActiveFrames = 0;
        private int yActiveFrames = 0;

        private const float deadZoneX = .1f;
        private const float deadZoneY = .1f;
        private const float thresholdX = 0.5f;
        private const float thresholdY = 0.5f;
        private const float triggerThreshold = .2f;

        private List<string> buttonsPressed;
        private List<byte> activeHeldControls; 
        private List<byte> activePressedControls;
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
        private bool grab = false;

        private Dictionary<string, string> controls; 

        public void Init(MenuInputController menuInputController)
        {
            XIndex = menuInputController.XIndex;
            TapJump = menuInputController.TapJump;
            Vibration = menuInputController.Vibration;
            DPad = menuInputController.DPad;
            controls.Add("Primary", "A");
            controls.Add("Secondary", "B");
            controls.Add("Jump", "X");
            controls.Add("Block", "LeftShoulder");
            controls.Add("Grab", "RightShoulder");
            controls.Add("Run", "RightTrigger");
            controls.Add("TiltLock", "LeftTrigger");
        }

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            buttonsPressed = new List<string>();
            activePressedControls = new List<byte>();
            activeHeldControls = new List<byte>();
            controls = new Dictionary<string, string>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            if (DPad)
            {
                if (gamePadState.DPad.Left == ButtonState.Pressed)
                {
                    x = -1;
                }
                else if (gamePadState.DPad.Right == ButtonState.Pressed)
                {
                    x = 1;
                }
                else
                {
                    x = 0;
                }
                if (gamePadState.DPad.Down == ButtonState.Pressed)
                {
                    y = -1;
                }
                else if (gamePadState.DPad.Up == ButtonState.Pressed)
                {
                    y = 1;
                }
                else
                {
                    y = 0;
                }
            }
            else
            {
                x = gamePadState.ThumbSticks.Left.X;
                y = gamePadState.ThumbSticks.Left.Y;
            }

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

            if (Mathf.Abs(x) < deadZoneX)
            {
                x = 0;
            }
            if (Mathf.Abs(y) < deadZoneY)
            {
                y = 0;
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

            CleanActiveButtons();

            // Check button presses
            if (!jump)
            {
                jump = GetButtonDown("Jump");
            }

            run = ButtonActive("Run");

            if (!primary)
            {
                primary = GetButtonDown("Primary");
            }

            if (!secondary)
            {
                secondary = GetButtonDown("Secondary");
            }

            if (!block)
            {
                block = GetButtonDown("Block");
            }

            if (!grab)
            {
                grab = GetButtonDown("Grab");
            }

            // TODO: Implement start menu
        }


        private void FixedUpdate()
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
                    block = false;
                }
                if (grab)
                {
                    playerController.GetState().Grab();
                    grab = false;
                }
            }
        }

        private void CleanActiveButtons()
        {
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            List<string> inactiveButtons = new List<string>();
            foreach (string button in buttonsPressed)
            {
                if (button == "A")
                {
                    if (gamePadState.Buttons.A == ButtonState.Released)
                    {
                        inactiveButtons.Add(button);
                    }
                }
                else if (button == "B")
                {
                    if (gamePadState.Buttons.B == ButtonState.Released)
                    {
                        inactiveButtons.Add(button);
                    }
                }
                else if (button == "X")
                {
                    if (gamePadState.Buttons.X == ButtonState.Released)
                    {
                        inactiveButtons.Add(button);
                    }
                }
                else if (button == "Y")
                {
                    if (gamePadState.Buttons.Y == ButtonState.Released)
                    {
                        inactiveButtons.Add(button);
                    }
                }
                else if (button == "RightShoulder")
                {
                    if (gamePadState.Buttons.RightShoulder == ButtonState.Released)
                    {
                        inactiveButtons.Add(button);
                    }
                }
                else if (button == "LeftShoulder")
                {
                    if (gamePadState.Buttons.LeftShoulder == ButtonState.Released)
                    {
                        inactiveButtons.Add(button);
                    }
                }
                else if (button == "RightTrigger")
                {
                    if (gamePadState.Triggers.Right < triggerThreshold)
                    {
                        inactiveButtons.Add(button);
                    }
                }
                else if (button == "LeftTrigger")
                {
                    if (gamePadState.Triggers.Left < triggerThreshold)
                    {
                        inactiveButtons.Add(button);
                    }
                }
                else if (button == "Start")
                {
                    if (gamePadState.Buttons.Start == ButtonState.Released)
                    {
                        inactiveButtons.Add(button);
                    }
                }
            }
            foreach (string button in inactiveButtons)
            {
                buttonsPressed.Remove(button);
            }
        }

        private bool GetButtonDown(string control)
        {
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            string button = controls[control];

            if (buttonsPressed.Contains(button))
            {
                return false;
            }

            if (button == "A")
            {
                if (gamePadState.Buttons.A == ButtonState.Pressed)
                {
                    buttonsPressed.Add(button);
                    activePressedControls.Add(0);
                    return true;
                }
            }
            else if (button == "B")
            {
                if (gamePadState.Buttons.B == ButtonState.Pressed)
                {
                    buttonsPressed.Add(button);
                    activePressedControls.Add(1);
                    return true;
                }
            }
            else if (button == "X")
            {
                if (gamePadState.Buttons.X == ButtonState.Pressed)
                {
                    buttonsPressed.Add(button);
                    activePressedControls.Add(2);
                    return true;
                }
            }
            else if (button == "Y")
            {
                if (gamePadState.Buttons.Y == ButtonState.Pressed)
                {
                    buttonsPressed.Add(button);
                    activePressedControls.Add(3);
                    return true;
                }
            }
            else if (button == "RightShoulder")
            {
                if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    buttonsPressed.Add(button);
                    activePressedControls.Add(5);
                    return true;
                }
            }
            else if (button == "LeftShoulder")
            {
                if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    buttonsPressed.Add(button);
                    activePressedControls.Add(4);
                    return true;
                }
            }
            else if (button == "RightTrigger")
            {
                if (gamePadState.Triggers.Right > triggerThreshold)
                {
                    buttonsPressed.Add(button);
                    activePressedControls.Add(7);
                    return true;
                }
            }
            else if (button == "LeftTrigger")
            {
                if (gamePadState.Triggers.Left > triggerThreshold)
                {
                    buttonsPressed.Add(button);
                    activePressedControls.Add(6);
                    return true;
                }
            }
            else if (button == "Start")
            {
                if (gamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    buttonsPressed.Add(button);
                    return true;
                }
            }
            return false;
        }

        public bool ButtonActive(string control)
        {
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            if (!controls.ContainsKey(control))
            {
//                print("Error: " + control);
                return false;
            }
            string button = controls[control];

            if (button == "A")
            {
                if (gamePadState.Buttons.A == ButtonState.Pressed)
                {
                    return true;
                }
            }
            else if (button == "B")
            {
                if (gamePadState.Buttons.B == ButtonState.Pressed)
                {
                    return true;
                }
            }
            else if (button == "X")
            {
                if (gamePadState.Buttons.X == ButtonState.Pressed)
                {
                    return true;
                }
            }
            else if (button == "Y")
            {
                if (gamePadState.Buttons.Y == ButtonState.Pressed)
                {
                    return true;
                }
            }
            else if (button == "RightShoulder")
            {
                if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    activePressedControls.Add(5);
                    return true;
                }
            }
            else if (button == "LeftShoulder")
            {
                if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    return true;
                }
            }
            else if (button == "RightTrigger")
            {
                if (gamePadState.Triggers.Right > triggerThreshold)
                {
                    return true;
                }
            }
            else if (button == "LeftTrigger")
            {
                if (gamePadState.Triggers.Left > triggerThreshold)
                {
                    return true;
                }
            }
            else if (button == "Start")
            {
                if (gamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AxisActive(string name)
        {
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            if (name == "Horizontal")
            {
                if (DPad)
                {
                    return gamePadState.DPad.Left == ButtonState.Pressed || gamePadState.DPad.Right == ButtonState.Pressed;
                }
                return Mathf.Abs(gamePadState.ThumbSticks.Left.X) > thresholdX;
            }
            else if (name == "Vertical")
            {
                if (DPad)
                {
                    return gamePadState.DPad.Up == ButtonState.Pressed || gamePadState.DPad.Down == ButtonState.Pressed;
                }
                return Mathf.Abs(gamePadState.ThumbSticks.Left.Y) > thresholdY;
            }
            else if (name == "HorizonalDPad")
            {
                return gamePadState.DPad.Left == ButtonState.Pressed || gamePadState.DPad.Right == ButtonState.Pressed;
            }
            else if (name == "VerticalDPad")
            {
                return gamePadState.DPad.Up == ButtonState.Pressed || gamePadState.DPad.Down == ButtonState.Pressed;
            }
            return false;
        }

        public bool AxisPositive(string name)
        {
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            if (name == "Horizontal")
            {
                if (DPad)
                {
                    return gamePadState.DPad.Right == ButtonState.Pressed;
                }
                return gamePadState.ThumbSticks.Left.X > thresholdX;
            }
            else if (name == "Vertical")
            {
                if (DPad)
                {
                    return gamePadState.DPad.Up == ButtonState.Pressed;
                }
                return gamePadState.ThumbSticks.Left.Y > thresholdY;
            }
            else if (name == "HorizonalDPad")
            {
                return gamePadState.DPad.Right == ButtonState.Pressed;
            }
            else if (name == "VerticalDPad")
            {
                return gamePadState.DPad.Up == ButtonState.Pressed;
            }
            return false;
        }

        public bool AxisNegative(string name)
        {
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            if (name == "Horizontal")
            {
                if (DPad)
                {
                    return gamePadState.DPad.Left == ButtonState.Pressed;
                }
                return gamePadState.ThumbSticks.Left.X < -thresholdX;
            }
            else if (name == "Vertical")
            {
                if (DPad)
                {
                    return gamePadState.DPad.Down == ButtonState.Pressed;
                }
                return gamePadState.ThumbSticks.Left.Y < -thresholdY;
            }
            else if (name == "HorizonalDPad")
            {
                return gamePadState.DPad.Left == ButtonState.Pressed;
            }
            else if (name == "VerticalDPad")
            {
                return gamePadState.DPad.Down == ButtonState.Pressed;
            }
            return false;
        }

        public void VibrateController(float leftMotor, float rightMotor)
        {
//            print("Trying to vibrate " + (PlayerIndex)(ControllerNumber));
            if (Vibration)
            {
                GamePad.SetVibration(XIndex, leftMotor, rightMotor);
            }
        }

        public void StopVibration()
        {
            if (Vibration)
            {
                GamePad.SetVibration(XIndex, 0f, 0f);
            }
        }


        public bool GetTapJump()
        {
            return TapJump;
        }

        public List<byte> ControllerButtonPressState()
        {
            List<byte> buttonState = new List<byte>(activePressedControls);
            activePressedControls.Clear();
            return buttonState;
        }

        public sbyte[] ControllerAnalogState()
        {
            sbyte[] analogState = {0, 0};
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            float xCheck = gamePadState.ThumbSticks.Left.X;
            float yCheck = gamePadState.ThumbSticks.Left.Y;

            if (AxisPositive("Horizontal"))
            {
                analogState[0] = 2;
            }
            else if (AxisNegative("Horizontal"))
            {
                analogState[0] = -2;
            }
            else if (xCheck > 0 && xCheck < thresholdX)
            {
                analogState[0] = 1;
            }
            else if (xCheck < 0 && xCheck > -thresholdX)
            {
                analogState[0] = -1;
            }
            else
            {
                analogState[0] = 0;
            }

            if (AxisPositive("Vertical"))
            {
                analogState[1] = 2;
            }
            else if (AxisNegative("Vertical"))
            {
                analogState[1] = -2;
            }
            else if (yCheck > 0 && yCheck < thresholdY)
            {
                analogState[1] = 1;
            }
            else if (yCheck < 0 && yCheck > -thresholdY)
            {
                analogState[1] = -1;
            }
            else
            {
                analogState[1] = 0;
            }
            return analogState;
        }


        public List<byte> ControllerButtonHoldState()
        {
            activeHeldControls.Clear();
            if (ButtonActive("Primary") || activePressedControls.Contains(0))
            {
                activeHeldControls.Add(0);
            }
            if (ButtonActive("Secondary") || activePressedControls.Contains(1))
            {
                activeHeldControls.Add(1);
            }
            if (ButtonActive("Jump") || activePressedControls.Contains(2))
            {
                activeHeldControls.Add(2);
            }
            if (ButtonActive("Block") || activePressedControls.Contains(4))
            {
                activeHeldControls.Add(4);
            }
            if (ButtonActive("Grab") || activePressedControls.Contains(5))
            {
                activeHeldControls.Add(5);
            }
            if (ButtonActive("Run") || activePressedControls.Contains(7))
            {
                activeHeldControls.Add(7);
            }
            if (ButtonActive("TiltLock") || activePressedControls.Contains(6))
            {
                activeHeldControls.Add(6);
            }
            activePressedControls.Clear();
            return activeHeldControls;
        }
    }
}