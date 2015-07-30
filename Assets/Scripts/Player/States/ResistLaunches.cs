using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class ResistLaunches : StateMachineBehaviour
    {
        [SerializeField] private int resistance = 5;
        private PlayerController playerController;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.resistance = resistance;
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            playerController.resistance = 0;
        }
    }
}