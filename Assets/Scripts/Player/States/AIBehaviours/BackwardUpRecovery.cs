using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviours
{
    public class BackwardUpRecovery : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            MonoBehaviour.print("BackwardUpRecovery");
            PlayerInputController.MoveY(1);
            PlayerInputController.Secondary();
            playerAnimator.SetTrigger("TurnAround");
            if (!TimedDisable)
            {
                Disable();
            }
        }
    }
}