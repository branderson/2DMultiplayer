using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;

namespace Assets.Scripts.Stage
{
    public class PlatformTopTrigger : MonoBehaviour
    {
        private Collider2D platformCollider;

        private void Awake()
        {
            platformCollider = transform.parent.Find("Rigidbody").GetComponent<Collider2D>();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "PlayerFeet")
            {
                PlayerController controller = other.transform.parent.GetComponentInChildren<PlayerController>();
                Rigidbody2D rigidbody = other.transform.parent.GetComponentInChildren<Rigidbody2D>();
                if (rigidbody.velocity.y < 0)
                {
                    if (controller.passThroughFloor) // && transform.parent.tag != "Impermeable")
                    {
                        controller.IgnoreCollision(platformCollider);
                    }
//                    else if (transform.parent.tag == "Impermeable")
//                    {
//                        controller.passThroughFloor = false;
//                        controller.CheckForGround();
//                        // TODO: Put into idle state
//                    }
                    else
                    {
                        controller.IgnoreCollision(platformCollider, false);
                    }
                }
                else
                {
                }
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "PlayerFeet")
            {
                PlayerController controller = other.transform.parent.GetComponentInChildren<PlayerController>();
//                Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();

                if (controller.passThroughFloor) // && transform.parent.tag != "Impermeable")
                {
                    controller.IgnoreCollision(platformCollider);
                    controller.animator.SetTrigger("FallThroughFloor");
                }
//                else
//                {
//                }
            }
        }
    }
}