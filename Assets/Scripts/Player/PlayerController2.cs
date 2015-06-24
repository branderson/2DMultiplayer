﻿using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Player.States;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerController2 : MonoBehaviour
    {
        [SerializeField] internal float maxSpeedX = 8f; // The fastest the player can travel in the x axis.
        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool airControl = true; // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
        [SerializeField] private Transform groundCheck; // A position marking where to check if the player is on the ground
        [SerializeField] private Transform ceilingCheck; // A position marking where to check for ceilings
        [SerializeField] private float jumpHeight = 4f; // Height of single jump from flat ground
        [SerializeField] private float airJumpHeight = 4f; // Height of air jump
        [SerializeField] private float neutralAirTime = .68f; // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        [SerializeField] private float terminalVelocity = -20f; // Maximum regular falling rate
        [SerializeField] private float terminalVelocityFast = -30f;
        [SerializeField] private float fastFallFactor = 1.4f; // Velocity multiplier for fast fall

        private PlayerState currentPlayerState;
        internal Animator animator; // Reference to the player's animator component.
        private Rigidbody2D rigidBody; // Reference to the player's Rigidbody2D component

        private const float GroundedRadius = .1f; // Radius of the overlap circle to determine if onGround
        private const float CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        
        // Inspector debug values that are never used. Turns off warnings
//        #pragma warning disable 0168
        private float speedX;
        private float speedY;
//        #pragma warning restore 0168

        private bool onGround;

        // TODO: Set up a set of flags that are internal, which determine behaviour, everything else is private
        internal bool canFall;
        internal bool fastFall;
        internal bool facingRight; // For determining which way the player is currently facing
        internal float jumpSpeed { get; set; }
        internal float airJumpSpeed { get; set; }
        internal bool canAirJump;
        private float gravity; // Rate per second of decreasing vertical speed

        // TODO: Do I need to initialize in a constructor or can I initialize in the class itself?
        public PlayerController2()
        {
            this.canFall = true;
            this.facingRight = true;
            this.canAirJump = true;
        }

        // Use this for initialization
        private void Awake()
        {
            // Setting up references.
            groundCheck = transform.Find("GroundCheck");
            ceilingCheck = transform.Find("CeilingCheck");
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody2D>();
            jumpSpeed = 4*jumpHeight/neutralAirTime;
            gravity = -2*jumpSpeed/neutralAirTime;
            airJumpSpeed = (float) Math.Sqrt(-2*gravity*airJumpHeight);
        }

        // Update is called once per frame
        private void Update()
        {
            speedX = rigidBody.velocity.x;
            speedY = rigidBody.velocity.y;
//            print(currentPlayerState.GetName());
        }

        private void FixedUpdate()
        {
//            CheckForGround();
            if (canFall)
                if (fastFall)
                    FallFast();
                else
                    FallRegular();
            animator.SetFloat("xVelocity", speedX);
            animator.SetFloat("yVelocity", speedY);
            animator.SetFloat("xSpeed", Mathf.Abs(speedX));
            animator.SetFloat("ySpeed", Mathf.Abs(speedY));
        }

        private void FallRegular()
        {
            // Handles acceleration due to gravity
            if (rigidBody.velocity.y > terminalVelocity)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y + gravity*Time.fixedDeltaTime);
            }
            // Caps terminal velocity
            if (rigidBody.velocity.y < terminalVelocity)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, terminalVelocity);
            }
        }

        private void FallFast()
        {
            if (rigidBody.velocity.y > terminalVelocityFast)
            {
                SetVelocityY(rigidBody.velocity.y + gravity*fastFallFactor*Time.fixedDeltaTime);
            }
            if (rigidBody.velocity.y < terminalVelocityFast)
            {
                SetVelocityY(terminalVelocityFast);
            }
        }

        public bool CheckForGround()
        {
            onGround = false;
            animator.SetBool("Ground", false);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedRadius, groundLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    onGround = true;
                    canAirJump = true;
                    fastFall = false;
                    animator.SetBool("Ground", true);
                }
            }
            return onGround;
        }

        public void SetState(PlayerState state)
        {
            currentPlayerState = state;
        }

        public PlayerState GetState()
        {
            return currentPlayerState;
        }

        public void SetVelocity(Vector2 velocity)
        {
            rigidBody.velocity = velocity;
        }

        public void SetVelocity(float x, float y)
        {
            rigidBody.velocity = new Vector2(x, y);
        }

        public void SetVelocityX(float x)
        {
            rigidBody.velocity = new Vector2(x, rigidBody.velocity.y);
        }

        public void SetVelocityY(float y)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, y);
        }

        public void Flip()
        {
            // Switch the way the player is labelled as facing.
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}