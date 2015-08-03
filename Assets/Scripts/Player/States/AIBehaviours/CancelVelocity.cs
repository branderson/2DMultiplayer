using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviours
{
    public class CancelVelocity : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            if (!Mathf.Approximately(playerController.velocityX, 0))
            {
                if (playerController.velocityX < 0)
                {
                    PlayerInputController.Move(1, 0);
                }
                else if (playerController.velocityX > 0)
                {
                    PlayerInputController.Move(-1, 0);
                }
            }
        }
    }
}