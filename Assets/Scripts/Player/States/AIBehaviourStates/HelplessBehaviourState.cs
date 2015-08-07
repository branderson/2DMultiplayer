using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Assets.Scripts.Player.States.AIBehaviours;
using Assets.Scripts.Stage;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviourStates
{
    public class HelplessBehaviourState : AIBehaviourState
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        override public void ProcessAI(List<Transform> opponentPositions)
        {
            if (!playerController.RaycastGround())
            {
//                RaycastHit2D raycast = Physics2D.Raycast(playerController.transform.position, )
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
                Transform closestPlatform = platforms[0].transform;
                float shortestDistance = Mathf.Infinity;
                // TODO: Maybe make the player look for edges? Can't right now because permeable platforms have none but can use special tagged points
                foreach (GameObject platform in platforms)
                {
                    float distance = (platform.transform.position - playerController.transform.position).sqrMagnitude;
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestPlatform = platform.transform;
                    }
                }

                if (closestPlatform.position.x > playerController.transform.position.x)
                {
                    // Above it and to its left
                    if (playerController.facingRight)
                    {
                        ActivateBehaviour(typeof (MoveForward));
                    }
                    else
                    {
                        ActivateBehaviour(typeof (MoveBackward));
                    }
                }
                else
                {
                    // Above it and to its right
                    if (playerController.facingRight)
                    {
                        ActivateBehaviour(typeof (MoveBackward));
                    }
                    else
                    {
                        ActivateBehaviour(typeof (MoveForward));
                    }
                }
            }
            base.ProcessAI(opponentPositions);
        }
    }
}