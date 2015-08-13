using UnityEngine;

namespace Assets.Scripts.Player.StateBehaviours
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