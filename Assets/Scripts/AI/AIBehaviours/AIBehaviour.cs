using System.Collections.Generic;
using Assets.Scripts.AI.AIBehaviourStates;
using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.AI.AIBehaviours
{
    public abstract class AIBehaviour : StateMachineBehaviour
    {
        [SerializeField] protected bool startingActive = true;
        protected bool active;
        internal bool TimedDisable = false;
        private int activeFrames;
        protected PlayerController playerController;
        protected AIInputController PlayerInputController;
        protected Animator playerAnimator;
        protected AIBehaviourController behaviourController;
        protected PlayerState playerState;
        protected AIBehaviourState aiState;
        internal bool InState = false;

        public abstract void Process(List<Transform> opponentPositions);

        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            active = startingActive;
            playerController = animator.GetComponentInChildren<PlayerController>();
            PlayerInputController = animator.GetComponentInChildren<AIInputController>();
            playerState = playerController.GetState();
            behaviourController = animator.GetComponentInChildren<AIBehaviourController>();
            playerAnimator = animator;
            aiState = playerState.AIState;
            InState = true;
        }

        public void CountDown()
        {
            if (TimedDisable)
            {
                if (activeFrames > 0)
                {
                    activeFrames--;
                }
                else
                {
                    Disable();
                }
            }
        }

        public virtual new void OnStateExit(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            active = false;
            InState = false;
        }

        public bool IsActive()
        {
            if (playerController != null && PlayerInputController != null && playerAnimator != null && playerState != null && aiState != null)
            {
                return active;
            }
            return false;
        }

        public void Enable()
        {
            active = true;
        }

        public void Enable(int frames)
        {
            Enable();
            TimedDisable = true;
            activeFrames = frames*2;
        }

        public virtual void Disable()
        {
            active = false;
            TimedDisable = false;
            activeFrames = 0;
        }
    }
}