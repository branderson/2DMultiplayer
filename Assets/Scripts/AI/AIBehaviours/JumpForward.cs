using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviours
{
    public class JumpForward : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            PlayerInputController.Jump();
            behaviourController.EnableOnNextFrame(typeof(HoldForward));
//            if (playerController.facingRight)
//            {
////                PlayerInputController.MoveX(1);
//            }
//            else
//            {
////                PlayerInputController.MoveX(-1);
//            }
            if (!TimedDisable)
            {
                Disable();
            }
        }
    }
}