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
        private GameManager gameManager;
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
                if (ghostFrame == 0 && playerController.RaycastGround())
                {
                    int situationID = aiLearner.GenerateSituationIndex(playerController);
                    currentCase = aiLearner.LookupSituationIndex(situationID, playerController.characterName);
                    //                if (currentCase != null)
                    //                {
                    //                    print("Entering new case");
                    //                }
                }
                if (currentCase != null && playerController.RaycastGround())
                {
                    usedGhost = true;
                    ////                print("Using ghost AI");
                    int chosenResponse = 0;
                    //
                    if (currentCase.ResponseStateList.Any())
                    {
                        CaseBase.ControllerStateSet currentSet = currentCase.ResponseStateList.First().GetVersions().First().Key;
                        if (currentSet.NewStateAtFrame((byte) ghostFrame))
                        {
                            chosenResponse = currentSet.GetStateAtFrame((byte) ghostFrame);
                        }
                        else if (currentSet.PassedLastState((byte) ghostFrame) && ghostFrame > 10)
                        {
                            //                        print("Ending sequence early");
                            ghostFrame = 0;
                            inputController.ClearActiveButtons();
                            return;
                        }
                    }
                    if (chosenResponse != 0)
                    {
                        //                    print("Response chosen");
                        DecodeResponse(chosenResponse);
                    }
                    ghostFrame++;
                    if (ghostFrame >= CaseBase.RecordFrames)
                    {
                        //                    print("Exiting case");
                        inputController.ClearActiveButtons();
                        ghostFrame = 0;
                    }
                    //                if (currentCase.ResponseState[ghostFrame].Any())
                    //                {
                    //                    chosenResponse = currentCase.ResponseState[ghostFrame].OrderByDescending(item => item.Value).First().Key;
                    //                }
                    //                if (chosenResponse != 0)
                    //                {
                    //                    DecodeResponse(chosenResponse);
                    //                }
                    ////                print(ghostFrame);
                    ////                BLF.PrintBinary(chosenResponse);
                }
            }

            // Run AI system
            if (playerController.GetState() != null && !usedGhost)
            {
//                print("Processing AI manually");
                playerController.GetState().ProcessAI(opponentPositions);
            }
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
//                print("Tilt right");
                inputController.MoveX(.4f);
            }
            else if (BLF.IsBitSet(chosenResponse, 10))
            {
//                print("Full left");
                inputController.MoveX(-1);
            }
            else if (BLF.IsBitSet(chosenResponse, 11))
            {
//                print("Tilt left");
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
    }
}