using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using XInputDotNetPure;

namespace Assets.Scripts.Menu
{
    public class MenuInput : MonoBehaviour {
        [SerializeField] public int playerNumber = 0;
        [SerializeField] public int XIndex;
        [SerializeField] public bool TapJump = true;
        [SerializeField] public bool Vibration = true;
        private readonly string[] player = {"K", "J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9", "J10", "J11"};

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

        // Use this for initialization
        void Start () {
        
        }
        
        // Update is called once per frame
        void Update () {
            x = CrossPlatformInputManager.GetAxis("Horizontal" + player[playerNumber]);
            y = CrossPlatformInputManager.GetAxis("Vertical" + player[playerNumber]);

            // Check if axes have just been pressed
            if (Input.GetAxisRaw("Horizontal" + player[playerNumber]) != 0)
            {
                if (!xActive)
                {
                    if (Input.GetAxisRaw("Horizontal" + player[playerNumber]) < 0)
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

            if (Input.GetAxisRaw("Vertical" + player[playerNumber]) != 0)
            {
                if (!yActive)
                {
                    if (Input.GetAxisRaw("Vertical" + player[playerNumber]) < 0)
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
            if (!jump)
            {
                jump = CrossPlatformInputManager.GetButtonDown("Jump" + player[playerNumber]);
            }

            run = CrossPlatformInputManager.GetButton("Run" + player[playerNumber]);

            if (!primary)
            {
                primary = CrossPlatformInputManager.GetButtonDown("Primary" + player[playerNumber]);
            }

            if (!secondary)
            {
                secondary = CrossPlatformInputManager.GetButtonDown("Secondary" + player[playerNumber]);
            }
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