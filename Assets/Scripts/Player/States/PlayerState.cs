using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public abstract class PlayerState : StateMachineBehaviour, IPlayerState
    {
        protected PlayerController2 playerController;

        public abstract string GetName();

        public abstract void Jump();
        
        public abstract void Move(float h, float v);

        public abstract void Action1(float h, float v);

        public abstract void Action2(float h, float v);

        public abstract void Block();

        public abstract void Throw();
        
        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            playerController = animator.GetComponent<PlayerController2>();
            playerController.currentPlayerState = this;
        }

    }
}