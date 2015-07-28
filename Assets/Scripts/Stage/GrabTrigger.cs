using Assets.Scripts.Player;
using Assets.Scripts.Player.States;
using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class GrabTrigger : MonoBehaviour
    {
        private bool occupied = false;
        private PlayerController occupyingPlayer;

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
//                print("Old: " + player.GetComponent<Rigidbody2D>().transform.position);
//                print("Edge position: " + transform.position + ", localPosition: " + other.transform.localPosition);
//                print("Goal position: " +
//                      (transform.position - other.transform.localPosition));
                Rigidbody2D occupyingRigidbody = occupyingPlayer.GetComponent<Rigidbody2D>();
                if (occupyingPlayer.facingRight)
                {
//                    occupyingPlayer.transform.Translate(transform.position - occupyingPlayer.transform.position -
//                                                        other.transform.localPosition);
                    occupyingRigidbody.MovePosition(transform.position - other.transform.localPosition);
                }
                else
                {
                    Vector3 reverseX = new Vector3(-other.transform.localPosition.x, other.transform.localPosition.y,
                        other.transform.localPosition.z);
                    occupyingRigidbody.MovePosition(transform.position - reverseX);
                }
//                occupyingPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
//                print("New: " + player.GetComponent<Rigidbody2D>().transform.position);
            }
        }

//        private void OnTriggerStay2D(Collider2D other)
//        {
//            if (other.tag == "PlayerGrab" && other.transform.parent.GetComponentInChildren<PlayerController>() == occupyingPlayer)
//            {
//                if (other.transform.position != transform.position)
//                {
//                    print("Correcting grab");
//                    OnTriggerExit2D(other);
//                    OnTriggerEnter2D(other);
//                }
//            }
//        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "PlayerGrab")
            {
                if (other.transform.parent.GetComponentInChildren<PlayerController>() == occupyingPlayer)
                {
                    occupied = false;
                    occupyingPlayer = null;
                }
            }
        }
    }
}