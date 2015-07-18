using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class SetInvincible : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.StateInvincible = true;
        }       
        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.StateInvincible = false;
        }       
    }
}