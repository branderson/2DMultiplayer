using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;

namespace Assets.Scripts.Stage
{
    public class PlatformBottomTrigger : MonoBehaviour
    {
        private Collider2D platformCollider;

        private void Awake()
        {
            platformCollider = transform.parent.Find("Rigidbody").GetComponent<Collider2D>();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            // TODO: Look for a collision with the head, on a separate collider (non rigidbody). If detected, turn on collision for feet
            if (other.tag == "PlayerHead")
            {
                // TODO: Could put side triggers on separate script and disable on enter
                PlayerController controller = other.transform.parent.GetComponentInChildren<PlayerController>();
                Rigidbody2D rigidbody = other.transform.parent.GetComponentInChildren<Rigidbody2D>();
                // If enterring from the bottom, pass through, otherwise turn on collisions
                if (rigidbody.velocity.y > 0) // && !transform.parent.CompareTag("Impermeable"))
                {
                    controller.IgnoreCollision(platformCollider);
                }
//                else if (transform.parent.CompareTag("Impermeable"))
//                {
//                    controller.IgnoreCollision(platformCollider, false);
//                    controller.passThroughFloor = false;
//                }
            }
            else if (other.tag == "PlayerTrigger")
            {
                // BodyTrigger must always be smaller than rigidbody
                PlayerController controller = other.transform.parent.GetComponentInChildren<PlayerController>();
                controller.IgnoreCollision(platformCollider);
            }
        }
//
//        public void OnTriggerExit2D(Collider2D other)
//        {
//            if (other.CompareTag("Player"))
//            {
//                
//            }
//        }
    }
}