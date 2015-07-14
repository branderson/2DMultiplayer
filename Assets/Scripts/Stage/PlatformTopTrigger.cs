using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;

namespace Assets.Scripts.Stage
{
    public class PlatformTopTrigger : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController controller = other.GetComponent<PlayerController>();
                Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
                if (rigidbody.velocity.y < 0)
                {
                    if (controller.passThroughFloor && transform.parent.tag != "Impermeable")
                    {
                        controller.SetGroundCollisions(false);
                    }
                    else
                    {
                        controller.SetGroundCollisions(true);
                        print("Top collider is trying to stop him");
                    }
                }
                else
                {
                }
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController controller = other.GetComponent<PlayerController>();
                Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();

                if (controller.passThroughFloor && transform.parent.tag != "Impermeable")
                {
                    controller.SetGroundCollisions(false);
                }
                else
                {
                    controller.SetGroundCollisions(true);
                }
            }
        }
    }
}