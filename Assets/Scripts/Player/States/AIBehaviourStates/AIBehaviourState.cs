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
            playerState.AIState = null;
        }

        public virtual void ProcessAI(List<Transform> opponentPositions)
        {
            foreach (AIBehaviour behaviour in behaviours)
            {
                if (behaviour.IsActive())
                {
                    behaviour.Process(opponentPositions);
                }
            }
        }

        public void ActivateBehaviour(Type behaviourType)
        {
            AIBehaviour pendingBehaviour = behaviours.FirstOrDefault(item => item.GetType() == behaviourType && item.InState);
            if (pendingBehaviour != null)
            {
                pendingBehaviour.Enable();
            }
        }

        public void DeactivateBehaviour(Type behaviourType)
        {
            AIBehaviour pendingBehaviour = behaviours.FirstOrDefault(item => item.GetType() == behaviourType && item.InState);
            if (pendingBehaviour != null)
            {
                pendingBehaviour.Disable();
            }
        }
    }
}