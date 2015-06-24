using System;
using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts
{
    [RequireComponent(typeof (PlayerController2))]
    public class PlayerControllerInput2 : MonoBehaviour
    {
        private PlayerController2 character;
        private bool jump = false;


        private void Awake()
        {
            character = GetComponent<PlayerController2>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (!jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

//            currentPlayerState.Move(h, v);
            if (jump)
            {
                character.currentPlayerState.Jump();
            }
            jump = false;
        }
    }
}