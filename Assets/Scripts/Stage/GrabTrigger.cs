using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class GrabTrigger : MonoBehaviour
    {
        private bool occupied = false;
        private PlayerController occupyingPlayer;
        private Rigidbody2D occupyingRigidbody;

        // When player changes between states, edge triggers get wiped
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "PlayerGrab" && !occupied)
            {
                PlayerController encounteredPlayer = other.transform.parent.GetComponentInChildren<PlayerController>();

                // TODO: Account for cases where player begins falling without leaving trigger (make trigger smaller maybe?)
                if (!(encounteredPlayer.GetVelocityY() < 0))
                {
                    return;
                }
                occupyingPlayer = encounteredPlayer;
                occupied = true;

                occupyingPlayer.animator.SetTrigger("EdgeGrab");
                occupyingRigidbody = occupyingPlayer.GetComponent<Rigidbody2D>();
                occupyingPlayer.IncrementVelocity(-occupyingPlayer.GetVelocity());
                if (occupyingPlayer.facingRight)
                {
                    occupyingPlayer.transform.Translate(transform.position - occupyingPlayer.transform.position -
                                                        other.transform.localPosition);
                    occupyingRigidbody.MovePosition(transform.position - other.transform.localPosition);
//                        print("Moving: " + (transform.position.x - other.transform.localPosition.x) + " " +
//                              (transform.position.y - other.transform.localPosition.y));
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
            if (occupied && other.tag == "PlayerGrab")
            {
                if (occupyingPlayer.GetState().GetName() == "EdgeGrabState")
                {
//                    occupyingPlayer.IncrementVelocity(-occupyingPlayer.GetVelocity());
                    if (occupyingPlayer.facingRight)
                    {
                        //                    occupyingPlayer.transform.Translate(transform.position - occupyingPlayer.transform.position -
                        //                                                        other.transform.localPosition);
                        occupyingPlayer.transform.Translate(transform.position - occupyingPlayer.transform.position -
                                                            other.transform.localPosition);
                        occupyingRigidbody.MovePosition(transform.position - other.transform.localPosition);
//                        print("Moving: " + (transform.position.x - other.transform.localPosition.x) + " " +
//                              (transform.position.y - other.transform.localPosition.y));
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
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "PlayerGrab")
            {
                if (other.transform.parent.GetComponentInChildren<PlayerController>() == occupyingPlayer)
                {
                    occupied = false;
                    occupyingPlayer = null;
                    occupyingRigidbody = null;
                }
            }
        }
    }
}