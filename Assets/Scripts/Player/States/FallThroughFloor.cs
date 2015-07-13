using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class FallThroughFloor : PlayerState
    {

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            playerController.SetTriggers(true);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            playerController.passThroughFloor = true;
            // TODO: Seemingly, OnGround is becoming true too early
            if (!playerController.CheckForCeiling() && !playerController.CheckForGround())
            {
                playerController.SetTriggers(false);
                playerController.passThroughFloor = false;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateExit(animator, stateInfo, layerIndex);
        }

        public override string GetName()
        {
            throw new System.NotImplementedException();
        }
    }
}