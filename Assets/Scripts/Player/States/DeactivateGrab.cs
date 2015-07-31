using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class DeactivateGrab : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GameObject playerGrabReach = animator.transform.Find("PlayerGrabReach").gameObject;
            playerGrabReach.SetActive(false);
        }       
    }
}