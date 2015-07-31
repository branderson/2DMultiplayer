using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class GrabState : PlayerState
    {
        private GameObject playerGrabReach;
        private bool grabReleased = false;

        public void Awake()
        {
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (playerGrabReach == null)
            {
                playerGrabReach = playerController.transform.parent.Find("PlayerGrabReach").gameObject;
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
        }

        public override string GetName()
        {
            return "GrabState";
        }
    }
}