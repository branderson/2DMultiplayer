using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class VibrateController : StateMachineBehaviour
    {
        [SerializeField] private int startingFrame = 0;
        [SerializeField] private int frames = 0;
        [SerializeField] private float leftMotor = 0f;
        [SerializeField] private float rightMotor = 0f;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.SetVibrate(frames, leftMotor, rightMotor);
        }       
    }
}