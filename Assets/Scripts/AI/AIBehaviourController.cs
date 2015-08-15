using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.AI
{
    [RequireComponent(typeof (PlayerController))]
    [RequireComponent(typeof (AIInputController))]
    public class AIBehaviourController : MonoBehaviour
    {
        private static System.Random random = new System.Random();
        internal int difficulty = 1;
        private PlayerController playerController;
        private AIInputController inputController;
        private AILearner aiLearner;
        private GameManager gameManager;

        private CaseBase currentCase;
        private byte ghostFrame = 0;

        private List<Type> nextStateBehaviours = new List<Type>(); 
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
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }

        public void Start()
        {
        }

        public void Update()
        {
            opponentPositions = playerController.opponents.Select(item => item.transform).ToList();

            // Ghost AI system
            bool usedGhost = false;
            if (gameManager.GameConfig.UseGhostAI)
            {
                if (playerController.RaycastGround()) // && ghostFrame == 0);
                {
                    int situationID = aiLearner.GenerateSituationIndex(playerController);
                    currentCase = aiLearner.LookupSituationIndex(situationID, playerController.characterName);
                    // Don't execute ineffective sequences
                    if (currentCase != null)
                    {
                        if (currentCase.ResponseStateList.All(item => item.Effectiveness <= 0))
                        {
                            currentCase = null;
                        }
                        else
                        {
                            ghostFrame = 0;
                        }
//                        else
//                        {
//                            print("Encountered case " + currentCase.ResponseStateList.First().Effectiveness);
//                        }
                    }
                }
                // Is the current situation found in the database
                if (currentCase != null && playerController.RaycastGround())
                {
                    usedGhost = true;
                    int chosenResponse = 0;
                    // Figure out what response sequence to pick
                    if (currentCase.ResponseStateList.Any())
                    {
                        chosenResponse = ChooseResponse();
                    }
                    // Execute response
                    if (chosenResponse != 0)
                    {
                        DecodeResponse(chosenResponse);
                    }
                    if (ghostFrame >= CaseBase.RecordFrames)
                    {
                        inputController.ClearActiveButtons();
                        ghostFrame = 0;
                    }
                }
            }

            // Procedural AI system
            if (playerController.GetState() != null && !usedGhost)
            {
//                print("Processing AI manually");
                if (playerController.GetState().AIState != null)
                {
                    foreach (Type behaviour in nextStateBehaviours)
                    {
                        playerController.GetState().AIState.ActivateBehaviour(behaviour);
                    }
                    nextStateBehaviours.Clear();
                    playerController.GetState().ProcessAI(opponentPositions);
                }
            }
        }

        private short ChooseResponse()
        {
            // Difficulty-based logic to pick how effective the selection should be
            int difficultyModifier = currentCase.ResponseStateList.Count - difficulty + 1; // Higher difficulty has less possible choices
            if (difficultyModifier < 0)
            {
                difficultyModifier = 0;
            }
            int sequenceIndex = random.Next(difficultyModifier);

            // Perform most common version of selected input sequence
            CaseBase.ControllerStateSet currentSet = currentCase.ResponseStateList[sequenceIndex].GetVersions().First().Key;
//                        print("Effectiveness: " + currentCase.ResponseStateList[sequenceIndex].Effectiveness + ", Index " + sequenceIndex);
            if (currentSet.NewStateAtFrame(ghostFrame))
            {
                return currentSet.GetStateAtFrame(ghostFrame++);
            }
            ghostFrame += 1;
            if (currentSet.PassedLastState(ghostFrame))
            {
                //                        print("Ending sequence early");
                ghostFrame = 0;
                inputController.ClearActiveButtons();
            }
            return 0;
        }

        private void DecodeResponse(int chosenResponse)
        {
            bool setBlock = false;
//            BLF.PrintBinary(chosenResponse);
            if (BLF.IsBitSet(chosenResponse, 0))
            {
                inputController.SetButtonActive("Primary");
            }
            else
            {
                inputController.SetButtonInactive("Primary");
            }
            if (BLF.IsBitSet(chosenResponse, 1))
            {
                inputController.SetButtonActive("Secondary");
            }
            else
            {
                inputController.SetButtonInactive("Secondary");
            }
            if (BLF.IsBitSet(chosenResponse, 2))
            {
                inputController.SetButtonActive("Jump");
            }
            else
            {
                inputController.SetButtonInactive("Jump");
            }
            if (BLF.IsBitSet(chosenResponse, 3))
            {
            }
            if (BLF.IsBitSet(chosenResponse, 4))
            {
                inputController.SetButtonActive("Block");
            }
            else
            {
                inputController.SetButtonInactive("Block");
            }
            if (BLF.IsBitSet(chosenResponse, 5))
            {
                inputController.SetButtonActive("Grab");
            }
            else
            {
                inputController.SetButtonInactive("Grab");
            }
            if (BLF.IsBitSet(chosenResponse, 6))
            {
                inputController.SetButtonActive("TiltLock");
            }
            else
            {
                inputController.SetButtonInactive("TiltLock");
            }
            if (BLF.IsBitSet(chosenResponse, 7))
            {
                inputController.Run(true);
            }
            else
            {
                inputController.Run(false);
            }
            if (BLF.IsBitSet(chosenResponse, 8))
            {
                inputController.MoveX(1);
            }
            else if (BLF.IsBitSet(chosenResponse, 9))
            {
                inputController.MoveX(.4f);
            }
            else if (BLF.IsBitSet(chosenResponse, 10))
            {
                inputController.MoveX(-1);
            }
            else if (BLF.IsBitSet(chosenResponse, 11))
            {
                inputController.MoveX(-.4f);
            }
            else
            {
                inputController.MoveX(0);
            }
            if (BLF.IsBitSet(chosenResponse, 12))
            {
                inputController.MoveY(1);
            }
            else if (BLF.IsBitSet(chosenResponse, 13))
            {
                inputController.MoveY(.4f);
            }
            else if (BLF.IsBitSet(chosenResponse, 14))
            {
                inputController.MoveY(-1);
            }
            else if (BLF.IsBitSet(chosenResponse, 15))
            {
                inputController.MoveY(-.4f);
            }
            else
            {
                inputController.MoveY(0);
            }
            if (!setBlock)
            {
                inputController.SetBlock(false);
            }
        }

        public void EnableOnNextFrame(Type behaviour)
        {
            nextStateBehaviours.Add(behaviour);
        }
    }
}