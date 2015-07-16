using UnityEngine;
using System.Collections;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;

namespace Assets.Scripts.Stage
{
    public class KillZoneTrigger : MonoBehaviour
    {
        private LevelManager levelManager;

        private void Awake()
        {
            levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                print("Sees player");
                levelManager.Respawn(other.GetComponent<PlayerController>());
            }
        }
    }
}