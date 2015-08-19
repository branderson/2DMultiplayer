using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menu;
using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using XInputDotNetPure;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof (PlayerController))]
    public class PlayerDirectInputController : MonoBehaviour, IInputController
    {
        [SerializeField] public int ControllerNumber = 0;
        [SerializeField] public bool TapJump = true;
        [SerializeField] public bool Vibration = true;
        [SerializeField] public bool DPad = false;
        private PlayerController playerController;
        private int xActiveFrames = 0;
        private int yActiveFrames = 0;

        private const float thresholdX = .5f;
        private const float thresholdY = .5f;

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
        private string horizontalString = "Horizontal";
        private string verticalString = "Vertical";

        private readonly string[] player = {"J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9", "J10", "J11"};

        public void Init(MenuInputController menuInputController)
        {
            ControllerNumber = menuInputController.ControllerNumber;
            TapJump = menuInputController.TapJump;
            Vibration = menuInputController.Vibration;
            DPad = menuInputController.DPad;
            if (DPad)
            {
                horizontalString = "HorizontalDPad";
                verticalString = "VerticalDPad";
            }
            else
            {
                horizontalString = "Horizontal";
                verticalString = "Vertical";
            }
        }

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            activeHeldControls = new List<byte>();
        }

        private void Start()
        {
//            print(ControllerNumber);
//            print(XIndex);
//            print("");
        }

        private void Update()
        {
            x = CrossPlatformInputManager.GetAxis(horizontalString + player[ControllerNumber]);
            y = CrossPlatformInputManager.GetAxis(verticalString + player[ControllerNumber]);

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
            if (Mathf.Abs(Input.GetAxisRaw(horizontalString + player[ControllerNumber])) > .1)
            {
                if (xActiveFrames < 3)
                {
                    if (Input.GetAxisRaw(horizontalString + player[ControllerNumber]) < -thresholdX) // < 0)
                    {
                        if (!ButtonActive("TiltLock"))
                        {
                            leftPressed = true;
                        }
                    }
                    else if (Input.GetAxisRaw(horizontalString + player[ControllerNumber]) > thresholdX)
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

            if (Mathf.Abs(Input.GetAxisRaw(verticalString + player[ControllerNumber])) > .1)
            {
                if (yActiveFrames < 3)
                {
                    if (Input.GetAxisRaw(verticalString + player[ControllerNumber]) < -thresholdY)
                    {
                        if (!ButtonActive("TiltLock"))
                        {
                            downPressed = true;
                        }
                    }
                    else if (Input.GetAxisRaw(verticalString + player[ControllerNumber]) > thresholdY)
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

            // Check button presses
            if (!jump)
            {
                jump = CrossPlatformInputManager.GetButtonDown("Jump" + player[ControllerNumber]);
            }

            run = CrossPlatformInputManager.GetButton("Run" + player[ControllerNumber]);

            if (!primary)
            {
                primary = CrossPlatformInputManager.GetButtonDown("Primary" + player[ControllerNumber]);
            }

            if (!secondary)
            {
                secondary = CrossPlatformInputManager.GetButtonDown("Secondary" + player[ControllerNumber]);
            }

            if (!block)
            {
                block = CrossPlatformInputManager.GetButtonDown("Block" + player[ControllerNumber]);
            }

            if (!grab)
            {
                //                grab = CrossPlatformInputManager.GetButtonDown("Grab" + player[ControllerNumber]);
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
            else
            {
                if (playerController.animator.GetCurrentAnimatorClipInfo(0).Count() != 0)
                {
//                    print("The current state is not implemented: " +
//                          playerController.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                }
//                print("The current state is not implemented");
            }
        }

        public bool ButtonActive(string name)
        {
            return Input.GetButton(name + player[ControllerNumber]);
        }

        public float GetAxis(string name)
        {
            if (name == "Vertical")
            {
                name = verticalString;
            }
            else if (name == "Horizontal")
            {
                name = horizontalString;
            }
            return Input.GetAxisRaw(name + player[ControllerNumber]);
        }

        public bool AxisActive(string name)
        {
            if (name == "Vertical")
            {
                name = verticalString;
            }
            else if (name == "Horizontal")
            {
                name = horizontalString;
            }
            if (Input.GetAxisRaw(name + player[ControllerNumber]) != 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisPositive(string name)
        {
            if (name == "Vertical")
            {
                name = verticalString;
            }
            else if (name == "Horizontal")
            {
                name = horizontalString;
            }
            if (Input.GetAxisRaw(name + player[ControllerNumber]) > 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisNegative(string name)
        {
            if (name == "Vertical")
            {
                name = verticalString;
            }
            else if (name == "Horizontal")
            {
                name = horizontalString;
            }
            if (Input.GetAxisRaw(name + player[ControllerNumber]) < 0)
            {
                return true;
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
            return TapJump;
        }
//
//        public List<byte> ControllerButtonPressState()
//        {
//            throw new NotImplementedException();
//        }

        public sbyte[] ControllerAnalogState()
        {
            sbyte[] analogState = {0, 0};
            float xCheck = Input.GetAxisRaw(horizontalString + player[ControllerNumber]);
            float yCheck = Input.GetAxisRaw(verticalString + player[ControllerNumber]);

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
            // TODO: This needs to be cleaned up
            // TODO: Also, buttons need to be properly assigned for keyboard and direct input
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