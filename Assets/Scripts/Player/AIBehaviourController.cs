using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof (PlayerController))]
    [RequireComponent(typeof (AIInputController))]
    public class AIBehaviourController : MonoBehaviour
    {
        private PlayerController playerController;
        private AIInputController inputController;
        private AILearner aiLearner;
        internal int difficulty = 1;

        private CaseBase currentCase;
        private int ghostFrame = 0;

        private List<Transform> opponentPositions = new List<Transform>(); 

        public void Init(AIInputController input, PlayerController controller)
        {
            inputController = input;
            playerController = controller;
            opponentPositions = new List<Transform>();
        }

        public void Awake()
        {
            aiLearner = FindObjectOfType<AILearner>();
        }

        public void Start()
        {
        }

        public void Update()
        {
            opponentPositions = playerController.opponents.Select(item => item.transform).ToList();

            // Ghost AI system
            bool usedGhost = false;
            if (ghostFrame == 0)
            {
                int situationID = aiLearner.GenerateSituationIndex(playerController);
                currentCase = aiLearner.LookupSituationIndex(situationID, playerController.characterName);
            }
            if (currentCase != null)
            {
                usedGhost = true;
//                print("Using ghost AI");
                int chosenResponse = 0;

                if (currentCase.ResponseState[ghostFrame].Any())
                {
                    chosenResponse = currentCase.ResponseState[ghostFrame].OrderByDescending(item => item.Value).First().Key;
                }
                if (chosenResponse != 0)
                {
                    DecodeResponse(chosenResponse);
                }
//                print(ghostFrame);
//                BLF.PrintBinary(chosenResponse);
                ghostFrame++;
                if (ghostFrame >= CaseBase.RecordFrames)
                {
                    ghostFrame = 0;
                }
            }

            // Run AI system
            if (playerController.GetState() != null && !usedGhost)
            {
//                print("Processing AI manually");
//                playerController.GetState().ProcessAI(opponentPositions);
            }
        }

        private void DecodeResponse(int chosenResponse)
        {
            bool setBlock = false;
//            BLF.PrintBinary(chosenResponse);
            if (BLF.IsBitSet(chosenResponse, 0))
            {
                inputController.Primary();
            }
            if (BLF.IsBitSet(chosenResponse, 1))
            {
                inputController.Secondary();
            }
            if (BLF.IsBitSet(chosenResponse, 2))
            {
                inputController.Jump();
            }
            if (BLF.IsBitSet(chosenResponse, 3))
            {
            }
            if (BLF.IsBitSet(chosenResponse, 4))
            {
                setBlock = true;
                inputController.SetBlock(true);
            }
            if (BLF.IsBitSet(chosenResponse, 5))
            {
                MonoBehaviour.print("Pressed Grab");
                inputController.Grab();
            }
            if (BLF.IsBitSet(chosenResponse, 6))
            {
            }
            if (BLF.IsBitSet(chosenResponse, 7))
            {
            }
            if (BLF.IsBitSet(chosenResponse, 8))
            {
                inputController.SetButtonActive("Primary");
            }
            if (BLF.IsBitSet(chosenResponse, 9))
            {
                inputController.SetButtonActive("Secondary");
            }
            if (BLF.IsBitSet(chosenResponse, 10))
            {
                inputController.SetButtonActive("Jump");
            }
            if (BLF.IsBitSet(chosenResponse, 11))
            {
            }
            if (BLF.IsBitSet(chosenResponse, 12))
            {
                inputController.SetButtonActive("Block");
            }
            if (BLF.IsBitSet(chosenResponse, 13))
            {
                MonoBehaviour.print("Holding Grab");
                inputController.SetButtonActive("Grab");
            }
            if (BLF.IsBitSet(chosenResponse, 14))
            {
                inputController.SetButtonActive("TiltLock");
            }
            if (BLF.IsBitSet(chosenResponse, 15))
            {
                inputController.Run(true);
            }
            else
            {
                inputController.Run(false);
            }
            if (BLF.IsBitSet(chosenResponse, 16))
            {
                inputController.MoveX(1);
            }
            if (BLF.IsBitSet(chosenResponse, 17))
            {
                inputController.MoveX(.4f);
            }
            if (BLF.IsBitSet(chosenResponse, 18))
            {
                inputController.MoveX(-1);
            }
            if (BLF.IsBitSet(chosenResponse, 19))
            {
                inputController.MoveX(-.4f);
            }
            if (BLF.IsBitSet(chosenResponse, 20))
            {
                inputController.MoveY(1);
            }
            if (BLF.IsBitSet(chosenResponse, 21))
            {
                inputController.MoveY(.4f);
            }
            if (BLF.IsBitSet(chosenResponse, 22))
            {
                inputController.MoveY(-1);
            }
            if (BLF.IsBitSet(chosenResponse, 23))
            {
                inputController.MoveY(-.4f);
            }
            if (!setBlock)
            {
                inputController.SetBlock(false);
            }
        }
    }
}