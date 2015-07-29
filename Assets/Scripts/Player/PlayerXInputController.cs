using System;
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
        internal PlayerIndex XIndex;
        private PlayerController character;
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

        private readonly string[] player = {"K", "J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9", "J10", "J11"};

        public void Init(MenuInputController menuInputController)
        {
            XIndex = menuInputController.XIndex;
            TapJump = menuInputController.TapJump;
            Vibration = menuInputController.Vibration;
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
            }
        }

        public bool ButtonActive(string name)
        {
//            return Input.GetButton(name + player[ControllerNumber]);
            return false;
        }

        public bool AxisActive(string name)
        {
//            if (Input.GetAxisRaw(name + player[ControllerNumber]) != 0)
//            {
//                return true;
//            }
            return false;
        }

        public bool AxisPositive(string name)
        {
//            if (Input.GetAxisRaw(name + player[ControllerNumber]) > 0)
//            {
//                return true;
//            }
            return false;
        }

        public bool AxisNegative(string name)
        {
//            if (Input.GetAxisRaw(name + player[ControllerNumber]) < 0)
//            {
//                return true;
//            }
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

//        public new string GetType()
//        {
//            return "XInputController";
//        }
    }
}