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
        [SerializeField] public int ControllerNumber = 0;
        [SerializeField] public bool TapJump = true;
        [SerializeField] public bool Vibration = true;
        internal int XIndex;
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

        private int frame = 0;

        private void Awake()
        {
            character = GetComponent<PlayerController>();
        }

        public void Init(Menu.MenuInputController menuInputController)
        {
            ControllerNumber = menuInputController.ControllerNumber;
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
                character.run = run;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void StopVibration()
        {
            throw new NotImplementedException();
        }
    }
}
