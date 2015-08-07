using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Player.Triggers;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class SmashState : PlayerState
    {
        private ApplyForceTrigger[] forceTriggers;
        private int frames = 0;
        private int minFrames = 20;
        private int charge = 0;
        private float chargeMultiplier = .01f;
        private float damageMultiplier = .1f;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            frames = 0;
            forceTriggers = animator.GetComponentsInChildren<ApplyForceTrigger>(true);
            charge = playerController.SmashCharge;
//            MonoBehaviour.print("Charge: " + charge);
            playerController.SmashCharge = 0;
            foreach (ApplyForceTrigger trigger in forceTriggers)
            {
                trigger.ForceMultiplier = charge*chargeMultiplier + 1;
                trigger.DamageSupplement = (int)Mathf.Floor(charge*damageMultiplier);
                //                trigger.DamageSupplement = (int)charge*chargeMultiplier + 1;
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            frames++;
            if (!PlayerInputController.ButtonActive("Primary") && playerController.Paused && frames > minFrames)
            {
                playerController.ResumeAnimation();
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            animator.GetComponent<AnimationEvents>().DeleteLastInstantiated();
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