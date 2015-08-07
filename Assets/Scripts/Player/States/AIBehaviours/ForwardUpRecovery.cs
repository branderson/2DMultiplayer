using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviours
{
    public class ForwardUpRecovery : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            MonoBehaviour.print("ForwardUpRecovery");
            PlayerInputController.MoveY(1);
            PlayerInputController.Secondary();
            if (!TimedDisable)
            {
                Disable();
            }
        }
    }
}