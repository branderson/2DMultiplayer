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
        private List<KeyValuePair<PlayerController, Queue<CaseBase>>> recordingCases = new List<KeyValuePair<PlayerController, Queue<CaseBase>>>();
        private List<KeyValuePair<PlayerController, int>> playerStates;
        private List<KeyValuePair<PlayerController, KeyValuePair<int, int>>> sequenceEffectiveness;
        private CaseBase comparisonBase = new CaseBase();

        public void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            players = GetComponent<LevelManager>().players.Select(item => item.GetComponentInChildren<PlayerController>()).ToList();
            playerStates = new List<KeyValuePair<PlayerController, int>>();
            sequenceEffectiveness = new List<KeyValuePair<PlayerController, KeyValuePair<int, int>>>();

            // Load in existing AI trees
            foreach (PlayerController player in players)
            {
                if (!player.Computer)
                {
                    recordingCases.Add(new KeyValuePair<PlayerController, Queue<CaseBase>>(player, new Queue<CaseBase>()));
                    playerStates.Add(new KeyValuePair<PlayerController, int>(player, GenerateSituationIndex(player)));
                    sequenceEffectiveness.Add(new KeyValuePair<PlayerController, KeyValuePair<int, int>>(player, new KeyValuePair<int, int>(0, 0)));
                }

                if (caseTrees.Any(item => item.Key == player.characterName) || player.Computer) continue;
                KeyValuePair<string, BinaryTree<CaseBase>> playerTree = LoadCases(player.characterName);
//                print("Cases loaded in: " + playerTree.Value.Count);
//                foreach (CaseBase caseBase in playerTree.Value.InOrder().Select(item => item.Data))
//                {
//                    BLF.PrintBinary(caseBase.SituationIndex);
//                }
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
            if (gameManager.GameConfig.TournamentMode)
            {
                return;
            }

            if (players.Any(item => item.GetState() == null))
            {
                return;
            }

//            if (!players.Any(item => item.GetState().GetStateID() < 64 && !item.Computer))
//            {
//                return;
//            }

            List<PlayerController> observedPlayers = players.Where(item => !item.Computer).ToList();
            foreach (PlayerController player in observedPlayers) // Ignore stun and launch states
            {
                // What situation is the player in
                int situationIndex = 0;
                bool inControl = true;
                if (player.GetState().GetStateID() >= 64)
                {
                    inControl = false;
                }
                else
                {
                    situationIndex = GenerateSituationIndex(player);
                }

                // Only check for new cases when situation has changed since last frame
//                if (situationIndex != playerStates.First(item => item.Key == player).Value && inControl)
                if (inControl)
                {
                    // Has this case been encountered before
                    CaseBase currentCase;
                    comparisonBase.SituationIndex = situationIndex;
                    BinaryTreeNode<CaseBase> existingCaseBase =
                        caseTrees.First(item => item.Key == player.characterName).Value.Search(comparisonBase);
                    // If so, use the case
                    if (existingCaseBase != null)
                    {
                        currentCase = existingCaseBase.Data;
                        currentCase.TotalRatio += 1;
                    }
                    // Otherwise, make a new one
                    else
                    {
                        currentCase = new CaseBase()
                        {
                            SituationIndex = situationIndex,
                            TotalRatio = 1,
                        };
                        caseTrees.First(item => item.Key == player.characterName).Value.Insert(currentCase);
                    }

                    // Add current case to queue if not already there
                    if (!recordingCases.First(item => item.Key == player).Value.Contains(currentCase))
                    {
                        recordingCases.First(item => item.Key == player).Value.Enqueue(currentCase);
                    }
                }

                // Update situation index of player
                KeyValuePair<PlayerController, int> playerState = playerStates.First(item => item.Key == player);
                playerState = new KeyValuePair<PlayerController, int>(player, GenerateSituationIndex(player));
                playerStates.Remove(playerStates.First(item => item.Key == player));
                playerStates.Add(playerState);

                // Gather controller input for current frame
                List<byte> buttonState = player.input.ControllerButtonHoldState();
                sbyte[] analogState = player.input.ControllerAnalogState();
//                if (buttonState.Contains(0) && (analogState[0] == 2 || analogState[0] == -2))
//                {
//                    print("Should be recording a side smash");
//                }

                byte dequeue = 0;

                foreach (CaseBase activeCase in recordingCases.First(item => item.Key == player).Value)
                {
                    if (activeCase.Frame < CaseBase.RecordFrames)
                    {
                        activeCase.PushButtonStateResponse(buttonState);
                        activeCase.PushAnalogResponse(analogState);
                        activeCase.PushActiveResponseState();
                        int punish = 0;
                        if (!inControl)
                        {
                            punish += 1;
                        }
                        if (player.Respawned)
                        {
                            punish += 100;
                        }
//                        print("Players");
//                        foreach (PlayerController playerCon in players)
//                        {
//                            BLF.PrintBinary(playerCon.GetState().GetStateID());
//                        }
                        int reward = ShouldReward(player, situationIndex);
                        KeyValuePair<PlayerController, KeyValuePair<int, int>> newEffectiveness = new KeyValuePair<PlayerController, KeyValuePair<int, int>>(player, new KeyValuePair<int, int>(sequenceEffectiveness.First(item => item.Key == player).Value.Key + reward, sequenceEffectiveness.First(item => item.Key == player).Value.Value + punish));
                        sequenceEffectiveness.Remove(sequenceEffectiveness.First(item => item.Key == player));
                        sequenceEffectiveness.Add(newEffectiveness);
                        activeCase.Frame += 1;
                    }
                    else
                    {
                        dequeue += 1;
                    }
                }
                while (dequeue > 0)
                {
                    recordingCases.First(item => item.Key == player).Value.Dequeue().PushActiveSet(sequenceEffectiveness.First(item => item.Key == player).Value.Key, sequenceEffectiveness.First(item => item.Key == player).Value.Value);
                    sequenceEffectiveness.Remove(sequenceEffectiveness.First(item => item.Key == player));
                    sequenceEffectiveness.Add(new KeyValuePair<PlayerController, KeyValuePair<int, int>>(player, new KeyValuePair<int, int>(0, 0)));
                    dequeue--;
                }
            }
            foreach (PlayerController player in players)
            {
                player.Respawned = false;
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
//            print("Player state ID " + player.GetState());
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
//            print("Closest player to right: " + nearestToRight);
            if (nearestToRight)
            {
                situationIndex = BLF.SetBit(situationIndex, 25);
            }

            // [Bits 24-21] What distance range is the enemy from the player (4 bits)
            float distanceToNearest = (Mathf.Abs(player.transform.position.x - nearestPlayer.transform.position.x));
            int rangeValue = xDeltas.Query((int)distanceToNearest).First().Value;
//            print("Horizontal distance range: " + rangeValue);
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
//            print("Nearest above: " + nearestAbove);
            if (nearestAbove)
            {
                situationIndex = BLF.SetBit(situationIndex, 20);
            }

            // [Bits 19-18] What distance range is the enemy above or below the player (2 bits)
            distanceToNearest = (Mathf.Abs(player.transform.position.y - nearestPlayer.transform.position.y));
            rangeValue = yDeltas.Query((int)distanceToNearest).First().Value;
//            print("Vertical diestance range: " + rangeValue);
            for (int i = 0; i < 2; i++)
            {
                if (BLF.IsBitSet(rangeValue, i))
                {
//                        print("\tSetting bit " + (24 - i));
                    situationIndex = BLF.SetBit(situationIndex, 19 - i);
                }
            }


//            if (nearestPlayer.GetState().GetStateID() >= 64)
//            {
//                print("Player state ID " + player.GetState());
//                print("Nearest state ID " + nearestPlayer.GetState().GetStateID());
//                print("General state: " + stateCategories.Query(nearestPlayer.GetState().GetStateID()).First().Value);
//                BLF.PrintBinary(situationIndex);
//                BLF.PrintBinary(stateCategories.Query(nearestPlayer.GetState().GetStateID()).First().Value);
//            }
            // [Bits 17-14] What general state is the nearest player in (4 bits)
            for (int i = 0; i < 4; i++)
            {
//                    BLF.PrintBinary(nearestPlayer.GetState().GetStateID());
//                    BLF.PrintBinary(stateCategories.Query(nearestPlayer.GetState().GetStateID()).First().Value);
//                    print("");
                if (BLF.IsBitSet(stateCategories.Query(nearestPlayer.GetState().GetStateID()).First().Value, 3-i))
                {
//                        print("\tSetting bit " + (31 - i));
//                    if (nearestPlayer.GetState().GetStateID() >= 64)
//                    {
//                        print("Setting bit " + (17-i) + " to " + BLF.IsBitSet(stateCategories.Query(nearestPlayer.GetState().GetStateID()).First().Value, 3-i));
//                    }
                    situationIndex = BLF.SetBit(situationIndex, 17 - i);
                }
            }
//            if (nearestPlayer.GetState().GetStateID() >= 64)
//            {
//                BLF.PrintBinary(situationIndex);                      
//            }

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

        private int ShouldReward(PlayerController player, int situationIndex)
        {
            // Are bits 17-14 set to binary 13 (1101)
            int reward = 0;

            if (BLF.IsBitSet(situationIndex, 17) && BLF.IsBitSet(situationIndex, 16) &&
                !BLF.IsBitSet(situationIndex, 15) && BLF.IsBitSet(situationIndex, 14))
            {
                reward += 1;
            }
            PlayerController nearestPlayer = player.opponents.OrderByDescending(item => Mathf.Abs(player.transform.position.x - item.transform.position.x)).Last();
            if (nearestPlayer.Respawned)
            {
                reward += 10;
            }
            return reward;
        }

        private KeyValuePair<string, BinaryTree<CaseBase>> LoadCases(string characterName)
        {
            return gameManager.LoadGhostAIData(characterName);
        }

        private void SaveCases()
        {
//            foreach (KeyValuePair<string, BinaryTree<CaseBase>> caseTree in caseTrees)
//            {
//                List<int> deletions = caseTree.Value.InOrder().Select(item => item.Data).Where(caseBase => caseBase.Empty()).Select(current => current.SituationIndex).ToList();
//                foreach (int deletion in deletions)
//                {
//                    print("Trying to delte " + deletion);
//                    comparisonBase.SituationIndex = deletion;
//                    if (caseTree.Value.Search(comparisonBase) != null)
//                    {
//                        caseTree.Value.Delete(comparisonBase, false);
//                    }
//                    print("Deleting");
//                }
//            }
            foreach (KeyValuePair<string, BinaryTree<CaseBase>> caseTree in caseTrees)
            {
                gameManager.SaveGhostAIData(caseTree.Value, caseTree.Key);
            }
        }
    }
}