using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviours
{
    public class JumpBackward : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            MonoBehaviour.print("JumpBackward");
            if (playerController.facingRight)
            {
                PlayerInputController.MoveX(-1);
            }
            else
            {
                PlayerInputController.MoveX(1);
            }
            if (!TimedDisable)
            {
                Disable();
            }
        }

        public override void Disable()
        {
            base.Disable();
            PlayerInputController.Jump();
        }
    }
}