using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class DisableGravity : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.canFall = false;
        }       
        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.canFall = true;
        }       
    }
}