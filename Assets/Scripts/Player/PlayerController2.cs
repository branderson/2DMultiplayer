using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Player.States;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerController2 : MonoBehaviour
    {
        [SerializeField] private const float maxSpeedX = 8f; // The fastest the player can travel in the x axis.
        [Range(0, 1)] [SerializeField] private const float crouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private const bool airControl = true; // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
        [SerializeField] private const float jumpHeight = 4f; // Height of single jump from flat ground
        [SerializeField] private const float airJumpHeight = 4f; // Height of air jump
        [SerializeField] private const float neutralAirTime = .68f; // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        [SerializeField] private const float terminalVelocity = -20f; // Maximum regular falling rate
        [SerializeField] private const float fastFallFactor = 1.4f; // Velocity multiplier for fast fall

        private PlayerState currentPlayerState;
        private Animator anim; // Reference to the player's animator component.
        private Transform groundCheck; // A position marking where to check if the player is grounded.
        private Transform ceilingCheck; // A position marking where to check for ceilings
        private Rigidbody2D rigidbody; // Reference to the player's Rigidbody2D component
        private float speedX;
        private float speedY;
        private const float GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
        private bool grounded; // Whether or not the player is grounded.
        private const float CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private bool facingRight; // For determining which way the player is currently facing.
        private float jumpSpeed;
        private float airJumpSpeed;
        private bool canAirJump;
        private float gravity; // Rate per second of decreasing vertical speed

        // TODO: Do I need to initialize in a constructor or can I initialize in the class itself?
        public PlayerController2()
        {
            this.facingRight = true;
            this.canAirJump = true;
        }

        // Use this for initialization
        private void Awake()
        {
            // Setting up references.
            groundCheck = transform.Find("GroundCheck");
            ceilingCheck = transform.Find("CeilingCheck");
            anim = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody2D>();
            jumpSpeed = 4*jumpHeight/neutralAirTime;
            gravity = -2*jumpSpeed/neutralAirTime;
            airJumpSpeed = (float) Math.Sqrt(-2*gravity*airJumpHeight);
        }

        private void Start()
        {
            currentPlayerState = anim.GetBehaviour<PlayerState>();
            currentPlayerState.playerController = this;
        }

        // Update is called once per frame
        private void Update()
        {
            currentPlayerState = anim.GetBehaviour<PlayerState>();
//            currentPlayerState.playerController = this;
            print(currentPlayerState.GetName());
        }
    }
}