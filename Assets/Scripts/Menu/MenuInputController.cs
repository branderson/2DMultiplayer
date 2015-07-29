using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using XInputDotNetPure;

namespace Assets.Scripts.Menu
{
    [RequireComponent(typeof (MenuPlayerController))]
    public class MenuInputController : MonoBehaviour {
        internal int ControllerNumber = -1;
        internal bool UseXIndex = false;
        internal bool Computer = false;
        public PlayerIndex XIndex = PlayerIndex.One;
        public bool TapJump = true;
        public bool Vibration = true;
        public bool DPad = false;
        private int repeatFrames = 15;
        private int repeatCountdown = 0;
        private readonly string[] player = {"K", "J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9", "J10", "J11"};

        private MenuPlayerController playerController;

        private List<string> buttonsPressed; 
        private bool xActive = false;
        private bool yActive = false;
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

        public void Init(int number)
        {
            ControllerNumber = number;
        }

        void Awake()
        {
            repeatCountdown = repeatFrames;
            playerController = GetComponent<MenuPlayerController>();
            buttonsPressed = new List<string>();
            buttonsPressed.Add("A");
        }

        // Use this for initialization
        void Start () {
        
        }

        public void Deactivate()
        {
//            ControllerNumber = -1;
        }
        
        // Update is called once per frame
        void Update () {
            if (playerController.IsActive() && !Computer && !UseXIndex)
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal" + player[ControllerNumber]);
                y = CrossPlatformInputManager.GetAxis("Vertical" + player[ControllerNumber]);

                if (x == 0 && y == 0)
                {
                    repeatCountdown = repeatFrames;
                }

                // Check if axes have just been pressed
                if (Input.GetAxisRaw("Horizontal" + player[ControllerNumber]) != 0 || Input.GetAxisRaw("HorizontalDPad" + player[ControllerNumber]) != 0)
                {
                    if (!xActive)
                    {
                        if (Input.GetAxisRaw("Horizontal" + player[ControllerNumber]) < 0 ||
                            Input.GetAxisRaw("HorizontalDPad" + player[ControllerNumber]) < 0)
                        {
                            leftPressed = true;
                        }
                        else
                        {
                            rightPressed = true;
                        }
                        xActive = true;
                        repeatCountdown = repeatFrames;
                    }
                    else
                    {
                        if (repeatCountdown == 0)
                        {
                            xActive = false;
                        }
                        else
                        {
                            repeatCountdown--;
                        }
                    }
                }
                else
                {
                    xActive = false;
                }

                if (Input.GetAxisRaw("Vertical" + player[ControllerNumber]) != 0 || Input.GetAxisRaw("VerticalDPad" + player[ControllerNumber]) != 0)
                {
                    if (!yActive)
                    {
                        if (Input.GetAxisRaw("Vertical" + player[ControllerNumber]) < 0 || Input.GetAxisRaw("VerticalDPad" + player[ControllerNumber]) < 0)
                        {
                            downPressed = true;
                        }
                        else
                        {
                            upPressed = true;
                        }
                        yActive = true;
                        repeatCountdown = repeatFrames;
                    }
                    else
                    {
                        if (repeatCountdown == 0)
                        {
                            yActive = false;
                        }
                        else
                        {
                            repeatCountdown--;
                        }
                    }
                }
                else
                {
                    yActive = false;
                }

                // Check button presses
                if (upPressed)
                {
                    playerController.PressUp();
                    upPressed = false;
                }
                if (downPressed)
                {
                    playerController.PressDown();
                    downPressed = false;
                }
                if (leftPressed)
                {
                    playerController.PressLeft();
                    leftPressed = false;
                }
                if (rightPressed)
                {
                    playerController.PressRight();
                    rightPressed = false;
                }
                if (CrossPlatformInputManager.GetButtonDown("Jump" + player[ControllerNumber]))
                {
                }
                if (CrossPlatformInputManager.GetButtonDown("Run" + player[ControllerNumber]))
                {
                }
                if (CrossPlatformInputManager.GetButtonDown("Primary" + player[ControllerNumber]))
                {
                    playerController.PressPrimary();
                }
                if (CrossPlatformInputManager.GetButtonDown("Secondary" + player[ControllerNumber]))
                {
                    playerController.PressSecondary();
                }
                if (CrossPlatformInputManager.GetButtonDown("Start" + player[ControllerNumber]))
                {
                    playerController.PressStart();
                }
            }
            // XInput
            else if (playerController.IsActive() && !Computer && GamePad.GetState(XIndex).IsConnected)
            {
                GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
                x = gamePadState.ThumbSticks.Left.X;
                y = gamePadState.ThumbSticks.Left.Y;

                if (Mathf.Approximately(x, 0) && Mathf.Approximately(y, 0)) 
                {
                    repeatCountdown = repeatFrames;
                }

                // Check if axes have just been pressed
                if (!Mathf.Approximately(x, 0) || gamePadState.DPad.Right == ButtonState.Pressed || gamePadState.DPad.Left == ButtonState.Pressed)
                {
                    if (!xActive)
                    {
                        if (x < 0 || gamePadState.DPad.Left == ButtonState.Pressed)
                        {
                            leftPressed = true;
                        }
                        else if (x > 0 || gamePadState.DPad.Right == ButtonState.Pressed)
                        {
                            rightPressed = true;
                        }
                        xActive = true;
                        repeatCountdown = repeatFrames;
                    }
                    else
                    {
                        if (repeatCountdown == 0)
                        {
                            xActive = false;
                        }
                        else
                        {
                            repeatCountdown--;
                        }
                    }
                }
                else
                {
                    xActive = false;
                }

                if (!Mathf.Approximately(y, 0) || gamePadState.DPad.Up == ButtonState.Pressed || gamePadState.DPad.Down == ButtonState.Pressed)
                {
                    if (!yActive)
                    {
                        if (y < 0 || gamePadState.DPad.Down == ButtonState.Pressed)
                        {
                            downPressed = true;
                        }
                        else if (y > 0 || gamePadState.DPad.Up == ButtonState.Pressed)
                        {
                            upPressed = true;
                        }
                        yActive = true;
                        repeatCountdown = repeatFrames;
                    }
                    else
                    {
                        if (repeatCountdown == 0)
                        {
                            yActive = false;
                        }
                        else
                        {
                            repeatCountdown--;
                        }
                    }
                }
                else
                {
                    yActive = false;
                }

                // Check button presses
                if (upPressed)
                {
                    playerController.PressUp();
                    upPressed = false;
                }
                if (downPressed)
                {
                    playerController.PressDown();
                    downPressed = false;
                }
                if (leftPressed)
                {
                    playerController.PressLeft();
                    leftPressed = false;
                }
                if (rightPressed)
                {
                    playerController.PressRight();
                    rightPressed = false;
                }
                
                CleanActiveButtons();

                if (GetButtonDown("A"))
                {
                    playerController.PressPrimary();
                }
                if (GetButtonDown("B"))
                {
                    playerController.PressSecondary();
                }
                if (GetButtonDown("Start"))
                {
                    playerController.PressStart();
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

        private bool GetButtonDown(string button)
        {
            GamePadState gamePadState = GamePad.GetState(XIndex, GamePadDeadZone.Circular);
            if (buttonsPressed.Contains(button))
            {
                return false;
            }
            else
            {
                if (button == "A")
                {
                    if (gamePadState.Buttons.A == ButtonState.Pressed)
                    {
                        buttonsPressed.Add(button);
                        return true;
                    }
                }
                else if (button == "B")
                {
                    if (gamePadState.Buttons.B == ButtonState.Pressed)
                    {
                        buttonsPressed.Add(button);
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
            }
            return false;
        }

        public void ToggleTapJump()
        {
            TapJump = !TapJump;
        }

        public void ToggleVibration()
        {
            Vibration = !Vibration;
        }

        public void ToggleDPad()
        {
            DPad = !DPad;
        }

        public bool ButtonActive(string name)
        {
            if (!UseXIndex && !Computer)
            {
                return Input.GetButton(name + player[ControllerNumber]);
            }
            return false;
        }

        public bool AxisActive(string name)
        {
            if (!UseXIndex && !Computer)
            {
                if (Input.GetAxisRaw(name + player[ControllerNumber]) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AxisPositive(string name)
        {
            if (!UseXIndex && !Computer)
            {
                if (Input.GetAxisRaw(name + player[ControllerNumber]) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AxisNegative(string name)
        {
            if (!UseXIndex && !Computer)
            {
                if (Input.GetAxisRaw(name + player[ControllerNumber]) < 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void VibrateController(float leftMotor, float rightMotor)
        {
            if (UseXIndex && Vibration)
            {
                GamePad.SetVibration(XIndex, leftMotor, rightMotor);
            }
        }

        public void StopVibration()
        {
            if (UseXIndex && Vibration)
            {
                GamePad.SetVibration(XIndex, 0f, 0f);
            }
        }
    }
}