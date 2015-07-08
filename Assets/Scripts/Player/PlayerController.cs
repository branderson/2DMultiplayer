using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Player.States;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] internal float maxSpeedX = 8f; // The fastest the player can travel in the x axis.
        [SerializeField] internal float runSpeedX = 14f; // The speed that the player runs
        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
        [SerializeField] private Transform groundCheck; // A position marking where to check if the player is on the ground
        [SerializeField] private Transform ceilingCheck; // A position marking where to check for ceilings
        [SerializeField] private float jumpHeight = 6f; // Height of single jump from flat ground
        [SerializeField] private float airJumpHeight = 4f; // Height of air jump
        [SerializeField] private float sideJumpHeight = 6f; // Height of side jump
        [SerializeField] private float sideJumpDistance = 5f; // Horizontal distance of side jump
        [SerializeField] private float airSideJumpHeight = 4f;
        [SerializeField] private float airSideJumpDistance = 4f;
        [SerializeField] private float recoveryHeight = 8f;
        [SerializeField] private float neutralAirTime = .68f; // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        [SerializeField] private float terminalVelocity = -15f; // Maximum regular falling rate
        [SerializeField] private float terminalVelocityFast = -30f; // Fast fall terminal velocity
        [SerializeField] private float fastFallFactor = 3f; // Velocity multiplier for fast fall
        [SerializeField] public float airControlSpeed = .5f; // Fraction of horizontal control while in air
        [SerializeField] public float shortHopFactor = .5f; // Fraction of neutral jump height/distance for short hop

        private PlayerState currentPlayerState;
        internal Animator animator; // Reference to the player's animator component.
        private Rigidbody2D rigidBody; // Reference to the player's Rigidbody2D component
        private PlayerControllerInput input;

        private const float GroundedRadius = .1f; // Radius of the overlap circle to determine if onGround
        private const float CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        
        internal float speedX;
        internal float speedY;

        private bool onGround;
        internal bool run;

        // TODO: Set up a set of flags that are internal, which determine behaviour, everything else is private
        internal bool canFall;
        internal bool fastFall;
        internal bool facingRight; // For determining which way the player is currently facing
        internal float jumpSpeed { get; set; }
        internal float airJumpSpeed { get; set; }
        internal float sideJumpSpeedX;
        internal float sideJumpSpeedY;
        internal float airSideJumpSpeedX;
        internal float airSideJumpSpeedY;
        internal float recoverySpeed;
        internal float maxAirSpeedX;

        internal bool canAirJump;
        internal bool canRecover;
        private float gravity; // Rate per second of decreasing vertical speed
        internal bool timedVibrate = false;
        internal int vibrate;
        internal float leftIntensity;
        internal float rightIntensity;

        // TODO: Do I need to initialize in a constructor or can I initialize in the class itself?
        public PlayerController()
        {
            this.canFall = true;
            this.facingRight = true;
            this.canAirJump = true;
            this.canRecover = true;
            this.timedVibrate = false;
            this.vibrate = 0;
            this.leftIntensity = 0f;
            this.rightIntensity = 0f;
        }

        // Use this for initialization
        private void Awake()
        {
            // Setting up references.
            groundCheck = transform.Find("GroundCheck");
            ceilingCheck = transform.Find("CeilingCheck");
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerControllerInput>();
            CalculatePhysics();
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
            if (canFall && !onGround)
                if (fastFall)
                    FallFast();
                else
                    FallRegular();
//            else if (onGround)
//            {
//                SetVelocityY(0f);
//            }
            animator.SetFloat("xVelocity", speedX);
            animator.SetFloat("yVelocity", speedY);
            animator.SetFloat("xSpeed", Mathf.Abs(speedX));
            animator.SetFloat("ySpeed", Mathf.Abs(speedY));
            animator.SetBool("Run", run);
            animator.SetBool("CanAirJump", canAirJump);
            animator.SetBool("CanRecover", canRecover);
            if (timedVibrate)
            {
                Vibrate();
            }
        }

        private void CalculatePhysics()
        {
            jumpSpeed = 4*jumpHeight/neutralAirTime;
            gravity = -2*jumpSpeed/neutralAirTime;
            airJumpSpeed = (float) Math.Sqrt(-2*gravity*airJumpHeight);
            sideJumpSpeedY = (float) Math.Sqrt(-2*gravity*sideJumpHeight);
            sideJumpSpeedX = sideJumpDistance/(neutralAirTime); // TODO: Should be sideJumpDistance/calculated time of vertical side jump in air
            airSideJumpSpeedY = (float) Math.Sqrt(-2*gravity*airSideJumpHeight);
            airSideJumpSpeedX = airSideJumpDistance/neutralAirTime; // TODO: Same as above
            recoverySpeed = (float) Math.Sqrt(-2*gravity*recoveryHeight);
            maxAirSpeedX = airSideJumpSpeedX*2;
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
            bool grounded = false;
            animator.SetBool("Ground", false);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedRadius, groundLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
//                    if (!onGround)
//                    {
//                        if (rigidBody.velocity.y > 15)
//                        {
//                            SetTimedVibrate(12, .8f, .0f);
//                        }
//                        else
//                        {
//                            SetTimedVibrate(12, .8f, .0f);
//                        }
//                    }
                    onGround = true;
                    grounded = true;
                    canAirJump = true;
                    canRecover = true;
                    fastFall = false;
                    animator.SetBool("Ground", true);
                }
            }
            if (!grounded)
            {
                onGround = false;
            }
            return onGround;
        }

        private void Vibrate()
        {
            if (vibrate == 0)
            {
                timedVibrate = false;
                input.StopVibration();
            }
            else
            {
                // TODO: Set tuple to control Vibration intensity
                input.VibrateController(leftIntensity, rightIntensity);
                vibrate -= 1;
            }
        }

        public void SetTimedVibrate(int frames, float leftIntensity, float rightIntensity)
        {
            timedVibrate = true;
            this.vibrate = frames;
            this.leftIntensity = leftIntensity;
            this.rightIntensity = rightIntensity;
        }

        public void SetState(PlayerState state)
        {
            currentPlayerState = state;
        }

        public PlayerState GetState()
        {
            return currentPlayerState;
        }

        // Don't increment velocity per frame unintentionally!!!
        public void IncrementVelocity(Vector2 velocity)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x + velocity.x, rigidBody.velocity.y + velocity.y);
        }

        public void IncrementVelocity(float x, float y)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x + x, rigidBody.velocity.y + y);
        }

        public void IncrementVelocityX(float x)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x + x, rigidBody.velocity.y);
        }

        public void IncrementVelocityY(float y)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y);
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

        public void SetLayerOrder(int height)
        {
            foreach (SpriteRenderer renderer in this.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.sortingOrder += 8*height;
            }
        }
    }
}