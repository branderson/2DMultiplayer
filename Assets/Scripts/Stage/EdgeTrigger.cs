using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class EdgeTrigger : MonoBehaviour
    {
        [SerializeField] private bool right;

        // When player changes between states, edge triggers get wiped
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (right)
                {
                    player.onEdgeRight = true;
                }
                else
                {
                    player.onEdgeLeft = true;
                }
                player.animator.SetBool("TouchingEdge", true);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (right)
                {
                    player.onEdgeRight = false;
                }
                else
                {
                    player.onEdgeLeft = false;
                }
                player.animator.SetBool("TouchingEdge", false);
            }
        }
    }
}