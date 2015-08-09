using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
//using UnityEditor;

namespace Assets.Scripts.Player
{
    public class PushOpponents : MonoBehaviour
    {
        [SerializeField] private bool right;
        private PlayerController playerController;
        private Collider2D collider;
        private Rigidbody2D rigidBody;
        private List<PlayerController> touchingPlayers;
        private List<PlayerController> removedPlayers;
        private float pushSpeed = 3.29f;

        private void Awake()
        {
            playerController = transform.parent.GetComponentInChildren<PlayerController>();
            collider = GetComponent<Collider2D>();
            rigidBody = transform.parent.GetComponentInChildren<Rigidbody2D>();
            touchingPlayers = new List<PlayerController>();
            removedPlayers = new List<PlayerController>();
        }

        private void FixedUpdate()
        {
            if (playerController.Grabbed)
            {
                return;
            }
            foreach (PlayerController player in touchingPlayers)
            {
                if (!player.GetComponentsInChildren<Collider2D>().Any(item => item.IsTouching(collider)))
                {
                    removedPlayers.Add(player);
                    continue;
                }
                if ((right && playerController.facingRight) || (!right && !playerController.facingRight))
                {
                    if (Mathf.Abs(player.transform.position.x - playerController.transform.position.x) < .2f && !player.Invincible && !player.onEdgeRight)
                    {
                        if (player.playerNumber > playerController.playerNumber)
                        {
                            player.IncrementVelocityX(-1);
//                            playerController.IncrementVelocityX(1);
                        }
                    }
                    else if (player.GetSpeedX() < pushSpeed && !player.Invincible && !player.onEdgeRight)
                    {
                        player.IncrementVelocityX(pushSpeed);
                    }
                }
                else
                {
                    if (Mathf.Abs(player.transform.position.x - playerController.transform.position.x) < .2f && !player.Invincible && !player.onEdgeLeft)
                    {
                        if (player.playerNumber > playerController.playerNumber)
                        {
                            player.IncrementVelocityX(1);
//                            playerController.IncrementVelocityX(-1);
                        }
                    }
                    else if (player.GetSpeedX() < pushSpeed && !player.Invincible && !player.onEdgeLeft) 
                    {
                        player.IncrementVelocityX(-pushSpeed);
                    }
                }
            }
            foreach (PlayerController player in removedPlayers)
            {
                touchingPlayers.Remove(player);
                print("Hard removing a player");
            }
            removedPlayers.Clear();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerController otherController = other.GetComponentInParent<PlayerController>();
                if (!touchingPlayers.Contains(otherController) && otherController != playerController)
                {
                    touchingPlayers.Add(otherController);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerController otherController = other.GetComponentInParent<PlayerController>();
                if (touchingPlayers.Contains(otherController))
                {
                    touchingPlayers.Remove(otherController);
                }
            }
        }
    }
}