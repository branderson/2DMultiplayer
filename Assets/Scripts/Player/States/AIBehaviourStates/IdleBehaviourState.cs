using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts.Player.States.AIBehaviours;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviourStates
{
    public class IdleBehaviourState : AIBehaviourState
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        override public void ProcessAI(List<Transform> opponentPositions)
        {
            Vector3 closestOpponentPosition = opponentPositions.OrderByDescending(item => Mathf.Abs(playerController.transform.position.x - item.position.x)).Last().position;
            if (Mathf.Abs(closestOpponentPosition.x - playerController.transform.position.x) > 2f)
            {
                ActivateBehaviour(typeof (MoveTowardNearestPlayer));
            }
            base.ProcessAI(opponentPositions);
        }
    }
}