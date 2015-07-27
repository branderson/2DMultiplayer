using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class StopOnEdge : StateMachineBehaviour
    {
        private bool applied = false;
        private PlayerController playerController;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerController = animator.GetComponentInChildren<PlayerController>();
            applied = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerController.CheckForGround())
            {
                if (playerController.GetVelocityX() > 0f && playerController.onEdgeRight)
                {
                    playerController.IncrementVelocity(-playerController.GetVelocityX(),
                        -playerController.GetVelocityY());
                }
                else if (playerController.GetVelocityX() < 0f && playerController.onEdgeLeft)
                {
                    playerController.IncrementVelocity(-playerController.GetVelocityX(),
                        -playerController.GetVelocityY());
                }
            }
        }       
    }
}