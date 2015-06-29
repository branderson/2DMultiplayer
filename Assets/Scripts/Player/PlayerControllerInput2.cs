﻿using System;
using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof (PlayerController2))]
    public class PlayerControllerInput2 : MonoBehaviour
    {
        private PlayerController2 character;
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
        private bool action1 = false;
        private bool action2 = false;

        private void Awake()
        {
            character = GetComponent<PlayerController2>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            x = CrossPlatformInputManager.GetAxis("Horizontal");
            y = CrossPlatformInputManager.GetAxis("Vertical");

            // Check if axes have just been pressed
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (!xActive)
                {
                    if (Input.GetAxisRaw("Horizontal") < 0)
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

            if (Input.GetAxisRaw("Vertical") != 0)
            {
                if (!yActive)
                {
                    if (Input.GetAxisRaw("Vertical") < 0)
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
            
            // Remove noise
            if (Mathf.Abs(x) < thresholdX)
                x = 0;
            if (Mathf.Abs(y) < thresholdY)
                y = 0;
            
            // Check button presses
            if (!jump)
            {
                jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!action1)
            {
                action1 = CrossPlatformInputManager.GetButtonDown("Action1");
            }

            if (!action2)
            {
                action2 = CrossPlatformInputManager.GetButtonDown("Action2");
            }

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
//                if (leftPressed)
//                {
//                    character.GetState().Left();
//                    leftPressed = false;
//                }
//                if (rightPressed)
//                {
//                    character.GetState().Right();
//                    rightPressed = false;
//                }
                if (jump)
                {
                    character.GetState().Jump();
                    jump = false;
                }
                if (action1)
                {
                    character.GetState().Action1(x, y);
                    action1 = false;
                }
                if (action2)
                {
                    character.GetState().Action2(x, y);
                    action1 = false;
                }
            }
        }

        public bool ButtonActive(string name)
        {
            return Input.GetButton(name);
        }

        public bool AxisActive(string name)
        {
            if (Input.GetAxisRaw(name) != 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisPositive(string name)
        {
            if (Input.GetAxisRaw(name) > 0)
            {
                return true;
            }
            return false;
        }

        public bool AxisNegative(string name)
        {
            if (Input.GetAxisRaw(name) < 0)
            {
                return true;
            }
            return false;
        }
    }
}