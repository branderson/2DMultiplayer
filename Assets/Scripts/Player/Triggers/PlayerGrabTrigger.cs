using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class PlayerGrabTrigger : MonoBehaviour
    {
        private bool occupied = false;
        private PlayerController playerController;
        private PlayerController occupyingPlayer;
        private Rigidbody2D occupyingRigidbody;

        private void Awake()
        {
            playerController = transform.parent.GetComponentInChildren<PlayerController>();
        }

        private void OnDisable()
        {
            if (occupyingPlayer != null)
            {
                occupyingPlayer.Grabbed = false;
            }
            playerController.Holding = false;
            occupied = false;
            occupyingPlayer = null;
            occupyingRigidbody = null;
        }

        public void Turn()
        {
            if (occupyingPlayer != null)
            {
                occupyingPlayer.Flip();
            }
        }

        // When player changes between states, edge triggers get wiped
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player" && !occupied)
            {
                PlayerController encounteredPlayer = other.GetComponentInParent<PlayerController>();

                // TODO: Ungrabbable conditions go here 
                if (encounteredPlayer == playerController || encounteredPlayer.Grabbed || encounteredPlayer.Invincible)
                {
                    return;
                }
                occupyingPlayer = encounteredPlayer;
                occupied = true;

                playerController.Holding = true;
                occupyingPlayer.Grabbed = true;
                occupyingRigidbody = occupyingPlayer.GetComponent<Rigidbody2D>();
                occupyingPlayer.IncrementVelocity(-occupyingPlayer.GetVelocity());
                if (occupyingPlayer.facingRight)
                {
                    occupyingPlayer.transform.Translate(transform.position - occupyingPlayer.transform.position -
                                                        other.transform.localPosition);
                    occupyingRigidbody.MovePosition(transform.position - other.transform.localPosition);
                }
                else
                {
                    Vector3 reverseX = new Vector3(-other.transform.localPosition.x, other.transform.localPosition.y,
                        other.transform.localPosition.z);
                    occupyingPlayer.transform.Translate(transform.position - occupyingPlayer.transform.position -
                                                        reverseX);
                    occupyingRigidbody.MovePosition(transform.position - reverseX);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (occupied && other.tag == "Player")
            {
                if (occupyingPlayer.Grabbed && playerController.Holding) //(occupyingPlayer.GetState().GetName() == "EdgeGrabState")
                {
                    if (occupyingPlayer.facingRight)
                    {
                        occupyingPlayer.transform.Translate(transform.position - occupyingPlayer.transform.position -
                                                            other.transform.localPosition);
                        occupyingRigidbody.MovePosition(transform.position - other.transform.localPosition);
                    }
                    else
                    {
                        Vector3 reverseX = new Vector3(-other.transform.localPosition.x, other.transform.localPosition.y,
                            other.transform.localPosition.z);
                        occupyingPlayer.transform.Translate(transform.position - occupyingPlayer.transform.position -
                                                            reverseX);
                        occupyingRigidbody.MovePosition(transform.position - reverseX);
                    }

                }
                else if (!occupyingPlayer.Grabbed)
                {
                    playerController.Holding = false;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                if (other.GetComponent<PlayerController>() == occupyingPlayer)
                {
                    occupied = false;
                    occupyingPlayer = null;
                    occupyingRigidbody = null;
                }
            }
        }
    }
}