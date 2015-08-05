using System.Collections.Generic;
using System.Linq;
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

        private List<Transform> opponentPositions = new List<Transform>(); 

        private bool hanging = false;

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
            int situationID = aiLearner.GenerateSituationIndex(playerController);
            CaseBase currentCase = aiLearner.LookupSituationIndex(situationID);
            if (currentCase != null)
            {
                usedGhost = true;
                print("Using ghost AI");
                List<byte> chosenButtonPressResponses = null;
                List<byte> chosenButtonHoldResponses = null;
                sbyte[] chosenAnalogResponses = null;
                float xAnalog = 0;
                float yAnalog = 0;

                if (currentCase.ButtonPressResponseList.Any())
                {
                    chosenButtonPressResponses =
                        currentCase.ButtonPressResponseList.OrderByDescending(item => item.Value).First().Key;
                }
                if (currentCase.ButtonHoldResponseList.Any())
                {
                    chosenButtonHoldResponses =
                        currentCase.ButtonHoldResponseList.OrderByDescending(item => item.Value).First().Key;
                }
                if (currentCase.AnalogResponseList.Any())
                {
                    chosenAnalogResponses = currentCase.AnalogResponseList.OrderByDescending(item => item.Value).First().Key;
                }
//                foreach (KeyValuePair<List<byte>, int> response in currentCase.ButtonPressResponseList)
//                {
//                    // Use difficulty and ratios here to determine response
//                    chosenButtonPressResponses = response.Key;
//                }
//                foreach (KeyValuePair<List<byte>, int> response in currentCase.ButtonHoldResponseList)
//                {
//                    // Use difficulty and ratios here to determine response
//                    chosenButtonHoldResponses = response.Key;
//                }
//                foreach (KeyValuePair<sbyte[], int> response in currentCase.AnalogResponseList)
//                {
//                    chosenAnalogResponses = response.Key;
//                }
                if (chosenButtonPressResponses != null)
                {
                    bool setBlock = false;
                    foreach (byte button in chosenButtonPressResponses)
                    {
                        switch (button)
                        {
                            case 0:
                                inputController.Primary();
                                break;
                            case 1:
                                inputController.Secondary();
                                break;
                            case 2:
                                inputController.Jump();
                                break;
                            case 3:
                                break;
                            case 4:
                                setBlock = true;
                                inputController.SetBlock(true);
                                break;
                            case 5:
                                break;
                            case 6:
                                break;
                            case 7:
                                break;
                        }
                    }
                    if (!setBlock)
                    {
                        inputController.SetBlock(false);
                    }
                }
                if (chosenButtonHoldResponses != null)
                {
//                    bool setBlock = false;
//                    foreach (byte button in chosenButtonHoldResponses)
//                    {
//                        switch (button)
//                        {
//                            case 0:
//                                inputController.Primary();
//                                break;
//                            case 1:
//                                inputController.Secondary();
//                                break;
//                            case 2:
//                                inputController.Jump();
//                                break;
//                            case 3:
//                                break;
//                            case 4:
//                                setBlock = true;
//                                inputController.SetBlock(true);
//                                break;
//                            case 5:
//                                break;
//                            case 6:
//                                break;
//                            case 7:
//                                break;
//                        }
//                    }
//                    if (!setBlock)
//                    {
//                        inputController.SetBlock(false);
//                    }
                }
                if (chosenAnalogResponses != null)
                {
                    switch (chosenAnalogResponses[0])
                    {
                        case 2:
                            inputController.MoveX(1);
                            break;
                        case -2:
                            inputController.MoveX(-1);
                            break;
                        case 1:
                            inputController.MoveX(.4f);
                            break;
                        case -1:
                            inputController.MoveX(-.4f);
                            break;
                        default:
                            inputController.MoveX(0);
                            break;
                    }
                    switch (chosenAnalogResponses[1])
                    {
                        case 2:
                            inputController.MoveY(1);
                            break;
                        case -2:
                            inputController.MoveY(-1);
                            break;
                        case 1:
                            inputController.MoveY(.4f);
                            break;
                        case -1:
                            inputController.MoveY(-.4f);
                            break;
                        default:
                            inputController.MoveY(0);
                            break;
                    }
                }
            }

            // Run AI system
            if (playerController.GetState() != null && !usedGhost)
            {
                print("Processing AI manually");
//                playerController.GetState().ProcessAI(opponentPositions);
            }
        }
    }
}