using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class AILearner : MonoBehaviour
    {
        private List<PlayerController> players;
        // Might want to decrease the resolution of some of these ranges to increase chance of finding a matching situation
        private static readonly RangeTree<int, IntRange> xDeltas = new RangeTree<int, IntRange>(new RangeItemComparer());
        private static readonly RangeTree<int, IntRange> yDeltas = new RangeTree<int, IntRange>(new RangeItemComparer()); 
        private static readonly RangeTree<int, IntRange> shieldRanges = new RangeTree<int, IntRange>(new RangeItemComparer()); 

        public void Awake()
        {
            players = GetComponent<LevelManager>().players.Select(item => item.GetComponentInChildren<PlayerController>()).ToList();
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
        }

        public void Update()
        {
            List<PlayerController> observedPlayers;

            if (!players.Any(item => item.GetState().GetStateID() < 64 && item.RaycastGround() && !item.Computer))
            {
                return;
            }

            observedPlayers = players.Where(item => item.GetState().GetStateID() < 64 && item.RaycastGround() && !item.Computer).ToList();
            foreach (PlayerController player in observedPlayers) // Ignore stun and launch states
            {
                int situationIndex = 0; // Encrypted state of the game

                print("StateID: " + player.GetState().GetStateID());

                // Append player state last 6 bits to 
                for (int i = 0; i < 6; i++)
                {
                    if (BLF.IsBitSet(player.GetState().GetStateID(), 6-i))
                    {
                        print("\tSetting bit " + (31 - i));
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
                    
                // Is closest player to the left or to the right
                bool nearestToRight = (player.transform.position.x - nearestPlayer.transform.position.x >= 0);
                if (nearestToRight)
                {
                    situationIndex = BLF.SetBit(situationIndex, 25);
                }

                // What distance range is the enemy from the player
                float distanceToNearest = (Mathf.Abs(player.transform.position.x - nearestPlayer.transform.position.x));
                int rangeValue = xDeltas.Query((int)distanceToNearest).First().Value;
                BLF.PrintBinary(rangeValue);
                for (int i = 3; i >= 0; i--)
                {
                    if (BLF.IsBitSet(rangeValue, i))
                    {
                        print("\tSetting bit " + (24 - i));
                        situationIndex = BLF.SetBit(situationIndex, 24 - i);
                    }
                }

                BLF.PrintBinary(situationIndex);
            }
        }
    }
}