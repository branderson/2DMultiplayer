﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.AI.AIBehaviourStates;
using Assets.Scripts.Player.Triggers;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public abstract class PlayerState : StateMachineBehaviour //, IPlayerState
    {
        [SerializeField] private byte StateID = 0;
        protected PlayerController playerController;
        protected IInputController PlayerInputController;
        protected Animator playerAnimator;
        internal AIBehaviourState AIState;

        public abstract string GetName();

        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            playerController = animator.GetComponentInChildren<PlayerController>();
            PlayerInputController = animator.GetComponentInChildren<IInputController>();
            playerController.SetState(this);
            playerAnimator = animator;
            base.OnStateUpdate(animator, stateinfo, layerindex);
        }

        public virtual new void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Reset unused triggers before next state update
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("Upward");
            animator.ResetTrigger("Downward");
            animator.ResetTrigger("Forward");
            animator.ResetTrigger("Backward");
            animator.ResetTrigger("Primary");
            animator.ResetTrigger("Secondary");
            animator.ResetTrigger("Block");
            animator.ResetTrigger("Grab");
            animator.ResetTrigger("Dodge");
            animator.ResetTrigger("PrimaryReleased");
            animator.ResetTrigger("SecondaryReleased");
            animator.ResetTrigger("BlockReleased");
            animator.ResetTrigger("GrabReleased");
            animator.ResetTrigger("FallThroughFloor");
            animator.ResetTrigger("Helpless");
            animator.ResetTrigger("EdgeGrab");
            animator.ResetTrigger("TurnAround");
            animator.ResetTrigger("LetGo");
            animator.ResetTrigger("Stagger");
            animator.ResetTrigger("Respawn");
            animator.ResetTrigger("Parry");
            animator.ResetTrigger("SlideOut");
        }

        public virtual new void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!(playerController == null))
            {
                playerController.sprite.transform.localPosition = Vector3.zero;
                playerController.StopShaking();          
            }
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        public virtual void Jump()
        {
//            MonoBehaviour.print(GetName());
            playerAnimator.SetTrigger("Jump");
        }

        public virtual void Move(float x, float y)
        {
            playerAnimator.SetFloat("xInput", x);
                
            playerAnimator.SetFloat("yInput", y);
        }

        public virtual void Run()
        {
            playerAnimator.SetBool("Run", true);
        }

        public virtual void Up()
        {
            playerAnimator.SetTrigger("Upward");
        }

        public virtual void Down()
        {
            playerAnimator.SetTrigger("Downward");
        }

        public virtual void Left()
        {
            if (playerController.facingRight)
            {
                playerAnimator.SetTrigger("Backward");
            }
            else
            {
                playerAnimator.SetTrigger("Forward");
            }
        }

        public virtual void Right()
        {
            if (playerController.facingRight)
            {
                playerAnimator.SetTrigger("Forward");
            }
            else
            {
                playerAnimator.SetTrigger("Backward");
            }
        }
        
        // TODO: Can remove x and y from Primary and Secondary
        public virtual void Primary(float x, float y)
        {
            playerAnimator.SetTrigger("Primary");
        }

        public virtual void Secondary(float x, float y)
        {
            playerAnimator.SetTrigger("Secondary");
        }

        public virtual void Block()
        {
            if (playerController.BlockStrength > 0)
            {
                playerAnimator.SetTrigger("Block");
            }
        }

        public virtual void Grab()
        {
            playerAnimator.SetTrigger("Grab");
        }

        public virtual void TakeHit(AttackData attackData)
        {
            
        }

        public virtual void ProcessAI(List<Transform> opponentPositions)
        {
            if (AIState != null)
            {
                AIState.ProcessAI(opponentPositions);
            }
        }

        public byte GetStateID()
        {
            return StateID;
        }
    }
}