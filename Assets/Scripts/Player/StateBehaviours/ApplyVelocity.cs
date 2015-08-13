using UnityEngine;

namespace Assets.Scripts.Player.StateBehaviours
{
    public class ApplyVelocity : StateMachineBehaviour
    {
        [SerializeField] private bool SetAbsolute = false;
        [SerializeField] private readonly Vector2 velocity = new Vector2();
        private float damageMultiplier = 1f;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController playerController = animator.GetComponentInChildren<PlayerController>();
            if (SetAbsolute)
            {
                playerController.CappedSetVelocity(velocity*damageMultiplier);
//                playerController.IncrementVelocity(velocity*damageMultiplier - playerController.GetVelocity());
            }
            else
            {
                playerController.IncrementVelocity(velocity*damageMultiplier);
            }
        }       
    }
}