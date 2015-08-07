using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class ApplyFacingRelativeVelocity : StateMachineBehaviour
    {
        [SerializeField] private int frameApplied = 0;
        [SerializeField] private bool continuous = false;
        [SerializeField] private bool SetAbsolute = false;
        [SerializeField] private bool SetCapped = false;
        [SerializeField] private bool DamageScaled = false;
        [SerializeField] private bool applyX = true;
        [SerializeField] private bool applyY = true;
        [SerializeField] private Vector2 velocity = new Vector2();
        private int countDown = 0;
        private bool applied = false;
        private float directionModifier = 1f;
        private float damageMultiplier = 1f;
        private PlayerController playerController;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerController = animator.GetComponentInChildren<PlayerController>();
            countDown = frameApplied;
            applied = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (countDown != 0 && !applied)
            {
                countDown--;
            }
            else if (!applied || continuous)
            {
                applied = true;
                directionModifier = 1f;
                damageMultiplier = 1f;

//                if (DamageScaled)
//                {
//                    damageMultiplier = playerController.shield;
//                }

                if (!playerController.facingRight)
                {
                    directionModifier = -1f;
                }

                if (SetAbsolute)
                {
                    if (applyX && applyY)
                    {
                        playerController.IncrementVelocity(velocity.x*directionModifier*damageMultiplier - playerController.GetVelocityX(), velocity.y*damageMultiplier - playerController.GetVelocityY());
                    }
                    else if (applyX)
                    {
                        playerController.IncrementVelocityX(velocity.x*directionModifier*damageMultiplier - playerController.GetVelocityX());
                    }
                    else if (applyY)
                    {
                        playerController.IncrementVelocityY(velocity.y*damageMultiplier - playerController.GetVelocityY());
                    }
                }
                else if (SetCapped)
                {
                    if (applyX && applyY)
                    {
                        playerController.CappedSetVelocity(velocity.x*directionModifier*damageMultiplier, velocity.y*damageMultiplier);
                    }
                    else if (applyX)
                    {
                        playerController.CappedSetVelocityX(velocity.x*directionModifier*damageMultiplier);
                    }
                    else if (applyY)
                    {
                        playerController.CappedSetVelocityY(velocity.y*damageMultiplier);
                    }
                }
                else
                {
                    if (applyX && applyY)
                    {
                        playerController.IncrementVelocity(velocity.x*directionModifier*damageMultiplier, velocity.y*damageMultiplier);
                    }
                    else if (applyX)
                    {
                        playerController.IncrementVelocityX(velocity.x*directionModifier*damageMultiplier);
                    }
                    else if (applyY)
                    {
                        playerController.IncrementVelocityY(velocity.y*damageMultiplier);
                    }
                }
            }
        }       
    }
}