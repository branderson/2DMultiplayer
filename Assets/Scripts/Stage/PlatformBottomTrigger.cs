using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;

namespace Assets.Scripts.Stage
{
    public class PlatformBottomTrigger : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                print("Player entered");
                PlayerController controller = other.GetComponent<PlayerController>();
                Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
                // If enterring from the bottom, pass through, otherwise turn on collisions
                if (rigidbody.velocity.y > 0 && transform.parent.tag != "Impermeable")
                {
                    controller.SetGroundCollisions(false);
                    print("Should be passing up through");
                }
                else
                {
                    print("Not allowing player through " + rigidbody.velocity.y + " " + transform.parent.tag);
                    controller.SetGroundCollisions(true);
                    controller.passThroughFloor = false;
                }
            }
        }
    }
}