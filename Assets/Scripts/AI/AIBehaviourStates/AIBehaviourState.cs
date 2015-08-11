using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AI.AIBehaviours;
using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviourStates
{
    public abstract class AIBehaviourState : StateMachineBehaviour
    {
        protected PlayerController playerController;
        protected AIInputController PlayerInputController;
        protected Animator playerAnimator;
        protected AIBehaviourController behaviourController;
        protected PlayerState playerState;
        protected AIBehaviour[] behaviours;


        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            playerController = animator.GetComponentInChildren<PlayerController>();
            PlayerInputController = animator.GetComponentInChildren<AIInputController>();
            playerAnimator = animator;
            behaviourController = animator.GetComponentInChildren<AIBehaviourController>();
            playerState = playerController.GetState();
            playerState.AIState = this;
            if (behaviours == null)
            {
                behaviours = playerAnimator.GetBehaviours<AIBehaviour>();
            }
        }

        public virtual new void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerState == null)
            {
                return;
            }
            playerState.AIState = null;
        }

        public virtual void ProcessAI(List<Transform> opponentPositions)
        {
            foreach (AIBehaviour behaviour in behaviours)
            {
                if (behaviour.IsActive())
                {
                    behaviour.Process(opponentPositions);
                    if (behaviour.TimedDisable)
                    {
                        behaviour.CountDown();
                    }
                }
            }
        }

        public void ActivateBehaviour(Type behaviourType)
        {
            AIBehaviour pendingBehaviour = behaviours.FirstOrDefault(item => item.GetType() == behaviourType && item.InState);
            if (pendingBehaviour != null)
            {
                if (!pendingBehaviour.IsActive())
                {
//                    MonoBehaviour.print("Activating behaviour");
                    pendingBehaviour.Enable();
                }
            }
        }

        public void ActivateBehaviour(Type behaviourType, int frames)
        {
            AIBehaviour pendingBehaviour = behaviours.FirstOrDefault(item => item.GetType() == behaviourType && item.InState);
            if (pendingBehaviour != null)
            {
                if (!pendingBehaviour.IsActive())
                {
//                    MonoBehaviour.print("Activating behaviour");
                    pendingBehaviour.Enable(frames);
                }
            }
        }

        public void DeactivateBehaviour(Type behaviourType)
        {
            AIBehaviour pendingBehaviour = behaviours.FirstOrDefault(item => item.GetType() == behaviourType && item.InState);
            if (pendingBehaviour != null)
            {
                if (pendingBehaviour.IsActive())
                {
//                    MonoBehaviour.print("Deactivating behaviour");
                    pendingBehaviour.Disable();
                }
            }
        }
    }
}