using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class GrabTrigger : MonoBehaviour
    {
        // When player changes between states, edge triggers get wiped
        private void OnTriggerEnter2D(Collider2D other)
        {
            print("OnTriggerEnter");
            print(other.tag);
            if (other.tag == "Player")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.animator.SetBool("EdgeGrab", true);
                print("Collision");
            }
        }
    }
}