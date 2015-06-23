using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Assets.Scripts;

namespace Assets.Scripts.Player.States
{
    public class IdleState : PlayerState
    {
        private bool jump = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
        }

        // onstateupdate is called on each update frame between onstateenter and onstateexit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (jump)
            {
                
                animator.SetBool("Ground", false);
                animator.SetFloat("vSpeed", -10);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        public override void Jump()
        {
            this.jump = true;
        }

        public void Move(float h, float v)
        {
        }

        public void Action1(float h, float v)
        {
        }

        public void Action2(float h, float v)
        {
        }

        public void Block()
        {
        }

        public void Throw()
        {
        }

        public override PlayerController2 playerController
        {
            set { throw new System.NotImplementedException(); }
        }

        public override string GetName()
        {
            return "Idle";
        }
    }
}