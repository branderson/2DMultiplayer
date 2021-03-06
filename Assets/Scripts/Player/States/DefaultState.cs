﻿using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class DefaultState : PlayerState
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            playerController.CheckForGround();
        }

        public override string GetName()
        {
            return "DefaultState";
        }

        public override void Primary(float x, float y)
        {
            playerAnimator.SetTrigger("Primary");
        }
    }
}