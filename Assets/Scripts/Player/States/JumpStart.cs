using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class JumpStart : StateMachineBehaviour 
    {
        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            PlayerController2 playerController = animator.GetComponent<PlayerController2>();
            playerController.SetVelocityY(playerController.jumpSpeed);
        }
    }
}