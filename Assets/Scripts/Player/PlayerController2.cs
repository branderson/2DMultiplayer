using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Player.States;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerController2 : MonoBehaviour
    {
        [SerializeField] private float maxSpeedX = 8f; // The fastest the player can travel in the x axis.
        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool airControl = true; // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
        [SerializeField] private float jumpHeight = 4f; // Height of single jump from flat ground
        [SerializeField] private float airJumpHeight = 4f; // Height of air jump
        [SerializeField] private float neutralAirTime = .68f; // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        [SerializeField] private float terminalVelocity = -20f; // Maximum regular falling rate
        [SerializeField] private float fastFallFactor = 1.4f; // Velocity multiplier for fast fall

        internal PlayerState currentPlayerState;
        internal Animator animator; // Reference to the player's animator component.
        public Transform groundCheck; // A position marking where to check if the player is on the ground
        public Transform ceilingCheck; // A position marking where to check for ceilings
        internal Rigidbody2D rigidBody; // Reference to the player's Rigidbody2D component

        private const float GroundedRadius = .1f; // Radius of the overlap circle to determine if onGround
        private const float CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        
        // Inspector debug values that are never used. Turns off warnings
        #pragma warning disable 0168
        private float speedX;
        private float speedY;
        #pragma warning restore 0168

        internal bool onGround { get; set; }

        internal bool canFall;
        internal bool facingRight; // For determining which way the player is currently facing
        internal float jumpSpeed { get; set; }
        internal float airJumpSpeed { get; set; }
        internal bool canAirJump { get; set; }
        internal float gravity; // Rate per second of decreasing vertical speed

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
            onGround = false;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedRadius, groundLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    onGround = true;
                    canAirJump = true;
                    animator.SetBool("Ground", true);
                }
            }

            if (canFall) // && !fastFall
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
            animator.SetFloat("hSpeed", speedX);
            animator.SetFloat("vSpeed", speedY);
        }
    }
}