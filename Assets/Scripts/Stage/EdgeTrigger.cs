using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class EdgeTrigger : MonoBehaviour
    {
        [SerializeField] private bool right;
        private Collider2D collider;
        private List<PlayerController> occupyingPlayers = new List<PlayerController>();

        public void Awake()
        {
            collider = GetComponent<Collider2D>();
        }

        public void Update()
        {
            occupyingPlayers =
                occupyingPlayers.Where(
                    item => item.GetComponentsInChildren<Collider2D>().Any(other => other.IsTouching(collider)))
                    .ToList();
        }

        // When player changes between states, edge triggers get wiped
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController player = other.GetComponentInParent<PlayerController>();
                occupyingPlayers.Add(player);
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
                PlayerController player = other.GetComponentInParent<PlayerController>();
                if (occupyingPlayers.Contains(player))
                {
                    occupyingPlayers.Remove(player);
                }
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