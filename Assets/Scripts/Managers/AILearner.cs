using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class AILearner : MonoBehaviour
    {
        private GameManager gameManager;
        private List<PlayerController> players;
        // Might want to decrease the resolution of some of these ranges to increase chance of finding a matching situation
        private static readonly RangeTree<int, IntRange> xDeltas = new RangeTree<int, IntRange>(new RangeItemComparer());
        private static readonly RangeTree<int, IntRange> yDeltas = new RangeTree<int, IntRange>(new RangeItemComparer()); 
        private static readonly RangeTree<int, IntRange> shieldRanges = new RangeTree<int, IntRange>(new RangeItemComparer()); 
        private static readonly RangeTree<int, IntRange> stateCategories = new RangeTree<int, IntRange>(new RangeItemComparer()); 

        // Map of CaseBases
        private List<KeyValuePair<string, BinaryTree<CaseBase>>> caseTrees = new List<KeyValuePair<string, BinaryTree<CaseBase>>>();
        private Queue<CaseBase> activeCases = new Queue<CaseBase>(); 
        private CaseBase comparisonBase = new CaseBase();

        public void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            players = GetComponent<LevelManager>().players.Select(item => item.GetComponentInChildren<PlayerController>()).ToList();

            // Load in existing AI trees
            foreach (PlayerController player in players)
            {
                if (caseTrees.Any(item => item.Key == player.characterName)) continue;
                KeyValuePair<string, BinaryTree<CaseBase>> playerTree = LoadCases(player.characterName);
                print(playerTree.Value.Count);
                caseTrees.Add(playerTree);
            }

            // Set up xDeltas
            xDeltas.Add(new IntRange(0, 2, 0));
            xDeltas.Add(new IntRange(3, 5, 1));
            xDeltas.Add(new IntRange(6, 9, 2));
            xDeltas.Add(new IntRange(10, 14, 3));
            xDeltas.Add(new IntRange(15, 19, 4));
            xDeltas.Add(new IntRange(20, 24, 5));
            xDeltas.Add(new IntRange(25, 29, 6));
            xDeltas.Add(new IntRange(30, 34, 7));
            xDeltas.Add(new IntRange(35, 39, 8));
            xDeltas.Add(new IntRange(40, 44, 9));
            xDeltas.Add(new IntRange(45, 49, 10));
            xDeltas.Add(new IntRange(50, 54, 11));
            xDeltas.Add(new IntRange(55, 59, 12));
            xDeltas.Add(new IntRange(60, 64, 13));
            xDeltas.Add(new IntRange(65, 79, 14));
            xDeltas.Add(new IntRange(80, 700, 15));

            // Set up yDeltas
            yDeltas.Add(new IntRange(0, 1, 0));
            yDeltas.Add(new IntRange(1, 2, 1));
            yDeltas.Add(new IntRange(3, 10, 2));
            yDeltas.Add(new IntRange(11, 100, 3));

            // Set up shieldRanges
            shieldRanges.Add(new IntRange(0, 5, 0));
            shieldRanges.Add(new IntRange(6, 10, 1));
            shieldRanges.Add(new IntRange(11, 20, 2));
            shieldRanges.Add(new IntRange(21, 35, 3));
            shieldRanges.Add(new IntRange(36, 55, 4));
            shieldRanges.Add(new IntRange(56, 80, 5));
            shieldRanges.Add(new IntRange(81, 100, 6));

            // Set up stateCategories
            stateCategories.Add(new IntRange(0, 15, 0)); // On ground, not attacking or blocking
            stateCategories.Add(new IntRange(16, 17, 1)); // Blocking
            stateCategories.Add(new IntRange(18, 19, 2)); // Dodging
            stateCategories.Add(new IntRange(20, 27, 3)); // On ground, attacking
            stateCategories.Add(new IntRange(28, 31, 4)); // Charging smashes
            stateCategories.Add(new IntRange(32, 34, 5)); // Smashes
            stateCategories.Add(new IntRange(35, 37, 6)); // Throws
            stateCategories.Add(new IntRange(38, 47, 7)); // In air, not attacking
            stateCategories.Add(new IntRange(48, 49, 8)); // Helpless
            stateCategories.Add(new IntRange(50, 57, 9)); // In air, attacking
            stateCategories.Add(new IntRange(58, 58, 10)); // Neutral special
            stateCategories.Add(new IntRange(59, 59, 11)); // Down special
            stateCategories.Add(new IntRange(60, 61, 12)); // Side special
            stateCategories.Add(new IntRange(62, 62, 13)); // Invulnerable
            stateCategories.Add(new IntRange(63, 65, 13)); // No control
        }

        public void OnDisable()
        {
            SaveCases();
        }

        public void Update()
        {
            List<PlayerController> observedPlayers;
            if (gameManager.GameConfig.TournamentMode)
            {
                return;
            }

            if (players.Any(item => item.GetState() == null))
            {
                return;
            }

            if (!players.Any(item => item.GetState().GetStateID() < 64 && item.RaycastGround() && !item.Computer))
            {
                return;
            }

            observedPlayers = players.Where(item => item.GetState().GetStateID() < 64 && item.RaycastGround() && !item.Computer).ToList();
            foreach (PlayerController player in observedPlayers) // Ignore stun and launch states
            {
                int situationIndex = GenerateSituationIndex(player);

//                BLF.PrintBinary(situationIndex);

                CaseBase currentCase;
                comparisonBase.SituationIndex = situationIndex;
                BinaryTreeNode<CaseBase> existingCaseBase = caseTrees.First(item => item.Key == player.characterName).Value.Search(comparisonBase);
                if (existingCaseBase != null)
                {
                    currentCase = existingCaseBase.Data;
                }
                else
                {
                    currentCase = new CaseBase();
                    currentCase.SituationIndex = situationIndex;
                    caseTrees.First(item => item.Key == player.characterName).Value.Insert(currentCase);
                    print("Adding new case");
                }

                // Gather controller input for frame
                List<byte> buttonPressState = player.input.ControllerButtonPressState();
                List<byte> buttonHoldState = player.input.ControllerButtonHoldState();
                sbyte[] analogState = player.input.ControllerAnalogState();

                activeCases.Enqueue(currentCase);

                bool dequeue = false;
                foreach (CaseBase activeCase in activeCases)
                {
                    if (activeCase.Frame < CaseBase.RecordFrames)
                    {
//                        print("Handling case " + activeCase.Frame);
                        if (buttonPressState.Any())
                        {
                            activeCase.PushButtonPressResponse(buttonPressState);
                        }
                        if (buttonHoldState.Any())
                        {
                            activeCase.PushButtonHoldResponse(buttonHoldState);
                        }
                        if (analogState.Any())
                        {
                            activeCase.PushAnalogResponse(analogState);
                        }
//                        BLF.PrintBinary(activeCase.ActiveResponseState);
                        activeCase.PushActiveResponseState();

                        activeCase.TotalRatio += 1;
                        activeCase.Frame += 1;
                    }
                    else
                    {
                        dequeue = true;
                    }
                }
                if (dequeue)
                {
                    activeCases.Dequeue().Frame = 0;
                }
            }
        }

        public int GenerateSituationIndex(PlayerController player)
        {
            int situationIndex = 0; // Encrypted state of the game

//                print("StateID: " + player.GetState().GetStateID());
            if (player.GetState() == null)
            {
                return 0;
            }

            // [Bits 31-26] Append player state last 6 bits to situationIndex (6 bits)
            for (int i = 0; i < 6; i++)
            {
                if (BLF.IsBitSet(player.GetState().GetStateID(), 6-i))
                {
//                        print("\tSetting bit " + (31 - i));
                    situationIndex = BLF.SetBit(situationIndex, 31 - i);
                }
            }

            // Find closest player
            PlayerController nearestPlayer = player.opponents.OrderByDescending(item => Mathf.Abs(player.transform.position.x - item.transform.position.x)).Last();
//                    PlayerController nearestPlayerY = player.opponents.OrderByDescending(item => Mathf.Abs(player.transform.position.y - item.transform.position.y)).Last();
//                    PlayerController nearestPlayer = nearestPlayerX;
//                    if (Mathf.Abs(player.transform.position.x - nearestPlayerX.transform.position.x) < 4*Mathf.Abs(player.transform.position.y - nearestPlayerY.transform.position.y))
//                    {
//                        nearestPlayer = nearestPlayerX;
//                    }
//                    else
//                    {
//                        nearestPlayer = nearestPlayerY;
//                    }
                
            // [Bit 25] Is closest player to the left or to the right (1 bit)
            bool nearestToRight = (player.transform.position.x - nearestPlayer.transform.position.x >= 0);
            if (nearestToRight)
            {
                situationIndex = BLF.SetBit(situationIndex, 25);
            }

            // [Bits 24-21] What distance range is the enemy from the player (4 bits)
            float distanceToNearest = (Mathf.Abs(player.transform.position.x - nearestPlayer.transform.position.x));
            int rangeValue = xDeltas.Query((int)distanceToNearest).First().Value;
            for (int i = 0; i < 4; i++)
            {
                if (BLF.IsBitSet(rangeValue, i))
                {
//                        print("\tSetting bit " + (24 - i));
                    situationIndex = BLF.SetBit(situationIndex, 24 - i);
                }
            }

            // [Bit 20] Is closest player above or below the player (1 bit)
            bool nearestAbove = (player.transform.position.y - nearestPlayer.transform.position.y >= 0);
            if (nearestAbove)
            {
                situationIndex = BLF.SetBit(situationIndex, 20);
            }

            // [Bits 19-18] What distance range is the enemy above or below the player (2 bits)
            distanceToNearest = (Mathf.Abs(player.transform.position.y - nearestPlayer.transform.position.y));
            rangeValue = yDeltas.Query((int)distanceToNearest).First().Value;
            for (int i = 0; i < 2; i++)
            {
                if (BLF.IsBitSet(rangeValue, i))
                {
//                        print("\tSetting bit " + (24 - i));
                    situationIndex = BLF.SetBit(situationIndex, 19 - i);
                }
            }

            // [Bits 17-14] What general state is the nearest player in (4 bits)
            for (int i = 0; i < 4; i++)
            {
//                    BLF.PrintBinary(nearestPlayer.GetState().GetStateID());
//                    BLF.PrintBinary(stateCategories.Query(nearestPlayer.GetState().GetStateID()).First().Value);
//                    print("");
                if (BLF.IsBitSet(stateCategories.Query(nearestPlayer.GetState().GetStateID()).First().Value, 4-i))
                {
//                        print("\tSetting bit " + (31 - i));
                    situationIndex = BLF.SetBit(situationIndex, 17 - i);
                }
            }

            // [Bit 13] Is there a projectile within range to dodge (1 bit)

            // [Bit 12-10] What range is the nearest player's shield strength in (3 bits)
            rangeValue = shieldRanges.Query(nearestPlayer.GetShield()).First().Value;
            for (int i = 0; i < 3; i++)
            {
                if (BLF.IsBitSet(rangeValue, i))
                {
                    situationIndex = BLF.SetBit(situationIndex, 12 - i);
                }
            }

            // [Bit 9] Is the player facing right
            if (player.facingRight)
            {
                situationIndex = BLF.SetBit(situationIndex, 9);
            }

            // [Bit 8] Is the player touching the left edge of a platform
            if (player.onEdgeLeft)
            {
                situationIndex = BLF.SetBit(situationIndex, 8);
            }

            // [Bit 7] Is the player touching the right edge of a platform
            if (player.onEdgeRight)
            {
                situationIndex = BLF.SetBit(situationIndex, 7);
            }

            // [Bits 6-0] Additional information as needed

            return situationIndex;
        }

        public CaseBase LookupSituationIndex(int index, string characterName)
        {
            comparisonBase.SituationIndex = index;
            BinaryTreeNode<CaseBase> result = caseTrees.First(item => item.Key == characterName).Value.Search(comparisonBase);
            if (result != null)
            {
                return result.Data;
            }
            return null;
        }

        private KeyValuePair<string, BinaryTree<CaseBase>> LoadCases(string characterName)
        {
            return gameManager.LoadGhostAIData(characterName);
        }

        private void SaveCases()
        {
            foreach (KeyValuePair<string, BinaryTree<CaseBase>> caseTree in caseTrees)
            {
                gameManager.SaveGhostAIData(caseTree.Value, caseTree.Key);
            }
        }
    }
}