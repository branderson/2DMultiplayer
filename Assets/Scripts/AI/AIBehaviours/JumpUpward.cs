using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviours
{
    public class JumpUpward : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            PlayerInputController.Jump();
            if (!TimedDisable)
            {
                Disable();
            }
        }
    }
}