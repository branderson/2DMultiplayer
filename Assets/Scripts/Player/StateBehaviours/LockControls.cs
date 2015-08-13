using UnityEngine;

namespace Assets.Scripts.Player.StateBehaviours
{
    public class LockControls : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("LockControls", true);
        }       
        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            animator.SetBool("LockControls", false);
        }       
    }
}