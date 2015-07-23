using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Player.Triggers;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class SmashState : PlayerState
    {
        private ApplyForceTrigger[] forceTriggers;
        private int charge = 0;
        private float chargeMultiplier = .01f;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            forceTriggers = animator.GetComponentsInChildren<ApplyForceTrigger>(true);
            charge = playerController.SmashCharge;
            MonoBehaviour.print("Charge: " + charge);
            playerController.SmashCharge = 0;
            foreach (ApplyForceTrigger trigger in forceTriggers)
            {
                trigger.ForceMultiplier = charge*chargeMultiplier + 1;
//                trigger.DamageSupplement = (int)charge*chargeMultiplier + 1;
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (ApplyForceTrigger trigger in forceTriggers)
            {
                trigger.ForceMultiplier = 1f;
                trigger.DamageSupplement = 0;
            }
        }

        public override string GetName()
        {
            return "Smash";
        }
    }
}