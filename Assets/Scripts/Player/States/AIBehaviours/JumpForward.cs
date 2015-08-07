using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviours
{
    public class JumpForward : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            MonoBehaviour.print("JumpForward");
            PlayerInputController.Jump();
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