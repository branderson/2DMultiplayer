using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof (PlayerController))]
    [RequireComponent(typeof (AIInputController))]
    public class AIBehaviourController : MonoBehaviour
    {
        private PlayerController playerController;
        private AIInputController inputController;

        private List<Transform> opponentPositions = new List<Transform>(); 

        private bool hanging = false;

        public void Init(AIInputController input, PlayerController controller)
        {
            inputController = input;
            playerController = controller;
            opponentPositions = new List<Transform>();
        }

        public void Start()
        {
        }

        public void Update()
        {
//            print(playerController.opponents.Count());
            opponentPositions = playerController.opponents.Select(item => item.transform).ToList();
//            Transform nearestOpponent = opponentPositions.

            // Move in opposite direction of launch
            if (!Mathf.Approximately(playerController.velocityX, 0))
            {
                if (playerController.velocityX < -10)
                {
                    inputController.Move(1, 0);
                }
                else if (playerController.velocityX > 10)
                {
                    inputController.Move(-1, 0);
                }
            }

            // Detect hanging
            // TODO: There was no state here
            if (playerController.GetState() != null)
            {
                if (playerController.GetState().GetName() == "EdgeGrabState")
                {
                    hanging = true;
                }
                else
                {
                    hanging = false;
                }
            }

            // Jump if hanging
            if (hanging)
            {
                inputController.Jump();
            }
//
//            if (opponentPositions.Any(item => Mathf.Abs(item.position.x - transform.position.x) < 2 && Mathf.Abs(item.position.y - transform.position.y) < 1))
//            {
//                inputController.SetBlock(true);
//            }
//            else
//            {
//                inputController.SetBlock(false);
//            }
        }
    }
}