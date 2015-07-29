using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class RestoreRecovery : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.canRecover = true;
        }       
    }
}