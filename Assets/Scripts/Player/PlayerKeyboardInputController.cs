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
    public class PlayerKeyboardInputController : MonoBehaviour, IInputController
    {
        [SerializeField] public bool TapJump = true;
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
        private string horizontalString = "HorizontalK";
        private string verticalString = "VerticalK";

        public void Init(MenuInputController menuInputController)
        {
            TapJump = menuInputController.TapJump;
        }

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            activeHeldControls = new List<byte>();
            activePressedControls = new List<byte>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            x = CrossPlatformInputManager.GetAxis(horizontalString);
            y = CrossPlatformInputManager.GetAxis(verticalString);

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
            if (Mathf.Abs(Input.GetAxisRaw(horizontalString)) > .1)
            {
                if (xActiveFrames < 3)
                {
                    if (Input.GetAxisRaw(horizontalString) < -thresholdX) // < 0)
                    {
                        if (!ButtonActive("TiltLock"))
                        {
                            leftPressed = true;
                        }
                    }
                    else if (Input.GetAxisRaw(horizontalString) > thresholdX)
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

            if (Mathf.Abs(Input.GetAxisRaw(verticalString)) > .1)
            {
                if (yActiveFrames < 3)
                {
                    if (Input.GetAxisRaw(verticalString) < -thresholdY)
                    {
                        if (!ButtonActive("TiltLock"))
                        {
                            downPressed = true;
                        }
                    }
                    else if (Input.GetAxisRaw(verticalString) > thresholdY)
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
                jump = CrossPlatformInputManager.GetButtonDown("JumpK");
            }

            run = CrossPlatformInputManager.GetButton("RunK");

            if (!primary)
            {
                primary = CrossPlatformInputManager.GetButtonDown("PrimaryK");
            }

            if (!secondary)
            {
                secondary = CrossPlatformInputManager.GetButtonDown("SecondaryK");
            }

            if (!block)
            {
                block = CrossPlatformInputManager.GetButtonDown("BlockK");
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
            return Input.GetButton(name + "K");
        }

        public float GetAxis(string name)
        {
            return Input.GetAxisRaw(name + "K");
        }

        public bool AxisActive(string name)
        {
            if (Input.GetAxisRaw(name + "K") != 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisPositive(string name)
        {
            if (Input.GetAxisRaw(name + "K") > 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisNegative(string name)
        {
            if (Input.GetAxisRaw(name + "K") < 0)
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
            float xCheck = Input.GetAxisRaw(horizontalString);
            float yCheck = Input.GetAxisRaw(verticalString);

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
            activeHeldControls.Clear();
            if (ButtonActive("Primary"))
            {
                activeHeldControls.Add(0);
            }
            if (ButtonActive("Secondary"))
            {
                activeHeldControls.Add(1);
            }
            if (ButtonActive("Jump"))
            {
                activeHeldControls.Add(2);
            }
            if (ButtonActive("Block"))
            {
                activeHeldControls.Add(4);
            }
            if (ButtonActive("Grab"))
            {
                activeHeldControls.Add(5);
            }
            if (ButtonActive("Run"))
            {
                activeHeldControls.Add(7);
            }
            if (ButtonActive("TiltLock"))
            {
                activeHeldControls.Add(6);
            }
            activePressedControls.Clear();
            return activeHeldControls;
        }
    }
}