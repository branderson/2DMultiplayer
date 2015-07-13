using System;
using System.Collections.Generic;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        private const float GroundedRadius = .5f; // Radius of the overlap circle to determine if onGround
        private const float CeilingRadius = 1f; // Radius of the overlap circle to determine should jump through the ceiling 
        [SerializeField] private float airJumpHeight = 4f; // Height of air jump
        [SerializeField] private float airSideJumpDistance = 4f;
        [SerializeField] private float airSideJumpHeight = 4f;
        [SerializeField] private float fastFallFactor = 3f; // Velocity multiplier for fast fall
        [SerializeField] private float jumpHeight = 6f; // Height of single jump from flat ground
        [SerializeField] private float neutralAirTime = .68f; // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        private readonly List<PlayerController> opponents = new List<PlayerController>();
        [SerializeField] private float recoveryHeight = 8f;
        [SerializeField] private float sideJumpDistance = 5f; // Horizontal distance of side jump
        [SerializeField] private float sideJumpHeight = 6f; // Height of side jump
        [SerializeField] private float terminalVelocity = -15f; // Maximum regular falling rate
        [SerializeField] private float terminalVelocityFast = -30f; // Fast fall terminal velocity
        [SerializeField] public float airControlSpeed = .5f; // Fraction of horizontal control while in air
        internal float airSideJumpSpeedX;
        internal float airSideJumpSpeedY;
        internal Animator animator; // Reference to the player's animator component.
        internal bool canAirJump;
        // TODO: Set up a set of flags that are internal, which determine behaviour, everything else is private
        internal bool canFall;
        internal bool canRecover;
        [SerializeField] private Transform[] ceilingCheck; // A position marking where to check for ceilings
        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
        private PlayerState currentPlayerState;
        internal bool facingRight; // For determining which way the player is currently facing
        internal bool fastFall;
        private float gravity; // Rate per second of decreasing vertical speed
        [SerializeField] private Transform[] groundCheck; // A position marking where to check if the player is on the ground
        [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
        private IInputController input;
        internal float leftIntensity;
        internal float maxAirSpeedX;
        [SerializeField] internal float maxSpeedX = 8f; // The fastest the player can travel in the x axis.
        private bool onGround;
        internal bool passThroughFloor;
        internal float recoverySpeed;
        internal float rightIntensity;
        private Rigidbody2D rigidBody; // Reference to the player's Rigidbody2D component
        internal bool run;
        [SerializeField] internal float runSpeedX = 14f; // The speed that the player runs
        [SerializeField] public float shortHopFactor = .5f; // Fraction of neutral jump height/distance for short hop
        internal float sideJumpSpeedX;
        internal float sideJumpSpeedY;
        internal float speedX;
        internal float speedY;
        internal bool timedVibrate;
        internal int vibrate;
        internal float jumpSpeed { get; set; }
        internal float airJumpSpeed { get; set; }
        private bool collisions = true;

        public void Init(int zPosition)
        {
            input = GetComponent<IInputController>();
            canFall = true;
            facingRight = true;
            canAirJump = true;
            canRecover = true;
            collisions = true;
            timedVibrate = false;
            vibrate = 0;
            leftIntensity = 0f;
            rightIntensity = 0f;
            // Set Layer Order should be fixed
            SetLayerOrder(zPosition);
        }

        public void FindPlayers(List<GameObject> players)
        {
            foreach (GameObject player in players)
            {
                PlayerController controller = player.GetComponent<PlayerController>();
                if (controller != this)
                {
                    opponents.Add(controller);
                }
            }
        }

        // Use this for initialization
        private void Awake()
        {
            groundCheck = new Transform[3];
            ceilingCheck = new Transform[3];
            // Setting up references.
            for (int i = 0; i < 3; i++)
            {
                groundCheck[i] = transform.Find("GroundCheck" + (i + 1));
                ceilingCheck[i] = transform.Find("CeilingCheck" + (i + 1));
            }
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody2D>();
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
            passThroughFloor = false;
            animator.SetFloat("xVelocity", speedX);
            animator.SetFloat("yVelocity", speedY);
            animator.SetFloat("xSpeed", Mathf.Abs(speedX));
            animator.SetFloat("ySpeed", Mathf.Abs(speedY));
            animator.SetBool("Run", run);
            animator.SetBool("CanAirJump", canAirJump);
            animator.SetBool("CanRecover", canRecover);
            animator.SetBool("Collisions", collisions);
            // Use coroutine for vibration throughout
            if (timedVibrate)
            {
                Vibrate();
            }

            // Push other players
            PushOthers();
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

        public bool CheckForCeiling()
        {
            bool encounteredImpermeable = false;
            bool approachingCeiling = false;

            List<Collider2D> colliders = new List<Collider2D>();
            foreach (Transform checkPosition in ceilingCheck)
            {
                foreach (Collider2D overlapCollider in Physics2D.OverlapCircleAll(checkPosition.position, CeilingRadius, groundLayer))
                {
                    colliders.Add(overlapCollider);
                }
            }
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].gameObject != gameObject && colliders[i].gameObject.tag != "Impermeable")
                {
                    foreach (Collider2D playerCollider in GetComponents<Collider2D>())
                    {
                        if (!encounteredImpermeable)
                        {
                            SetTriggers(true);
                            approachingCeiling = true;
                        }
                    }
                }
                else if (colliders[i].gameObject.tag == "Impermeable")
                {
                    encounteredImpermeable = true;
                    SetTriggers(false);
                    approachingCeiling = false;
                }
            }

            return approachingCeiling;
        }

        public bool CheckForGround()
        {
            bool grounded = false;
            onGround = false;
            animator.SetBool("Ground", false);
            List<Collider2D> colliders = new List<Collider2D>();
            foreach (Transform checkPosition in groundCheck)
            {
                foreach (Collider2D overlapCollider in Physics2D.OverlapCircleAll(checkPosition.position, GroundedRadius, groundLayer))
                {
                    colliders.Add(overlapCollider);
                }
            }
            animator.SetBool("CanFallThroughFloor", true);
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].gameObject.tag == "Impermeable")
                {
                    animator.SetBool("CanFallThroughFloor", false);
                    passThroughFloor = false;
                }
                if (colliders[i].gameObject != gameObject && !passThroughFloor)
                {
                    if (rigidBody.velocity.y < 0)
                    {
                        SetTriggers(false);
                    }
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
                    fastFall = false;
                    animator.SetBool("Ground", true);
                }
                else if (colliders[i].gameObject != gameObject)
                {
                    grounded = true;
                }
            }
            return grounded;
            //            if (!grounded)
            //            {
            //                onGround = false;
            //            }
            //            return grounded;
        }

        public void SetTriggers(bool value)
        {
            foreach (Collider2D playerCollider in GetComponents<Collider2D>())
            {
                playerCollider.isTrigger = value;
            }
            collisions = !value;
        }

        private void PushOthers()
        {
            //            foreach (PlayerController opponent in opponents)
            //            {
            //                print("Checking opponents");
            //                if (GetComponents<CircleCollider2D>().Any(collider => opponent.rigidBody.IsTouching(collider)))
            //                {
            //                    opponent.IncrementVelocityX(rigidBody.velocity.x*.05f);
            //                    print("Moving");
            //                }
            //            }
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
            vibrate = frames;
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

        // Move each player up by more than the number of layers in the biggest character
        public void SetLayerOrder(int height)
        {
            foreach (SpriteRenderer spritePiece in GetComponentsInChildren<SpriteRenderer>())
            {
                spritePiece.sortingOrder += height;
            }
        }
    }
}