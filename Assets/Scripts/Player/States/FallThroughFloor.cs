using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class FallThroughFloor : PlayerState
    {

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateEnter(animator, stateInfo, layerIndex);
//            PlayerController.SetGroundCollisions(false);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
//            PlayerController.passThroughFloor = true; // TODO: Remove this variable
//            PlayerController.SetGroundCollisions(false);
            // TODO: Player not falling at all
            // TODO: Seemingly, OnGround is becoming true too early
//            if (!PlayerController.CheckForCeiling() && !PlayerController.CheckForGround())
//            {
//                PlayerController.SetGroundCollisions(false);
//                PlayerController.passThroughFloor = false;
//            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateExit(animator, stateInfo, layerIndex);
        }

        public override string GetName()
        {
            return "FallThroughFloor";
        }
    }
}