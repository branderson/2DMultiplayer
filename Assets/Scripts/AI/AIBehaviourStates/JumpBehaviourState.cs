using System.Collections.Generic;
using Assets.Scripts.AI.AIBehaviours;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviourStates
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
                    // If platform is in direction of travel
                    if ((playerController.GetVelocityX() >= 0 &&
                         platform.transform.position.x - playerController.transform.position.x > 0) ||
                        (playerController.GetVelocityX() < 0 &&
                         platform.transform.position.x - playerController.transform.position.x < 0))
                    {
                        float distance = (platform.transform.position - playerController.transform.position).sqrMagnitude;
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            closestPlatform = platform.transform;
                        }
                    }
                }
                // If too far away, try to recover back
                if (closestPlatform.position.x - playerController.transform.position.x > 50)
                {
                    foreach (GameObject platform in platforms)
                    {
                        float distance =
                            (platform.transform.position - playerController.transform.position).sqrMagnitude;
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            closestPlatform = platform.transform;
                        }
                    }
                }

                if (playerController.AirJumps > 0)
                {
                    if (closestPlatform.position.y > playerController.transform.position.y)
                    {
                        if (closestPlatform.position.x > playerController.transform.position.x)
                        {
                            // Below it and to its left
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (JumpForward));
                                ActivateBehaviour(typeof (MoveForward));
                            }
                            else
                            {
                                ActivateBehaviour(typeof (JumpBackward));
                                ActivateBehaviour(typeof (MoveBackward));
                            }
                        }
                        else
                        {
                            // Below it and to its right
                            if (playerController.facingRight)
                            {
//                                MonoBehaviour.print("JumpForward");
                                ActivateBehaviour(typeof (JumpBackward));
                                ActivateBehaviour(typeof (MoveBackward));
                            }
                            else
                            {
//                                MonoBehaviour.print("JumpBackward");
                                ActivateBehaviour(typeof (JumpForward));
                                ActivateBehaviour(typeof (MoveForward));
                            }
                        }
                    }
                    else
                    {
                        if (closestPlatform.position.x > playerController.transform.position.x)
                        {
                            // Above it and to its left
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (JumpForward));
                                ActivateBehaviour(typeof (MoveForward));
                            }
                            else
                            {
                                ActivateBehaviour(typeof (JumpBackward));
                                ActivateBehaviour(typeof (MoveBackward));
                            }
                        }
                        else
                        {
                            // Above it and to its right
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (JumpBackward));
                                ActivateBehaviour(typeof (MoveBackward));
                            }
                            else
                            {
                                ActivateBehaviour(typeof (JumpForward));
                                ActivateBehaviour(typeof (MoveForward));
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
                            // Below it and to its left
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (ForwardUpRecovery));
                                ActivateBehaviour(typeof (MoveForward));
                            }
                            else
                            {
                                ActivateBehaviour(typeof (BackwardUpRecovery));
                                ActivateBehaviour(typeof (MoveBackward));
                            }
                        }
                        else
                        {
                            // Below it and to its right
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (BackwardUpRecovery));
                                ActivateBehaviour(typeof (MoveBackward));
                            }
                            else
                            {
                                ActivateBehaviour(typeof (ForwardUpRecovery));
                                ActivateBehaviour(typeof (MoveForward));
                            }
                        }
                    }
                    else
                    {
                        if (closestPlatform.position.x > playerController.transform.position.x)
                        {
                            // Above it and to its left
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (ForwardSideRecovery));
                                ActivateBehaviour(typeof (MoveForward));
                            }
                            else
                            {
                                ActivateBehaviour(typeof (BackwardSideRecovery));
                                ActivateBehaviour(typeof (MoveBackward));
                            }
                        }
                        else
                        {
                            // Above it and to its right
                            if (playerController.facingRight)
                            {
                                ActivateBehaviour(typeof (BackwardSideRecovery));
                                ActivateBehaviour(typeof (MoveBackward));
                            }
                            else
                            {
                                ActivateBehaviour(typeof (ForwardSideRecovery));
                                ActivateBehaviour(typeof (MoveForward));
                            }
                        }
                    }
                }
            }
            base.ProcessAI(opponentPositions);
        }
    }
}