using System;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float maxSpeedX = 40f;                    // The fastest the player can travel in the x axis.
        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool airControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask groundLayer;                  // A mask determining what is ground to the character
        [SerializeField] private float jumpHeight = 8f;                     // Height of single jump from flat ground
        [SerializeField] private float airJumpHeight = 8f;                  // Height of air jump
        [SerializeField] private float neutralAirTime = .68f;               // Time in air jumping once from flat ground, will be incorrect if terminal velocity set too low
        [SerializeField] private float terminalVelocity = -9.81f;            // Maximum regular falling rate
        [SerializeField] private float fastFallFactor = 1.4f;               // Velocity multiplier for fast fall

        private float speedX;
        private float speedY;
        private Transform groundCheck;      // A position marking where to check if the player is grounded.
        const float GroundedRadius = .1f;   // Radius of the overlap circle to determine if grounded
        private bool grounded;              // Whether or not the player is grounded.
        private Transform ceilingCheck;     // A position marking where to check for ceilings
        const float CeilingRadius = .01f;   // Radius of the overlap circle to determine if the player can stand up
        private Animator anim;              // Reference to the player's animator component.
        private Rigidbody2D rigidbody;      // Reference to the player's Rigidbody2D component
        private bool facingRight = true;    // For determining which way the player is currently facing.
        private float jumpSpeed;
        private float airJumpSpeed;
        private bool canAirJump = true;
        private float gravity;              // Rate per second of decreasing vertical speed
        // TODO: Make interface for jump height, max fall speed, time in air from single jump

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


        private void FixedUpdate()
        {
            grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedRadius, groundLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    grounded = true;
                    canAirJump = true;
                }
            }
            anim.SetBool("Ground", grounded);

            // Set the vertical animation
            anim.SetFloat("vSpeed", rigidbody.velocity.y);
        }

        public void Move(float move, bool crouch, bool jump)
        {

            // If crouching, check to see if the character can stand up
            if (!crouch && anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(ceilingCheck.position, CeilingRadius, groundLayer))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (grounded || airControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*crouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                anim.SetFloat("Speed", Mathf.Abs(move));
                if (Math.Abs(move) > 0)
                {
                    anim.SetBool("Run", true);
                }
                else
                {
                    anim.SetBool("Run", false);
                }

                // Move the character
                rigidbody.velocity = new Vector2(move*maxSpeedX, rigidbody.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !facingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && facingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (grounded && jump && anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                grounded = false;
                anim.SetBool("Ground", false);
//                rigidbody.AddForce(new Vector2(0f, jumpForce));
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpSpeed);
                anim.SetTrigger("Jump");
            }
            else if (jump && canAirJump)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0f);
//                rigidbody.AddForce(new Vector2(0f, airJumpForce));
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, airJumpSpeed);
                canAirJump = false;
            }

            if (!grounded) // && !fastFall
            {
                // Handles acceleration due to gravity
                if (rigidbody.velocity.y > terminalVelocity)
                {
                    rigidbody.velocity = new Vector2(rigidbody.velocity.x,
                        rigidbody.velocity.y + gravity*Time.fixedDeltaTime);
                }
                // Caps terminal velocity
                if (rigidbody.velocity.y < terminalVelocity)
                {
                    rigidbody.velocity = new Vector2(rigidbody.velocity.x, terminalVelocity);
                }
            }
//
//            else
//            {
//                rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
//            }
            
            speedX = rigidbody.velocity.x; // For debug purposes
            speedY = rigidbody.velocity.y;
        }


        private void Flip()
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
