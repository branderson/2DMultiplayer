using UnityEngine;

namespace Assets.Scripts.Player.StateBehaviours
{
    public class ApplyFacingRelativeVelocity : StateMachineBehaviour
    {
        [SerializeField] private int frameApplied = 0;
        [SerializeField] private bool continuous = false;
        [SerializeField] private bool SetAbsolute = false;
        [SerializeField] private bool SetCapped = false;
        [SerializeField] private bool applyX = true;
        [SerializeField] private bool applyY = true;
        [SerializeField] private Vector2 velocity = new Vector2();
        private int countDown = 0;
        private bool applied = false;
        private float directionModifier = 1f;
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

                if (!playerController.facingRight)
                {
                    directionModifier = -1f;
                }

                if (SetAbsolute)
                {
                    if (applyX && applyY)
                    {
                        playerController.SetVelocity(velocity.x*directionModifier, velocity.y);
                    }
                    else if (applyX)
                    {
                        playerController.SetVelocityX(velocity.x*directionModifier);
                    }
                    else if (applyY)
                    {
                        playerController.SetVelocityY(velocity.y);
                    }
                }
                else if (SetCapped)
                {
                    if (applyX && applyY)
                    {
                        playerController.CappedSetVelocity(velocity.x*directionModifier, velocity.y);
                    }
                    else if (applyX)
                    {
                        playerController.CappedSetVelocityX(velocity.x*directionModifier);
                    }
                    else if (applyY)
                    {
                        playerController.CappedSetVelocityY(velocity.y);
                    }
                }
                else
                {
                    if (applyX && applyY)
                    {
                        playerController.IncrementVelocity(velocity.x*directionModifier, velocity.y);
                    }
                    else if (applyX)
                    {
                        playerController.IncrementVelocityX(velocity.x*directionModifier);
                    }
                    else if (applyY)
                    {
                        playerController.IncrementVelocityY(velocity.y);
                    }
                }
            }
        }       
    }
}