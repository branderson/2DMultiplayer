using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts.Player.States.AIBehaviours;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Assets.Scripts.Player.States.AIBehaviourStates
{
    public abstract class AIBehaviourState : StateMachineBehaviour
    {
        protected PlayerController playerController;
        protected AIInputController PlayerInputController;
        protected Animator playerAnimator;
        protected PlayerState playerState;
        protected AIBehaviour[] behaviours;


        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            playerController = animator.GetComponentInChildren<PlayerController>();
            PlayerInputController = animator.GetComponentInChildren<AIInputController>();
            playerAnimator = animator;
            playerState = animator.GetBehaviour<PlayerState>();
            if (behaviours == null)
            {
                behaviours = playerAnimator.GetBehaviours<AIBehaviour>();
            }
            playerState.AIState = this;
        }

        public virtual new void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerState.AIState = null;
        }

        public virtual void ProcessAI(List<Transform> opponentPositions)
        {
            MonoBehaviour.print(behaviours.Count());
            foreach (AIBehaviour behaviour in behaviours)
            {
                if (behaviour.IsActive())
                {
                    MonoBehaviour.print("Processing something");
                    behaviour.Process(opponentPositions);
                }
                else
                {
                    MonoBehaviour.print("Behaviour inactive");
                }
            }
        }

        public void ActivateBehaviour(Type behaviourType)
        {
            AIBehaviour pendingBehaviour = behaviours.FirstOrDefault(item => item.GetType() == behaviourType);
            if (pendingBehaviour != null)
            {
                pendingBehaviour.Enable();
            }
        }

        public void DeactivateBehaviour(Type behaviourType)
        {
            AIBehaviour pendingBehaviour = behaviours.FirstOrDefault(item => item.GetType() == behaviourType);
            if (pendingBehaviour != null)
            {
                pendingBehaviour.Disable();
            }
        }
    }
}