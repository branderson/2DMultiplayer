using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviours
{
    public class MoveTowardNearestPlayer : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            Vector3 closestOpponentPosition = opponentPositions.OrderByDescending(item => Mathf.Abs(playerController.transform.position.x - item.position.x)).Last().position;
            if (playerController.transform.position.x > closestOpponentPosition.x + 2)
            {
                PlayerInputController.Move(-1f, 0f);              
            }
            else if (playerController.transform.position.x < closestOpponentPosition.x - 2)
            {
                PlayerInputController.Move(1f, 0f);
            }
            if (!TimedDisable)
            {
                Disable();
            }
        }
    }
}