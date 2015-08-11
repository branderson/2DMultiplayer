using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviours
{
    public class HoldForward : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            if (playerController.facingRight)
            {
                PlayerInputController.MoveX(1);
            }
            else
            {
                PlayerInputController.MoveX(-1);
            }
        }
    }
}