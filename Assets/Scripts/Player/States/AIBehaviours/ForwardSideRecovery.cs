using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviours
{
    public class ForwardSideRecovery : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            MonoBehaviour.print("ForwardSideRecovery");
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