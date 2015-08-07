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
                MonoBehaviour.print("Not timedDisable");
                Disable();
            }
        }

        public override void Disable()
        {
            base.Disable();
            PlayerInputController.Secondary();
        }
    }
}