using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviours
{
    public class ForwardUpRecovery : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            PlayerInputController.MoveY(1);
            PlayerInputController.Secondary();
            if (!TimedDisable)
            {
                Disable();
            }
        }
    }
}