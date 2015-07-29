using System;
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
    public class PlayerInputController : MonoBehaviour, IInputController
    {
        [SerializeField] public int ControllerNumber = 0;
        [SerializeField] public bool TapJump = true;
        [SerializeField] public bool Vibration = true;
        [SerializeField] public bool DPad = false;
        internal int XIndex;
        private PlayerController character;
        private int xActiveFrames = 0;
        private int yActiveFrames = 0;
        private bool xInactiveFrame = false;
        private bool yInactiveFrame = false;

        private const float thresholdX = .5f;
        private const float thresholdY = .5f;

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

        private readonly string[] player = {"K", "J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9", "J10", "J11"};

        public void Init(MenuInputController menuInputController)
        {
            ControllerNumber = menuInputController.ControllerNumber;
            XIndex = menuInputController.XIndex; // TODO: Integrate XIndices into MenuInputController
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
            character = GetComponent<PlayerController>();
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
            // Execute character moves
            if (character.GetState() != null)
            {
                character.GetState().Move(x, y);
                if (upPressed)
                {
                    character.GetState().Up();
                    upPressed = false;
                }
                if (downPressed)
                {
                    character.GetState().Down();
                    downPressed = false;
                }
                if (leftPressed)
                {
                    character.GetState().Left();
                    leftPressed = false;
                }
                if (rightPressed)
                {
                    character.GetState().Right();
                    rightPressed = false;
                }
                if (jump)
                {
                    character.GetState().Jump();
                    jump = false;
                }
                character.Run = run;
                if (primary)
                {
                    character.GetState().Primary(x, y);
                    primary = false;
                }
                if (secondary)
                {
                    character.GetState().Secondary(x, y);
                    secondary = false;
                }
                if (block)
                {
                    character.GetState().Block();
                    block = false;
                }
                if (grab)
                {
                    character.GetState().Grab();
                    grab = false;
                }
            }
            else
            {
                if (character.animator.GetCurrentAnimatorClipInfo(0).Count() != 0)
                {
                    print("The current state is not implemented: " +
                          character.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                }
                print("The current state is not implemented");
            }
        }

        public bool ButtonActive(string name)
        {
            return Input.GetButton(name + player[ControllerNumber]);
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
//            print("Trying to vibrate " + (PlayerIndex)(ControllerNumber));
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

        public bool GetTapJump()
        {
            return TapJump;
        }


//        public new string GetType()
//        {
//            return "DirectInputController";
//        }
    }
}