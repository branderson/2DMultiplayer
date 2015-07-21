using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class ChargeSmashState : PlayerState
    {
        private int chargeFrames = 0;
        private int maxChargeFrames = 60;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            chargeFrames = 0;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (!PlayerInputController.ButtonActive("Primary") || chargeFrames == maxChargeFrames)
            {
                playerController.SmashCharge = chargeFrames;
                playerAnimator.SetTrigger("PrimaryReleased");
            }
            else
            {
                chargeFrames++;
            }
        }

        public override string GetName()
        {
            return "ChargeSmash";
        }
    }
}