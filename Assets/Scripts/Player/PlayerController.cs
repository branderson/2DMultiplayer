using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menu;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] internal float maxSpeedX = 8f; // The fastest the player can travel in the x axis.
        [SerializeField] internal float runSpeedX = 14f; // The speed that the player runs
        [SerializeField] internal float WeightRatio = 1f;
        [SerializeField] private float jumpHeight = 6f; // Height of single jump from flat ground
        [SerializeField] private float airJumpHeight = 4f; // Height of air jump
        [SerializeField] private float airSideJumpDistance = 4f;
        [SerializeField] private float airSideJumpHeight = 4f;
        [SerializeField] private float recoveryHeight = 8f;
        [SerializeField] private float neutralAirTime = .68f; // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        [SerializeField] private List<Transform> groundCheck; // A position marking where to check if the player is on the ground
        [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private float sideJumpDistance = 5f; // Horizontal distance of side jump
        [SerializeField] private float sideJumpHeight = 6f; // Height of side jump
        [SerializeField] private float terminalVelocity = -15f; // Maximum regular falling rate
        [SerializeField] private float terminalVelocityFast = -30f; // Fast fall terminal velocity
        [SerializeField] private float fastFallFactor = 3f; // Velocity multiplier for fast fall
        [SerializeField] public float shortHopFactor = .7f; // Fraction of neutral jump height/distance for short hop
        [SerializeField] public float airControlSpeed = .5f; // Fraction of horizontal control while in air
        private const float GroundedRadius = .5f; // Radius of the overlap circle to determine if onGround

        internal float airSideJumpSpeedX;
        internal float airSideJumpSpeedY;
        internal bool canAirJump;
        internal bool canFall;
        internal bool canRecover;
        internal bool facingRight; // For determining which way the player is currently facing
        internal bool fastFall;
        private float gravity; // Rate per second of decreasing vertical speed
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

        internal int playerNumber;
        private PlayerState currentPlayerState;
        private Rigidbody2D rigidBody; // Reference to the player's Rigidbody2D component
        internal Animator animator; // Reference to the player's animator component.
        private IInputController input;
        private readonly List<PlayerController> opponents = new List<PlayerController>();
        internal PlayerUI playerUI;

        internal int SmashCharge = 0;
        private int shield = 100;
        private float damageRatio = .005f;
        internal bool onEdgeRight = false;
        internal bool onEdgeLeft = false;
        internal bool Invincible = false;
        internal bool StateInvincible = false;
        internal int IFrames = 0;
        internal bool Run;
        internal bool CanFallThroughFloor = false;

        public void Init(int zPosition, int slot)
        {
            input = GetComponent<IInputController>();
            shield = 100;
            canFall = true;
            facingRight = true;
            canAirJump = true;
            canRecover = true;
            SetLayerOrder(zPosition);
            playerNumber = slot + 1;
            IFrames = 120; // 5 seconds of invincibility
        }

        public void InitUI(PlayerUI uiCard)
        {
            playerUI = uiCard;
            playerUI.Init(this);
        }

        public void Respawn(Vector2 position)
        {
//            facingRight = true;
            if (playerUI.Lives > 0)
            {
                playerUI.Lives -= 1;
                shield = 100;
                playerUI.Shield = 100;
                fastFall = false;
                animator.SetTrigger("Helpless");
                SetVelocity(Vector2.zero);
                transform.position = position;
                IFrames = 120; // 2 seconds of invincibility
            }
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
            animator.SetFloat("xSpeed", Mathf.Abs(velocityX));
            animator.SetFloat("ySpeed", Mathf.Abs(velocityY));
            animator.SetFloat("WalkAnimationSpeed", Mathf.Abs(velocityX)/6);
            animator.SetBool("FacingRight", facingRight);
            animator.SetBool("Run", Run);
            animator.SetBool("CanAirJump", canAirJump);
            animator.SetBool("CanRecover", canRecover);
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;

            // Manage invincibility state
            if (IFrames > 0)
            {
                Invincible = true;
                IFrames -= 1;
            }
            else if (!StateInvincible)
            {
                Invincible = false;
            }
            else if (StateInvincible)
            {
                Invincible = true;
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
            if (GetVelocityY() > terminalVelocity)
            {
                IncrementVelocityY(gravity*Time.fixedDeltaTime);
            }
            // Caps terminal velocity
            // TODO: Change falling speed to set safely
//            if (GetVelocityY() < terminalVelocity)
//            {
//                SetVelocityY(terminalVelocity);
//            }
        }

        private void FallFast()
        {
            if (GetVelocityY() > terminalVelocityFast)
            {
                IncrementVelocityY(gravity*fastFallFactor*Time.fixedDeltaTime);
            }
//            if (GetVelocityY() < terminalVelocityFast)
//            {
//                SetVelocityY(terminalVelocityFast);
//            }
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
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].gameObject != gameObject && (!passThroughFloor || colliders[i].transform.parent.CompareTag("Impermeable")))
                {
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
        }

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

        public IEnumerator PauseAnimation(int frames)
        {
            float animationSpeed = animator.speed;
            animator.speed = 0;
            while (frames > 0)
            {
                frames--;
                yield return null;
            }
            animator.speed = animationSpeed;
        }

        private IEnumerator Vibrate(int frames, float leftIntensity, float rightIntensity)
        {
            for (int i = 0; i < frames; i++)
            {
                input.VibrateController(leftIntensity, rightIntensity);
                yield return null;
            }
            input.StopVibration();
        }

        public void SetVibrate(int frames, float leftIntensity, float rightIntensity)
        {
            StartCoroutine(Vibrate(frames, leftIntensity, rightIntensity));
        }

        public void SetState(PlayerState state)
        {
            currentPlayerState = state;
        }

        public PlayerState GetState()
        {
            return currentPlayerState;
        }

        public Vector2 GetVelocity()
        {
            return rigidBody.velocity;
        }

        public float GetVelocityX()
        {
            return rigidBody.velocity.x;
        }

        public float GetVelocityY()
        {
            return rigidBody.velocity.y;
        }

        public Vector2 GetSpeed()
        {
            return new Vector2(Mathf.Abs(GetVelocityX()), Mathf.Abs(GetVelocityY()));
        }

        public float GetSpeedX()
        {
            return Mathf.Abs(rigidBody.velocity.x);
        }

        public float GetSpeedY()
        {
            return Mathf.Abs(rigidBody.velocity.y);
        }

        public void Jump(float y)
        {
            if (GetVelocityY() < y)
            {
                SetVelocityY(y);
            }
        }

        public void IncrementVelocity(Vector2 velocity)
        {
            rigidBody.velocity += velocity;
        }

        public void IncrementVelocity(float x, float y)
        {
            rigidBody.velocity += new Vector2(x, y);
        }

        public void IncrementVelocityX(float x)
        {
            rigidBody.velocity += new Vector2(x, 0);
        }

        public void IncrementVelocityY(float y)
        {
            rigidBody.velocity += new Vector2(0, y);
        }

        private void SetVelocity(Vector2 velocity)
        {
            rigidBody.velocity = velocity;
        }

        private void SetVelocity(float x, float y)
        {
            rigidBody.velocity = new Vector2(x, y);
        }

        private void SetVelocityX(float x)
        {
            rigidBody.velocity = new Vector2(x, rigidBody.velocity.y);
        }

        private void SetVelocityY(float y)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, y);
        }

        public void IncrementSpeedX(float x)
        {
            if (GetVelocityX() >= 0)
            {
                // If x will increase speed or magnitude of x is less than magnitude of velocity
                if (x > 0 || -x < GetVelocityX())
                {
                    IncrementVelocityX(x);
                }
                // If x will change sign of velocity, set speed to 0
                else
                {
                    SetVelocityX(0f);
                }
            }
            else
            {
                // If x will increase speed or magnitude of x is less than magnitude of velocity
                if (x < 0 || -x > GetVelocityX())
                {
                    IncrementVelocityX(-x);
                }
                // If x will change sign of velocity, set speed to 0
                else
                {
                    SetVelocityX(0f);
                }
            }
        }

        public void IncrementSpeedY(float y)
        {
            if (GetVelocityY() >= 0)
            {
                // If y will increase speed or magnitude of y is less than magnitude of velocity
                if (y > 0 || -y < GetVelocityY())
                {
                    IncrementVelocityY(y);
                }
                // If y will change sign of velocity, set speed to 0
                else
                {
                    SetVelocityY(0f);
                }
            }
            else
            {
                // If y will increase speed or magnitude of y is less than magnitude of velocity
                if (y < 0 || -y > GetVelocityY())
                {
                    IncrementVelocityY(y);
                }
                // If y will change sign of velocity, set speed to 0
                else
                {
                    SetVelocityY(0f);
                }
            }
            
        }

        public void TakeDamage(int damage)
        {
            shield -= damage;
            if (shield < 0)
            {
                shield = 0;
            }
            playerUI.Shield = shield;
        }

        public float GetDamageRatio()
        {
            if (shield > 0)
            {
                print(((100-shield)*damageRatio + 1));
                return ((100 - shield)*damageRatio + 1);
            }
            else
            {
                return (2);
            }
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