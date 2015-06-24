using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class JumpState : PlayerState
    {
        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            base.OnStateEnter(animator, stateinfo, layerindex);
            animator.SetBool("Ground", false);
            playerController.onGround = false;
            playerController.rigidBody.velocity = new Vector2(playerController.rigidBody.velocity.x, playerController.jumpSpeed);
            MonoBehaviour.print(playerController.rigidBody.velocity.y);
            MonoBehaviour.print("In jump state");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

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

        public override void Jump()
        {
            
        }

        public override string GetName()
        {
            return "Jump";
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