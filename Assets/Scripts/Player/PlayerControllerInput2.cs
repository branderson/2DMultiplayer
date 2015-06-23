using System;
using Assets.Scripts.Player.States;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts
{
    [RequireComponent(typeof (PlayerController2))]
    public class PlayerControllerInput2 : MonoBehaviour
    {
        private PlayerController2 character;
        private Animator animator;
        private PlayerState currentPlayerState;
        private bool jump;


        private void Awake()
        {
            character = GetComponent<PlayerController2>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            currentPlayerState = animator.GetBehaviour<PlayerState>();
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
                currentPlayerState.Jump();
            jump = false;
        }
    }
}