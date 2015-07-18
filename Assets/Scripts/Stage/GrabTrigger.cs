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
            if (other.tag == "PlayerGrab")
            {
                PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
                player.animator.SetTrigger("EdgeGrab");
                print("Old: " + player.GetComponent<Rigidbody2D>().transform.position);
                print("Edge position: " + transform.position + ", localPosition: " + other.transform.localPosition);
                print("Goal position: " +
                      (transform.position - other.transform.localPosition));
                if (player.facingRight)
                {
                    player.transform.Translate(transform.position - player.transform.position -
                                               other.transform.localPosition);
                }
                else
                {
                    Vector3 reverseX = new Vector3(-other.transform.localPosition.x, other.transform.localPosition.y, other.transform.localPosition.z);
                    player.transform.Translate(transform.position - player.transform.position - reverseX);
                }
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                print("New: " + player.GetComponent<Rigidbody2D>().transform.position);
            }
        }
    }
}