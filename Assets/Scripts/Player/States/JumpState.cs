﻿using UnityEngine;
using System.Collections;
using Assets.Scripts.Player.States;

namespace Assets.Scripts.Player.States
{
    public class JumpState : PlayerState
    {
        private bool jump = false;
        private Vector2 move;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            jump = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerController.canAirJump && jump)
            {
                animator.SetTrigger("Jump");
                playerController.canAirJump = false;
            }
            jump = false;

            if (move.y < 0)
            {
                playerController.fastFall = true;
            }

            playerController.CheckForGround();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        public override string GetName()
        {
            return ("Jump");
        }

        public override void Jump()
        {
            jump = true;
        }

        public override void Move(float x, float y)
        {
            this.move = new Vector2(x, y);

            // Check controller's vertical state for attack and movement modifiers
            //        jump = false;
            //        if (move.y > 0)
            //        {
            //            Jump();
            //        }
            //
            //        else if (move.y < 0)
            //        {
            //            
            //        }
        }

        public override void Action1(float x, float y)
        {
            throw new System.NotImplementedException();
        }

        public override void Action2(float x, float y)
        {
            throw new System.NotImplementedException();
        }

        public override void Block()
        {
            throw new System.NotImplementedException();
        }

        public override void Throw()
        {
            throw new System.NotImplementedException();
        }
    }
}