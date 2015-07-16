using UnityEngine;

namespace Assets.Scripts.Player.Triggers
{
    public class ApplyForceTrigger : MonoBehaviour
    {
        [SerializeField] private Vector2 forceApplied;
        private PlayerController playerController;

        private void Awake()
        {
            playerController = transform.parent.GetComponentInChildren<PlayerController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController controller = other.GetComponent<PlayerController>();
                Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();

                // TODO: Maybe add a tag for hitable for team battles
                if (playerController.facingRight)
                {
                    controller.IncrementVelocity(forceApplied);
                }
                else
                {
                    controller.IncrementVelocity(-forceApplied);
                }
            }
        }
    }
}