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
        internal int difficulty = 1;

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
            opponentPositions = playerController.opponents.Select(item => item.transform).ToList();

            // Run AI system
            if (playerController.GetState() != null)
            {
                playerController.GetState().ProcessAI(opponentPositions);
            }

            // Ghost AI system
        }
    }
}