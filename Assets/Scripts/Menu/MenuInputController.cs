using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using XInputDotNetPure;

namespace Assets.Scripts.Menu
{
    [RequireComponent(typeof (MenuPlayerController))]
    public class MenuInputController : MonoBehaviour {
        internal int ControllerNumber = -1;
        internal bool UseXIndex = false;
        internal bool Computer = false;
        [SerializeField] public int XIndex = -1;
        [SerializeField] public bool TapJump = true;
        [SerializeField] public bool Vibration = true;
        [SerializeField] public bool DPad = false;
        private int repeatFrames = 15;
        private int repeatCountdown = 0;
        private readonly string[] player = {"K", "J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9", "J10", "J11"};

        private MenuPlayerController playerController;

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
            if (playerController.IsActive() && ControllerNumber >= 0)
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
            return Input.GetButton(name + player[ControllerNumber]);
        }

        public bool AxisActive(string name)
        {
            if (Input.GetAxisRaw(name + player[ControllerNumber]) != 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisPositive(string name)
        {
            if (Input.GetAxisRaw(name + player[ControllerNumber]) > 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisNegative(string name)
        {
            if (Input.GetAxisRaw(name + player[ControllerNumber]) < 0)
            {
                return true;
            }
            return false;
        }

        public void VibrateController(float leftMotor, float rightMotor)
        {
            if (XIndex != -1 && Vibration)
            {
                GamePad.SetVibration((PlayerIndex) XIndex, leftMotor, rightMotor);
            }
        }

        public void StopVibration()
        {
            if (XIndex != -1 && Vibration)
            {
                GamePad.SetVibration((PlayerIndex) XIndex, 0f, 0f);
            }
        }
    }
}