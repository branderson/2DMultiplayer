using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using XInputDotNetPure;

namespace Assets.Scripts.Menu
{
    [RequireComponent(typeof (MenuPlayerController))]
    public class MenuControllerInput : MonoBehaviour {
        [SerializeField] public int playerNumber = 0;
        [SerializeField] public int XIndex = -1;
        [SerializeField] public bool TapJump = true;
        [SerializeField] public bool Vibration = true;
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
            playerNumber = number;
        }

        void Awake()
        {
            playerController = GetComponent<MenuPlayerController>();
        }

        // Use this for initialization
        void Start () {
        
        }
        
        // Update is called once per frame
        void Update () {
            if (playerController.IsActive())
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal" + player[playerNumber]);
                y = CrossPlatformInputManager.GetAxis("Vertical" + player[playerNumber]);

                // Check if axes have just been pressed
                if (Input.GetAxisRaw("Horizontal" + player[playerNumber]) != 0 || Input.GetAxisRaw("HorizontalDPad" + player[playerNumber]) != 0)
                {
                    if (!xActive)
                    {
                        if (Input.GetAxisRaw("Horizontal" + player[playerNumber]) < 0 || Input.GetAxisRaw("HorizontalDPad" + player[playerNumber]) < 0)
                        {
                            leftPressed = true;
                        }
                        else
                        {
                            rightPressed = true;
                        }
                        xActive = true;
                    }
                }
                else
                {
                    xActive = false;
                }

                if (Input.GetAxisRaw("Vertical" + player[playerNumber]) != 0 || Input.GetAxisRaw("VerticalDPad" + player[playerNumber]) != 0)
                {
                    if (!yActive)
                    {
                        if (Input.GetAxisRaw("Vertical" + player[playerNumber]) < 0 || Input.GetAxisRaw("VerticalDPad" + player[playerNumber]) < 0)
                        {
                            downPressed = true;
                        }
                        else
                        {
                            upPressed = true;
                        }
                        yActive = true;
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
                if (CrossPlatformInputManager.GetButtonDown("Jump" + player[playerNumber]))
                {
                }
                if (CrossPlatformInputManager.GetButtonDown("Run" + player[playerNumber]))
                {
                }
                if (CrossPlatformInputManager.GetButtonDown("Primary" + player[playerNumber]))
                {
                    playerController.PressPrimary();
                }
                if (CrossPlatformInputManager.GetButtonDown("Secondary" + player[playerNumber]))
                {
                    playerController.PressSecondary();
                }
                if (CrossPlatformInputManager.GetButtonDown("Start" + player[playerNumber]))
                {
                    playerController.PressStart();
                }
                //            if (CrossPlatformInputManager.GetButtonDown("Restart" + player[playerNumber]))
                //            {
                //                playerController.PressRestart();
                //            }
                //            if (!jump)
                //            {
                //                jump = CrossPlatformInputManager.GetButtonDown("Jump" + player[playerNumber]);
                //            }
                //
                //            run = CrossPlatformInputManager.GetButton("Run" + player[playerNumber]);
                //
                //            if (!primary)
                //            {
                //                primary = CrossPlatformInputManager.GetButtonDown("Primary" + player[playerNumber]);
                //            }
                //
                //            if (!secondary)
                //            {
                //                secondary = CrossPlatformInputManager.GetButtonDown("Secondary" + player[playerNumber]);
                //            }
            }
        }

        public void SetTapJump(bool value)
        {
            TapJump = value;
        }

        public void SetVibration(bool value)
        {
            Vibration = value;
        }

        public bool ButtonActive(string name)
        {
            return Input.GetButton(name + player[playerNumber]);
        }

        public bool AxisActive(string name)
        {
            if (Input.GetAxisRaw(name + player[playerNumber]) != 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisPositive(string name)
        {
            if (Input.GetAxisRaw(name + player[playerNumber]) > 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisNegative(string name)
        {
            if (Input.GetAxisRaw(name + player[playerNumber]) < 0)
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