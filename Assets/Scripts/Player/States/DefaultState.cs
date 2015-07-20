using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class DefaultState : PlayerState
    {
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