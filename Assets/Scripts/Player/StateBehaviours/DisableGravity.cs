using UnityEngine;

namespace Assets.Scripts.Player.StateBehaviours
{
    public class DisableGravity : StateMachineBehaviour
    {
        [SerializeField] private int startFrame = 0;
        private int countdown = 0;
        private bool applied = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            countdown = startFrame;
            applied = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (countdown == 0 && !applied)
            {
                PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
                playerController.canFall = false;
                applied = true;
            }
            else if (!applied)
            {
                countdown--;
            }
        }
        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.canFall = true;
        }       
    }
}