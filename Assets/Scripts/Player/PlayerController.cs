﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Assets.Scripts.Menu;
using Assets.Scripts.Player.States;
using Assets.Scripts.Player.Triggers;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] internal float maxSpeedX = 8f; // The fastest the player can travel in the x axis.
        [SerializeField] internal float runSpeedX = 14f; // The speed that the player runs
        [SerializeField] internal float MaxBlockStrength = 20; // Amount of damage (before block reduction) that can be sustained before block break
        [SerializeField] private const float BlockRechargeRate = .2f;
        [SerializeField] internal float BlockBreakPenalty = 20;
        [SerializeField] internal int BlockBreakStun = 60;
        [SerializeField] internal int resistance = 0;
        [SerializeField] internal int launchFrames = 15;
        [Range(.25f, 2)][SerializeField] internal float WeightRatio = 1f;
        [SerializeField] private float noShieldPenalty = 1f;
        [SerializeField] private float noShieldBonus = .5f;
        [SerializeField] private int MaxAirJumps = 1;
        [SerializeField] private float airJumpDecayFactor = .5f;
        [SerializeField] private float jumpHeight = 6f; // Height of single jump from flat ground
        [SerializeField] private float airJumpHeight = 4f; // Height of air jump
        [SerializeField] private float airSideJumpDistance = 4f;
        [SerializeField] private float airSideJumpHeight = 4f;
        [SerializeField] private float recoveryHeight = 8f;
        [SerializeField] private float neutralAirTime = .68f; // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        [SerializeField] private List<Transform> groundCheck; // A position marking where to check if the player is on the ground
        [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
        [SerializeField] private float sideJumpDistance = 5f; // Horizontal distance of side jump
        [SerializeField] private float sideJumpHeight = 6f; // Height of side jump
        [SerializeField] private float fallSpeed = 0f;
        [SerializeField] private float terminalVelocity = -15f; // Maximum regular falling rate
        [SerializeField] private float terminalVelocityFast = -30f; // Fast fall terminal velocity
        [SerializeField] private float fastFallFactor = 3f; // Velocity multiplier for fast fall
        [SerializeField] public float shortHopFactor = .7f; // Fraction of neutral jump height/distance for short hop
        [SerializeField] public float airControlSpeed = .5f; // Fraction of horizontal control while in air
        private const float MaxVelCancelX = 20f;
        private const float MaxVelCancelY = 80f;
        private const float MaxDI = 20f;
        private const float GroundedRadius = .1f; // Radius of the overlap circle to determine if OnGround

        internal float airSideJumpSpeedX;
        internal float airSideJumpSpeedY;
        internal int AirJumps;
        internal bool canFall;
        internal bool canRecover;
        internal bool facingRight; // For determining which way the player is currently facing
        internal bool fastFall;
        private float gravity; // Rate per second of decreasing vertical speed
        internal float maxAirSpeedX;
        internal bool OnGround;
        internal bool passThroughFloor;
        internal bool fallingThroughFloor = false;
        internal float recoverySpeed;
        internal float velocityX;
        internal float velocityY;
        internal float sideJumpSpeedX;
        internal float sideJumpSpeedY;
        internal float jumpSpeed { get; set; }
        internal float airJumpSpeed { get; set; }

        internal int playerNumber;
        internal string characterName;
        private PlayerState currentPlayerState;
        private Rigidbody2D rigidBody; // Reference to the player's Rigidbody2D component
        internal Animator animator; // Reference to the player's animator component.
        internal IInputController input;
        internal readonly List<PlayerController> opponents = new List<PlayerController>();
        internal PlayerUI playerUI;
        internal SpriteRenderer sprite;
        internal Color color; // TODO: Get rid of this
        private Vector2 spritePosition;
        internal bool Computer = false;

        private float animationResumeSpeed;
        internal bool Paused = false;
        internal bool Respawned = false;
        private bool stunned = false;
        internal int SmashCharge = 0;
        private int health = 100;
        internal float BlockStrength;
        private float damageRatio = .01f;
        internal bool onEdgeRight = false;
        internal bool onEdgeLeft = false;
        internal bool Invincible = false;
        internal bool StateInvincible = false;
        internal int IFrames = 0;
        internal bool Run;
        internal bool Blocking = false;
        internal bool Holding = false;
        internal bool Grabbed = false;
        internal bool CanFallThroughFloor = false;
        private Vector2 storedVelocity;

        public void Init(int zPosition, int slot, string playerName, bool computer)
        {
            input = GetComponent<IInputController>();
            color = sprite.color;
            health = 100;
            BlockStrength = MaxBlockStrength;
            canFall = true;
            facingRight = true;
            AirJumps = MaxAirJumps;
            canRecover = true;
            SetLayerOrder(zPosition);
            playerNumber = slot + 1;
            IFrames = 120; // 5 seconds of invincibility
            characterName = playerName;
            Computer = computer;
        }

        public void InitUI(PlayerUI uiCard)
        {
            playerUI = uiCard;
            playerUI.Init(this);
        }

        public void Respawn(Vector3 position)
        {
            if (true) //(playerUI.Lives > 0)
            {
                if (!facingRight)
                {
                    Flip();
                }
                StopAllCoroutines();
                StopShaking();
                animator.SetBool("Stunned", false);
                animator.SetBool("Launch", false);
                stunned = false;
                SetVibrate(25, 1f, .5f);
                playerUI.Lives -= 1;
                health = 100;
                playerUI.Shield = 100;
                AirJumps = MaxAirJumps;
                canFall = true;
                canRecover = true;
                fastFall = false;
                animator.SetTrigger("Helpless");
                SetVelocity(Vector2.zero);
                transform.position = position;
//                rigidBody.MovePosition(position);
                IFrames = 120; // 2 seconds of invincibility
                Respawned = true;
            }
        }

        // TODO: Non AI players may not need this info
        public void FindPlayers(List<GameObject> players)
        {
            foreach (GameObject player in players)
            {
                PlayerController controller = player.GetComponentInChildren<PlayerController>();
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
            sprite = transform.parent.GetComponentInChildren<SpriteRenderer>();
            spritePosition = sprite.transform.localPosition;
            CalculatePhysics();
        }

        // Update is called once per frame
        private void Update()
        {
            velocityX = rigidBody.velocity.x;
            velocityY = rigidBody.velocity.y;

            if (Mathf.Approximately(velocityX, 0))
            {
                Run = false;
            }
            if (input.ButtonActive("Block") && BlockStrength > 0)
            {
                animator.SetTrigger("Block");
            }
            //            print(currentPlayerState.GetName());
        }

        private void FixedUpdate()
        {
            if (stunned)
            {
                StunFall();
            }
            else if (canFall) // && !OnGround)
            {
                if (fastFall)
                    FallFast();
                else
                    FallRegular();
            }
//            ApplyAirResistance();
            animator.SetFloat("xVelocity", velocityX);
            animator.SetFloat("yVelocity", velocityY);
            animator.SetFloat("xSpeed", Mathf.Abs(velocityX));
            animator.SetFloat("ySpeed", Mathf.Abs(velocityY));
            animator.SetInteger("ShieldPercent", health);
            animator.SetFloat("WalkAnimationSpeed", Mathf.Abs(velocityX)/6);
            animator.SetBool("FacingRight", facingRight);
            animator.SetBool("Run", Run);
            animator.SetInteger("AirJumps", AirJumps);
            animator.SetBool("CanRecover", canRecover);
            animator.SetBool("Holding", Holding);
            animator.SetBool("Grabbed", Grabbed);
            transform.parent.position = rigidBody.position;
//            rigidBody.position = transform.parent.position;
            transform.localPosition = Vector3.zero;

            // Recharge block
            if (!Blocking && BlockStrength < MaxBlockStrength)
            {
                BlockStrength += BlockRechargeRate;
            }
            if (BlockStrength > MaxBlockStrength)
            {
                BlockStrength = MaxBlockStrength;
            }

            // Manage invincibility state
            if (IFrames > 0)
            {
                if (IFrames%10 == 0)
                {
                    sprite.color = Color.yellow;
                }
                else if (IFrames%5 == 0)
                {
                    sprite.color = color;
                }
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
            else
            {
                sprite.color = color;
            }
        }

        private void CalculatePhysics()
        {
            jumpSpeed = 4*jumpHeight/neutralAirTime;
            if (fallSpeed == 0)
            {
                gravity = -2*jumpSpeed/neutralAirTime;
            }
            else
            {
                gravity = -fallSpeed;
            }
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

        private void StunFall()
        {
            IncrementVelocityY(-40*Time.fixedDeltaTime);
        }

        private void ApplyAirResistance()
        {
            if (!OnGround && GetSpeedX() > airSideJumpSpeedX)
            {
                IncrementSpeedX(-4f*Time.fixedDeltaTime);
            }
        }

        public bool CheckForGround()
        {
            bool grounded = false;
            OnGround = false;
            animator.SetBool("Ground", false);
            if (RaycastGround())
            {
                List<Collider2D> colliders = groundCheck.SelectMany(checkPosition => Physics2D.OverlapCircleAll(checkPosition.position, GroundedRadius, groundLayer)).ToList();
                foreach (Collider2D collision in colliders)
                {
                    if (collision.gameObject != gameObject)
                    {
                        grounded = true;
                    }
                    if (grounded && (!passThroughFloor && !fallingThroughFloor))
                    {
                        fastFall = false;
                        animator.SetBool("Ground", true);
                        OnGround = true;
                    }
                }
            }
            return grounded;
        }

        public bool RaycastGround()
        {
            return RaycastGround(-GroundedRadius) || RaycastGround(GroundedRadius);
        }

        public bool RaycastGround(float offset)
        {
            RaycastHit2D rayCast = Physics2D.Raycast(new Vector2(transform.position.x + offset, transform.position.y),
                Vector2.down, Mathf.Infinity, groundLayer);
            return rayCast.collider != null;
        }

        public void IgnoreCollision(Collider2D other)
        {
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
            foreach (Collider2D collider in colliders)
            {
                Physics2D.IgnoreCollision(collider, other);
            }
        }

        public void IgnoreCollision(Collider2D other, bool ignore)
        {
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
            foreach (Collider2D collider in colliders)
            {
                Physics2D.IgnoreCollision(collider, other, ignore);
            }
        }

        public IEnumerator PauseAnimation(int frames)
        {
            animationResumeSpeed = animator.speed;
            animator.speed = 0;
            while (frames > 0)
            {
                frames--;
                Paused = true;
                yield return null;
            }
            animator.speed = animationResumeSpeed;
            Paused = false;
        }

        public void ResumeAnimation()
        {
            StopCoroutine("PauseAnimation");
            Paused = false;
            if (animationResumeSpeed > 0)
            {
                animator.speed = animationResumeSpeed;
            }
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

        public int GetShield()
        {
            return health;
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

        // Increments velocity up to MaxVelCancel to reach velocity
        public void CappedSetVelocity(Vector2 velocity)
        {

        }

        public void CappedSetVelocity(float x, float y)
        {
        }

        public void CappedSetVelocityX(float x)
        {
            float xAdjust = x - GetVelocityX();
            if (xAdjust > MaxVelCancelX)
            {
                xAdjust = MaxVelCancelX;
            }
            else if (xAdjust < -MaxVelCancelX)
            {
                xAdjust = -MaxVelCancelX;
            }
            IncrementVelocityX(xAdjust);
        }

        public void CappedSetVelocityY(float y)
        {
            float yAdjust = y - GetVelocityY();
            if (yAdjust > MaxVelCancelY)
            {
                yAdjust = MaxVelCancelY;
            }
            else if (yAdjust < -MaxVelCancelY)
            {
                yAdjust = -MaxVelCancelY;
            }
            IncrementVelocityY(yAdjust);
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

        public void ResetAirJumps()
        {
            AirJumps = MaxAirJumps;
        }

        public float GetAirJumpSpeed()
        {
            if (AirJumps == MaxAirJumps)
            {
                return airJumpSpeed;
            }
            else
            {
//                print(Mathf.Pow(airJumpSpeed, 1f/(airJumpDecayFactor*(MaxAirJumps - AirJumps))));
                return airJumpSpeed*Mathf.Exp(-airJumpDecayFactor*(MaxAirJumps - AirJumps));
            }
        }

        public float GetAirSideJumpSpeedX()
        {
            if (AirJumps == MaxAirJumps)
            {
                return airSideJumpSpeedX;
            }
            else
            {
                return airSideJumpSpeedX*Mathf.Exp(-airJumpDecayFactor*(MaxAirJumps - AirJumps));
            }
        }

        public float GetAirSideJumpSpeedY()
        {
            if (AirJumps == MaxAirJumps)
            {
                return airSideJumpSpeedY;
            }
            else
            {
                return airSideJumpSpeedY*Mathf.Exp(-airJumpDecayFactor*(MaxAirJumps - AirJumps));
            }
        }

        public void TakeKnockback(AttackData attackData)
        {
            if (!Invincible && attackData.Stagger > resistance && !Blocking)
            {
                // Calculate scaled knockback
                float scaledKnockback = attackData.Knockback;
                if (!attackData.SetKnockback)
                {
                    float knockbackScaling = 0;
                    // Shield based scaling
                    knockbackScaling = (100-health)/10 + (100-health)*attackData.Damage/20;
                    // Weight based scaling
                    knockbackScaling *= 1/(WeightRatio + 1);
                    knockbackScaling *= attackData.Scaling;
//                    print("Base: " + scaledKnockback);
//                    print("Scaling: " + knockbackScaling);
                    scaledKnockback += knockbackScaling;
                    if (health == 0)
                    {
                        scaledKnockback *= 1.5f;
                    }
                }

                // Directional influence
                // Angle in degrees of directional input
                if (Mathf.Abs(input.GetAxis("Vertical")) > .1 || Mathf.Abs(input.GetAxis("Horizontal")) > .1)
                {
                    float directionalAngle = Mathf.Atan2(input.GetAxis("Vertical"), input.GetAxis("Horizontal")) * Mathf.Rad2Deg;
    //                print("y is " + input.GetAxis("Vertical") + " , x is " + input.GetAxis("Horizontal") + " , angle is " + directionalAngle + " , or " + directionalAngle*Mathf.Rad2Deg);
                    // Adjust directionalAngle to be between 0 and 360
                    while (directionalAngle > 360 || directionalAngle < 0)
                    {
                        if (directionalAngle > 360)
                        {
                            directionalAngle -= 360;
                        }
                        else if (directionalAngle < 0)
                        {
                            directionalAngle += 360;
                        }
                    }

                    // Determine angle between input and knockback angle
                    float angleDifference = directionalAngle - attackData.Direction;
//                    print("Attack: " + attackData.Direction + ", input: " + directionalAngle + ", difference: " + angleDifference);

//                    print("Adjustment: " + MaxDI*(Mathf.Sin(angleDifference*Mathf.Deg2Rad)));
                    // Adjust knockback angle based on closeness to perpendicular
                    attackData.Direction += MaxDI*(Mathf.Sin(angleDifference*Mathf.Deg2Rad));
                }

                Vector2 direction = new Vector2(Mathf.Cos(attackData.Direction*Mathf.Deg2Rad),
                    Mathf.Sin(attackData.Direction*Mathf.Deg2Rad));

                // Account for players with different gravity values. this formula will need tweaking
                float gravityAdjust = gravity/-120;

                // Only set the velocity if new velocity greater than initial
                if (
                    Mathf.Sqrt(Mathf.Pow(scaledKnockback*direction.x, 2) +
                               Mathf.Pow(scaledKnockback*direction.y*gravityAdjust, 2)) >
                    Mathf.Sqrt(Mathf.Pow(storedVelocity.x, 2) + Mathf.Pow(storedVelocity.y, 2)))
                {
                    SetVelocity(scaledKnockback*direction.x, scaledKnockback*direction.y*gravityAdjust);
                }
                else
                {
                    SetVelocity(storedVelocity);
                }

                // TODO: Look for meteor smash type moves and set player flag for them

                // Math for determining number of frames to stun for given knockback
                int stunFrames = (int) (scaledKnockback*.4f);
                Stun(stunFrames, true);
            }
            GetState().TakeHit(attackData);
            TakeDamage(attackData.Damage);
            canRecover = true;
            // Stagger(stagger);
        }

        public void TakeDamage(int damage)
        {
            if (!Invincible)
            {
                if (!Blocking)
                {
                    health -= damage;
                }
                else
                {
                    health -= damage/2; // How much to reduce damage by
                }
                if (health < 0)
                {
                    health = 0;
                }
                playerUI.Shield = health;
            }
        }

        public float GetAttackRatio()
        {
            if (health > 0)
            {
                return 1;
            }
            else
            {
                return 1 + noShieldBonus;
            }
        }

        public void Hitlag(AttackData attackData)
        {
            ResumeAnimation();
            StartCoroutine("HitlagRoutine", attackData.Hitlag);
        }

        private IEnumerator HitlagRoutine(int frames)
        {
            Vector2 position = transform.position;
            animationResumeSpeed = animator.speed;
            while (frames > 0)
            {
                transform.position = position;
                SetVelocity(0, 0);
                animator.speed = 0;
                Paused = true;
                frames--;
                yield return null;
            }
            animator.speed = animationResumeSpeed;
            Paused = false;
        }

        private IEnumerator ReceiveHitRoutine(AttackData attackData)
        {
            int frames = attackData.Hitlag;
            Stun(frames, false);
            Vector2 position = transform.position;
            storedVelocity = GetVelocity();
//            animationResumeSpeed = animator.speed;
            while (frames > 0)
            {
                transform.position = position;
                SetVelocity(0, 0);
//                if (frames != attackData.Hitlag)
//                {
//                    animator.speed = 0;
//                }
//                Paused = true;
                frames--;
                yield return null;
            }
//            animator.speed = animationResumeSpeed;
            TakeKnockback(attackData);
            Paused = false;
        }

        public void TakeHit(AttackData attackData)
        {
            if (!Invincible && !Blocking)
            {
                ResumeAnimation();
                StartCoroutine("ReceiveHitRoutine", attackData);
            }
            else if (Blocking)
            {
                TakeKnockback(attackData);
            }
        }

//        private IEnumerator HitlagRoutine(int frames)
//        {
//            PauseAnimation()
//        }

        public void Stun(int frames, bool canLaunch)
        {
            if (!Invincible)
            {
                StopCoroutine("StunRoutine");
                if (frames >= launchFrames && canLaunch)
                {
                    animator.SetBool("Launch", true);
                }
                if (frames < 10)
                {
                    frames = 10;
                }
                StartCoroutine("StunRoutine", frames);
            }
        }

        private IEnumerator StunRoutine(int frames)
        {
//            print("Stun frames: " + frames);
            animator.SetBool("Stunned", true);
            stunned = true;
            while (frames > 0)
            {
                frames--;
                yield return null;
            }
            stunned = false;
            animator.SetBool("Stunned", false);
            animator.SetBool("Launch", false);
            yield break;
        }


        public void ShakeSpriteX(int frames)
        {
            spritePosition = sprite.transform.localPosition;
            StopCoroutine("ShakeSprite");
            StartCoroutine("ShakeSprite", frames);
        }

        public void StopShaking()
        {
            StopCoroutine("ShakeSprite");
            sprite.transform.localPosition = spritePosition;
        }

        private IEnumerator ShakeSprite(int frames)
        {
            while (frames > 0)
            {
                frames--;
                if (frames%10 == 0)
                {
                    sprite.transform.localPosition = spritePosition + new Vector2(.05f, 0);
                }
                else if (frames%5 == 0)
                {
                    sprite.transform.localPosition = spritePosition - new Vector2(.05f, 0);
                }
                yield return null;
            }
            sprite.transform.localPosition = spritePosition;
        }

//        private IEnumerator Shake(float intensity)
//        {
//            // TODO: This doesn't work
//            int frame = 0;
//            Vector3 spritePosition = sprite.transform.localPosition;
//            Vector3 forwardPosition = new Vector3(spritePosition.x + .01f, spritePosition.y, spritePosition.z);
//            Vector3 backwardPosition = new Vector3(spritePosition.x - .01f, spritePosition.y, spritePosition.z);
//            while (true)
//            {
//                if (frame%10 == 0)
//                {
//                    sprite.transform.localPosition = forwardPosition;
//                }
//                else if (frame%5 == 0)
//                {
//                    sprite.transform.localPosition = backwardPosition;
//                }
//                frame++;
//                yield return null;
//            }
//        }

        public void Stagger(int stagger)
        {
            animator.SetTrigger("Stagger");
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