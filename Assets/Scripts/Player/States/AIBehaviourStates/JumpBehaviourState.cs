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
    public class JumpBehaviourState : AIBehaviourState
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        override public void ProcessAI(List<Transform> opponentPositions)
        {
//            Vector3 closestOpponentPosition = opponentPositions.OrderByDescending(item => Mathf.Abs(playerController.transform.position.x - item.position.x)).Last().position;

//            if (Mathf.Abs(closestOpponentPosition.x - playerController.transform.position.x) > 2f)
//            {
//                ActivateBehaviour(typeof (MoveTowardNearestPlayer));
//            }
//            else
//            {
//                DeactivateBehaviour(typeof(MoveTowardNearestPlayer));
//            }
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

                if (playerController.AirJumps > 0)
                {
                    if (closestPlatform.position.y > playerController.transform.position.y)
                    {
                        if (closestPlatform.position.x > playerController.transform.position.x)
                        {
                            MonoBehaviour.print("Need to go right");
                            // Below it and to its left
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (JumpForward), 3);
                            }
                            else
                            {
                                ActivateBehaviour(typeof (JumpBackward), 3);
                            }
                        }
                        else
                        {
                            MonoBehaviour.print("Need to go left");
                            // Below it and to its right
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (JumpBackward), 3);
                            }
                            else
                            {
                                ActivateBehaviour(typeof (JumpForward), 3);
                            }
                        }
                    }
                    else
                    {
                        if (closestPlatform.position.x > playerController.transform.position.x)
                        {
                            MonoBehaviour.print("Need to go right");
                            // Above it and to its left
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (JumpForward), 3);
                            }
                            else
                            {
                                ActivateBehaviour(typeof (JumpBackward), 3);
                            }
                        }
                        else
                        {
                            MonoBehaviour.print("Need to go left");
                            // Above it and to its right
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (JumpBackward), 3);
                            }
                            else
                            {
                                ActivateBehaviour(typeof (JumpForward), 3);
                            }
                        }
                    }
                }
                else
                {
                    // Need to use a recovery
                    if (closestPlatform.position.y > playerController.transform.position.y)
                    {
                        if (closestPlatform.position.x > playerController.transform.position.x)
                        {
                            MonoBehaviour.print("Need to go right");
                            // Below it and to its left
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (ForwardUpRecovery), 3);
                            }
                            else
                            {
                                ActivateBehaviour(typeof (BackwardUpRecovery), 3);
                            }
                        }
                        else
                        {
                            MonoBehaviour.print("Need to go left");
                            // Below it and to its right
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (BackwardUpRecovery), 3);
                            }
                            else
                            {
                                ActivateBehaviour(typeof (ForwardUpRecovery), 3);
                            }
                        }
                    }
                    else
                    {
                        if (closestPlatform.position.x > playerController.transform.position.x)
                        {
                            MonoBehaviour.print("Need to go right");
                            // Above it and to its left
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (ForwardSideRecovery), 3);
                            }
                            else
                            {
                                ActivateBehaviour(typeof (BackwardSideRecovery), 3);
                            }
                        }
                        else
                        {
                            MonoBehaviour.print("Need to go left");
                            // Above it and to its right
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (BackwardSideRecovery), 3);
                            }
                            else
                            {
                                ActivateBehaviour(typeof (ForwardSideRecovery), 3);
                            }
                        }
                    }
                }
            }
            base.ProcessAI(opponentPositions);
        }
    }
}