using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class ApplyFacingRelativeVelocity : StateMachineBehaviour
    {
        [SerializeField] private int frameApplied = 0;
        [SerializeField] private bool continuous = false;
        [SerializeField] private bool SetAbsolute = false;
        [SerializeField] private bool DamageScaled = false;
        [SerializeField] private Vector2 velocity = new Vector2();
        private int countDown = 0;
        private bool applied = false;
        private float directionModifier = 1f;
        private float damageMultiplier = 1f;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
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
                PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
                directionModifier = 1f;
                damageMultiplier = 1f;

                if (DamageScaled)
                {
                    damageMultiplier = playerController.DamageRatio;
                }

                if (!playerController.facingRight)
                {
                    directionModifier = -1f;
                }

                if (SetAbsolute)
                {
                    playerController.IncrementVelocity(velocity.x*directionModifier*damageMultiplier - playerController.GetVelocityX(), velocity.y*damageMultiplier - playerController.GetVelocityY());
                }
                else
                {
                    playerController.IncrementVelocity(velocity.x*directionModifier*damageMultiplier, velocity.y*damageMultiplier);
                }
            }
        }       
    }
}