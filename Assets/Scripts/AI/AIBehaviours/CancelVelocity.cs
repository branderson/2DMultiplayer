﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviours
{
    public class CancelVelocity : AIBehaviour
    {
        public override void Process(List<Transform> opponentPositions)
        {
            if (Mathf.Abs(playerController.velocityX) > 2)
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