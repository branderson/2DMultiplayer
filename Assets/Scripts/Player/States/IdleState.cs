using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Assets.Scripts;

namespace Assets.Scripts.Player.States
{
    public class IdleState : PlayerState
    {
        private bool jump = false;

        // onstateupdate is called on each update frame between onstateenter and onstateexit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (jump && playerController.onGround)
            {
                animator.SetTrigger("Jump");
                MonoBehaviour.print("Setting animator values");
            }
            jump = false;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        public override void Jump()
        {
            this.jump = true;
            MonoBehaviour.print("Idle jump");
        }

        public override string GetName()
        {
            return "Idle";
        }

        public override void Move(float h, float v)
        {
            throw new System.NotImplementedException();
        }

        public override void Action1(float h, float v)
        {
            throw new System.NotImplementedException();
        }

        public override void Action2(float h, float v)
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