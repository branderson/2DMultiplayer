using System;
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
        private bool jump = false;

        private const float thresholdX = 0f;
        private const float thresholdY = 1f;

        private float x = 0f;
        private float y = 0f;

        private void Awake()
        {
            character = GetComponent<PlayerController2>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            x = CrossPlatformInputManager.GetAxis("Horizontal");
            y = CrossPlatformInputManager.GetAxis("Vertical");
            
            // Remove noise
            if (Mathf.Abs(x) < thresholdX)
                x = 0;
            if (Mathf.Abs(y) < thresholdY)
                y = 0;
            
            if (!jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        private void FixedUpdate()
        {
            if (character.GetState() != null)
                character.GetState().Move(x, y);
            if (jump)
            {
                print("Pressing jump");
                character.GetState().Jump();
            }
            jump = false;
        }
    }
}