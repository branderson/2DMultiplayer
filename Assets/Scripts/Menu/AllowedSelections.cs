using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Menu
{
    public class AllowedSelections : MonoBehaviour
    {
        [SerializeField] private bool Player1 = true;
        [SerializeField] private bool Player2 = true;
        [SerializeField] private bool Player3 = true;
        [SerializeField] private bool Player4 = true;

        public bool Allow(int playerNumber)
        {
            switch (playerNumber)
            {
                case 1:
                    return Player1;
                case 2:
                    return Player2;
                case 3:
                    return Player3;
                case 4:
                    return Player4;
                default:
                    return false;
            }
        }
    }
}