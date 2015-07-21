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
//            if (other.tag == "Player")
//            {
//                playerController controller = other.GetComponent<playerController>();
//                Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
//                if (rigidbody.velocity.y < 0)
//                {
//                    controller.SetVelocityX(0f);
//                }
//            }
        }
    }
}