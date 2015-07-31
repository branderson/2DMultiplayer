using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class HoldState : PlayerState
    {
        [SerializeField] private float holdSpeedRatio = .5f;
        private GameObject playerGrabReach;
        private Vector2 move;
        private float moveSpeed = 1f;
        private bool grabReleased = false;

        public void Awake()
        {
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (playerGrabReach == null)
            {
                playerGrabReach = playerController.transform.parent.Find("PlayerGrabReach").gameObject;
            }
            playerGrabReach.SetActive(true);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            grabReleased = false;
            if (!PlayerInputController.ButtonActive("Grab"))
            {
                playerAnimator.SetTrigger("GrabReleased");
                grabReleased = true;
                playerGrabReach.SetActive(false);
            }
            
            if (move.x > 0 && !playerController.facingRight)
            {
                animator.SetTrigger("TurnAround");
            }
            else if (move.x < 0 && playerController.facingRight)
            {
                animator.SetTrigger("TurnAround");
            }
            // Movement
            else if (move.x > 0)
            {
                if (playerController.GetVelocityX() < playerController.maxSpeedX*holdSpeedRatio)
                {
                    playerController.IncrementVelocityX(moveSpeed*1/playerController.WeightRatio);
                }
            }
            else if (move.x < 0)
            {
                if (playerController.GetVelocityX() > -playerController.maxSpeedX*holdSpeedRatio)
                {
                    playerController.IncrementVelocityX(-moveSpeed/playerController.WeightRatio);
                }
            }

            playerController.CheckForGround(); // -> FallState
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            grabReleased = false;
            if (!PlayerInputController.ButtonActive("Grab"))
            {
                playerAnimator.SetTrigger("GrabReleased");
                grabReleased = true;
            }
            if (grabReleased || !playerController.Holding)
            {
                playerGrabReach.SetActive(false);
            }
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);
            move.x = x;
            move.y = y;
        }

        public override string GetName()
        {
            return "HoldState";
        }
    }
}