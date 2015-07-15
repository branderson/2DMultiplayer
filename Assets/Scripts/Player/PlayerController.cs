using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] internal float maxSpeedX = 8f; // The fastest the player can travel in the x axis.
        [SerializeField] internal float runSpeedX = 14f; // The speed that the player runs
        [SerializeField] private float jumpHeight = 6f; // Height of single jump from flat ground
        [SerializeField] private float airJumpHeight = 4f; // Height of air jump
        [SerializeField] private float airSideJumpDistance = 4f;
        [SerializeField] private float airSideJumpHeight = 4f;
        [SerializeField] private float recoveryHeight = 8f;
        [SerializeField] private float neutralAirTime = .68f; // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        [SerializeField] private List<Transform> groundCheck; // A position marking where to check if the player is on the ground
        [SerializeField] private List<Transform> ceilingCheck; // A position marking where to check for ceilings
        [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private float sideJumpDistance = 5f; // Horizontal distance of side jump
        [SerializeField] private float sideJumpHeight = 6f; // Height of side jump
        [SerializeField] private float terminalVelocity = -15f; // Maximum regular falling rate
        [SerializeField] private float terminalVelocityFast = -30f; // Fast fall terminal velocity
        [SerializeField] private float fastFallFactor = 3f; // Velocity multiplier for fast fall
        [SerializeField] public float shortHopFactor = .5f; // Fraction of neutral jump height/distance for short hop
        [SerializeField] public float airControlSpeed = .5f; // Fraction of horizontal control while in air
        [SerializeField] public float internalDampingRate = 1f;
        [SerializeField] public float externalDampingRate = 1f;
        private const float GroundedRadius = .5f; // Radius of the overlap circle to determine if onGround
        private const float CeilingRadius = 1f; // Radius of the overlap circle to determine should jump through the ceiling 

//        private Vector2 setterVelocity;
        internal Vector2 internalVelocity;
        internal Vector2 externalVelocity;
        internal float airSideJumpSpeedX;
        internal float airSideJumpSpeedY;
        internal bool canAirJump;
        internal bool canFall;
        internal bool canRecover;
        internal bool facingRight; // For determining which way the player is currently facing
        internal bool fastFall;
        private float gravity; // Rate per second of decreasing vertical speed
        internal float rightIntensity;
        internal float leftIntensity;
        internal float maxAirSpeedX;
        private bool onGround;
        internal bool passThroughFloor; // TODO: Try to get rid of this
        internal float recoverySpeed;
        internal float velocityX;
        internal float velocityY;
        internal float sideJumpSpeedX;
        internal float sideJumpSpeedY;
        internal float jumpSpeed { get; set; }
        internal float airJumpSpeed { get; set; }
//        internal int layer;

        private PlayerState currentPlayerState;
        private Rigidbody2D rigidBody; // Reference to the player's Rigidbody2D component
        internal Animator animator; // Reference to the player's animator component.
        private IInputController input;
        private readonly List<PlayerController> opponents = new List<PlayerController>();
        
        // TODO: Switch this to coroutines
        internal bool timedVibrate;
        internal int vibrate;
        internal bool run;
        private bool GroundCollisions = true;

        internal bool CanFallThroughFloor = false;

        public void Init(int zPosition, int slot)
        {
            input = GetComponent<IInputController>();
            canFall = true;
            facingRight = true;
            canAirJump = true;
            canRecover = true;
            GroundCollisions = true;
            timedVibrate = false;
            vibrate = 0;
            leftIntensity = 0f;
            rightIntensity = 0f;
            // Set Layer Order should be fixed
            SetLayerOrder(zPosition);
//            layer = gameObject.layer = 9 + slot; // Set the collision layer of the player. This is important for handling collisions manually
        }

        // TODO: Non AI players may not need this info
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
            // Setting up references.
            animator = GetComponentInParent<Animator>();
            rigidBody = GetComponent<Rigidbody2D>();
            internalVelocity = new Vector2();
            externalVelocity = new Vector2();
            internalVelocity = Vector2.zero;
            externalVelocity = Vector2.zero;
            CalculatePhysics();
        }

        // Update is called once per frame
        private void Update()
        {
            velocityX = rigidBody.velocity.x;
            velocityY = rigidBody.velocity.y;
            //            print(currentPlayerState.GetName());
        }

        private void FixedUpdate()
        {
            if (canFall)
            {
                if (fastFall)
                    FallFast();
                else
                    FallRegular();
            }
            animator.SetFloat("xVelocity", velocityX);
            animator.SetFloat("yVelocity", velocityY);
            animator.SetFloat("xVelocityInternal", internalVelocity.x);
            animator.SetFloat("yVelocityInternal", internalVelocity.y);
            animator.SetFloat("xSpeed", Mathf.Abs(velocityX));
            animator.SetFloat("ySpeed", Mathf.Abs(velocityY));
            animator.SetFloat("xSpeedInternal", Mathf.Abs(internalVelocity.x));
            animator.SetFloat("ySpeedInternal", Mathf.Abs(internalVelocity.y));
            animator.SetBool("Run", run);
            animator.SetBool("CanAirJump", canAirJump);
            animator.SetBool("CanRecover", canRecover);

            // Use coroutine for vibration throughout
            if (timedVibrate)
            {
                Vibrate();
            }

            // Push other players
            PushOthers();

            // TODO: Need to figure out a way to account for the state of the world in internal velocity
//            if (Mathf.Abs(rigidBody.velocity.x) - Mathf.Abs(externalVelocity.x) < Mathf.Abs(internalVelocity.x))
//            {
//                if (internalVelocity.x > 0)
//                {
//                    internalVelocity.x = Mathf.Abs(rigidBody.velocity.x) - Mathf.Abs(externalVelocity.x);
//                }
//                else if (internalVelocity.x < 0)
//                {
//                    internalVelocity.x = -Mathf.Abs(rigidBody.velocity.x) + Mathf.Abs(externalVelocity.x);
//                }
//            }

            // Update velocity
//            rigidBody.velocity = rigidBody.velocity + internalVelocity + externalVelocity;

            // Velocity damping
            DampExternal();
//            internalVelocity = Vector2.zero;
//            externalVelocity = Vector2.zero;

            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
        }

//        private void OnTriggerExit2D(Collision2D other)
//        {
//        }

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
            if (internalVelocity.y > terminalVelocity)
            {
                print("Fall " + internalVelocity.y);
                IncrementInternalVelocityY(gravity*Time.fixedDeltaTime);
            }
            // Caps terminal velocity
            if (internalVelocity.y < terminalVelocity)
            {
                print("Fall " + internalVelocity.y);
                SetInternalVelocityY(terminalVelocity);
            }
        }

        private void FallFast()
        {
            if (internalVelocity.y > terminalVelocityFast)
            {
                IncrementInternalVelocityY(gravity*fastFallFactor*Time.fixedDeltaTime);
            }
            if (internalVelocity.y < terminalVelocityFast)
            {
                SetInternalVelocityY(terminalVelocityFast);
            }
        }

        public void DampInternal()
        {
            if (internalVelocity.x > 0)
            {
                if (internalVelocity.x > internalDampingRate)
                {
                    internalVelocity.x -= internalDampingRate;
                }
                else
                {
                    internalVelocity.x = 0;
                }
            }
            else if (internalVelocity.x < 0)
            {
                if (internalVelocity.x < -internalDampingRate)
                {
                    internalVelocity.x += internalDampingRate;
                }
                else
                {
                    internalVelocity.x = 0;
                }
            }
//            if (internalVelocity.y > 0)
//            {
//                if (internalVelocity.y > internalDampingRate)
//                {
//                    internalVelocity.y -= internalDampingRate;
//                }
//                else
//                {
//                    internalVelocity.y = 0;
//                }
//            }
//            else if (internalVelocity.y < 0)
//            {
//                if (internalVelocity.y < -internalDampingRate)
//                {
//                    internalVelocity.y += internalDampingRate;
//                }
//                else
//                {
//                    internalVelocity.y = 0;
//                }
//            }
        }

        public void DampExternal()
        {
            if (externalVelocity.x > 0)
            {
                if (externalVelocity.x > externalDampingRate)
                {
                    IncrementExternalVelocityX(-externalDampingRate);
                }
                else
                {
                    SetExternalVelocityX(0);
                }
            }
            else if (externalVelocity.x < 0)
            {
                if (externalVelocity.x < -externalDampingRate)
                {
                    IncrementExternalVelocityX(externalDampingRate);
                }
                else
                {
                    SetExternalVelocityX(0);
                }
            }
            if (externalVelocity.y > 0)
            {
                if (externalVelocity.y > externalDampingRate)
                {
                    IncrementExternalVelocityY(-externalDampingRate);
                }
                else
                {
                    SetExternalVelocityY(0);
                }
            }
            else if (externalVelocity.y < 0)
            {
                if (externalVelocity.y < -externalDampingRate)
                {
                    IncrementExternalVelocityY(externalDampingRate);
                }
                else
                {
                    SetExternalVelocityY(0);
                }
            }
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
//            animator.SetBool("CanFallThroughFloor", true);
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].gameObject != gameObject && (!passThroughFloor || colliders[i].transform.parent.CompareTag("Impermeable")))
                {
                    //                    if (!onGround)
                    //                    {
                    //                        if (internalVelocity.y > 15)
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

//        public void SetGroundCollisions(bool value)
//        {
////            foreach (Collider2D playerCollider in GetComponents<Collider2D>())
////            {
////                playerCollider.isTrigger = value;
////            }
////            collisions = !value;
////            Physics2D.IgnoreLayerCollision(layer, 8, !value); // TODO: not converting groundLayer properly   // groundLayer, value);
//            GroundCollisions = value;
//        }

        public void IgnoreCollision(Collider2D other)
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                Physics2D.IgnoreCollision(collider, other);
            }
        }

        public void IgnoreCollision(Collider2D other, bool ignore)
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                Physics2D.IgnoreCollision(collider, other, ignore);
            }
        }

        private void PushOthers()
        {
//            foreach (PlayerController opponent in opponents)
//            {
//                print("Checking opponents");
//                if (transform.parent.GetComponentsInChildren<Collider2D>().Any(collider => opponent.rigidBody.IsTouching(collider)))
//                {
//                    opponent.IncrementInternalVelocityX(internalVelocity.x*.05f);
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
        public void IncrementInternalVelocity(Vector2 velocity)
        {
//            internalVelocity = new Vector2(internalVelocity.x + velocity.x, internalVelocity.y + velocity.y);
            internalVelocity = velocity;
            rigidBody.velocity += velocity;
        }

        public void IncrementInternalVelocity(float x, float y)
        {
//            internalVelocity = new Vector2(internalVelocity.x + x, internalVelocity.y + y);
            internalVelocity.x += x;
            internalVelocity.y += y;
            rigidBody.velocity += new Vector2(x, y);
        }

        public void IncrementInternalVelocityX(float x)
        {
//            internalVelocity = new Vector2(internalVelocity.x + x, internalVelocity.y);
            internalVelocity.x += x;
            rigidBody.velocity += new Vector2(x, 0);
        }

        public void IncrementInternalVelocityY(float y)
        {
//            internalVelocity = new Vector2(internalVelocity.x, internalVelocity.y);
            // TODO: Incrementing isn't working right
            internalVelocity.y += y;
            rigidBody.velocity += new Vector2(0, y);
            print(rigidBody.velocity.y);
        }

        public void SetInternalVelocity(Vector2 velocity)
        {
            rigidBody.velocity += velocity - internalVelocity;
            internalVelocity = velocity;
        }

        public void SetInternalVelocity(float x, float y)
        {
            rigidBody.velocity += new Vector2(x, y) - internalVelocity;
            internalVelocity.x = x;
            internalVelocity.y = y;
            //            internalVelocity = new Vector2(x, y);
        }

        public void SetInternalVelocityX(float x)
        {
            rigidBody.velocity += new Vector2(x - internalVelocity.x, 0);
            internalVelocity.x = x;
            //            internalVelocity = new Vector2(x, internalVelocity.y);
        }

        public void SetInternalVelocityY(float y)
        {
            print("Old internalVelocity.y = " + internalVelocity.y);
            print("Old rigidBody.velocity.y = " + rigidBody.velocity.y);
            print(rigidBody.velocity.y + " += " + y + " - " + internalVelocity.y);
            rigidBody.velocity += new Vector2(0, y - internalVelocity.y);
            internalVelocity.y = y;
            print("y = " + y + ", internal.y = " + internalVelocity.y);
            print("Set " + rigidBody.velocity.y);
            //            internalVelocity = new Vector2(internalVelocity.x, y);
        }

        public void IncrementExternalVelocity(Vector2 velocity)
        {
//            externalVelocity = new Vector2(externalVelocity.x + velocity.x, externalVelocity.y + velocity.y);
            externalVelocity = velocity;
            rigidBody.velocity += velocity;
        }

        public void IncrementExternalVelocity(float x, float y)
        {
//            externalVelocity = new Vector2(externalVelocity.x + x, externalVelocity.y + y);
            externalVelocity.x += x;
            externalVelocity.y += y;
            rigidBody.velocity += new Vector2(x, y);
        }

        public void IncrementExternalVelocityX(float x)
        {
//            externalVelocity = new Vector2(externalVelocity.x + x, externalVelocity.y);
            externalVelocity.x += x;
            rigidBody.velocity += new Vector2(x, 0);
        }

        public void IncrementExternalVelocityY(float y)
        {
//            externalVelocity = new Vector2(externalVelocity.x, externalVelocity.y);
            externalVelocity.y += y;
            rigidBody.velocity += new Vector2(0, y);
        }

        public void SetExternalVelocity(Vector2 velocity)
        {
            rigidBody.velocity += velocity - externalVelocity;
            externalVelocity = velocity;
        }

        public void SetExternalVelocity(float x, float y)
        {
            rigidBody.velocity += new Vector2(x, y) - externalVelocity;
            externalVelocity.x = x;
            externalVelocity.y = y;
            //            externalVelocity = new Vector2(x, y);
        }

        public void SetExternalVelocityX(float x)
        {
            rigidBody.velocity += new Vector2(x - externalVelocity.x, 0);
            externalVelocity.x = x;
            //            externalVelocity = new Vector2(x, externalVelocity.y);
        }

        public void SetExternalVelocityY(float y)
        {
            rigidBody.velocity += new Vector2(0, y - externalVelocity.y);
            externalVelocity.y = y;
            //            externalVelocity = new Vector2(externalVelocity.x, y);
        }
        public void Flip()
        {
            // Switch the way the player is labelled as facing.
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
            Vector3 parentTransform = transform.parent.transform.localScale;
            Vector3 theScale = parentTransform;
            theScale.x *= -1;
            transform.parent.transform.localScale = theScale;
        }

        // Move each player up by more than the number of layers in the biggest character
        public void SetLayerOrder(int height)
        {
            foreach (SpriteRenderer spritePiece in transform.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                spritePiece.sortingOrder += height;
            }
        }
    }
}