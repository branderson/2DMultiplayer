using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class StopPlayerInternalTrigger : MonoBehaviour
    {
        private Collider2D platformCollider;

        private void Awake()
        {
            platformCollider = transform.parent.Find("Rigidbody").GetComponent<Collider2D>();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController controller = other.GetComponent<PlayerController>();
                Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
                if (rigidbody.velocity.y < 0)
                {
                    controller.SetInternalVelocityX(0f);
                }
            }
        }
    }
}