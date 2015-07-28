using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player.States
{
    public class VibrateController : StateMachineBehaviour
    {
        [SerializeField] private int startingFrame = 0;
        [SerializeField] private int frames = 0;
        [SerializeField] private bool continuous = false;
        [SerializeField] private float leftMotor = 0f;
        [SerializeField] private float rightMotor = 0f;
        PlayerController playerController;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerController = animator.GetComponentInChildren<PlayerController>();
            playerController.SetVibrate(frames, leftMotor, rightMotor);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (continuous)
            {
                playerController.SetVibrate(frames, leftMotor, rightMotor);
            }
        }
    }
}