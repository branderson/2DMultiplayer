using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviours
{
    public class ForwardSideRecovery : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            PlayerInputController.Secondary();
            if (playerController.facingRight)
            {
                PlayerInputController.MoveX(1);
            }
            else
            {
                PlayerInputController.MoveX(-1);
            }
            if (!TimedDisable)
            {
                Disable();
            }
        }
    }
}