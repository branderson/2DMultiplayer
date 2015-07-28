using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof (PlayerController))]
    [RequireComponent(typeof (AIInputController))]
    public class AIBehaviourController : MonoBehaviour
    {
        private PlayerController playerController;
        private AIInputController inputController;

        public void Init(AIInputController input, PlayerController controller)
        {
            inputController = input;
            playerController = controller;
        }

        public void Update()
        {
            if (!Mathf.Approximately(playerController.velocityX, 0))
            {
                if (playerController.velocityX < 0)
                {
                    inputController.Move(1, 0);
                }
                else
                {
                    inputController.Move(-1, 0);
                }
            }
        }
    }
}