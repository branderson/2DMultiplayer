using UnityEngine;

namespace Assets.Scripts.Player.StateBehaviours
{
    public class MoveFacingRelativeDistance : StateMachineBehaviour
    {
        [SerializeField] private int frameApplied = 0;
        [SerializeField] private int endingFrame = 10;
        [SerializeField] private bool handleX = false;
        [SerializeField] private bool handleY = false;
        [SerializeField] private Vector2 distance = new Vector2();
        private Vector2 distancePerFrame = new Vector2();
        private Vector2 velocity = new Vector2();
        private int startCountDown = 0;
        private int motionCountDown = 0;
        private float directionModifier = 1f;
        private PlayerController playerController;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerController = animator.GetComponentInChildren<PlayerController>();
            startCountDown = frameApplied;
            motionCountDown = endingFrame - frameApplied;
            distancePerFrame.x = distance.x/motionCountDown;
            distancePerFrame.y = distance.y/motionCountDown;
            velocity.x = distancePerFrame.x/Time.fixedDeltaTime;
            velocity.y = distancePerFrame.y/Time.fixedDeltaTime;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (startCountDown != 0)
            {
                startCountDown--;
            }
            else if (motionCountDown != 0)
            {
                motionCountDown--;
                directionModifier = 1f;

                if (!playerController.facingRight)
                {
                    directionModifier = -1f;
                }

                // TODO: Might have errors in math with distance
                if (handleX)
                {
                    playerController.CappedSetVelocityX(velocity.x*directionModifier);
                }
                if (handleY)
                {
                    playerController.CappedSetVelocityY(velocity.y);
                }
            }
        }       
    }
}