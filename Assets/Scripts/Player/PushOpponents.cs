using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;

namespace Assets.Scripts.Player
{
    public class PushOpponents : MonoBehaviour
    {
        private PlayerController playerController;
        private Rigidbody2D rigidBody;
        private List<PlayerController> touchingPlayers; 

        private void Awake()
        {
            playerController = transform.parent.GetComponentInChildren<PlayerController>();
            rigidBody = transform.parent.GetComponentInChildren<Rigidbody2D>();
            touchingPlayers = new List<PlayerController>();
        }

        private void FixedUpdate()
        {
            foreach (PlayerController player in touchingPlayers)
            {
                // TODO: Doesn't work because input controller canceling out
                player.IncrementVelocityX(rigidBody.velocity.x * .5f);
                print("Moving " + (rigidBody.velocity.x * .5f));
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerController otherController = other.GetComponent<PlayerController>();
                if (!touchingPlayers.Contains(otherController) && otherController != playerController)
                {
                    touchingPlayers.Add(otherController);
                    print("Adding");
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerController otherController = other.GetComponent<PlayerController>();
                touchingPlayers.Remove(otherController);
            }
        }
    }
}