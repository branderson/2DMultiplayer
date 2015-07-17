using UnityEngine;

namespace Assets.Scripts.Player.Triggers
{
    public class ApplyForceTrigger : MonoBehaviour
    {
        [SerializeField] public bool OverrideOthers;
        [SerializeField] private int damageApplied;
        [SerializeField] private Vector2 forceApplied;
        private PlayerController playerController;
        private ForceTriggerManager manager;

        private void Awake()
        {
            playerController = transform.parent.parent.GetComponentInChildren<PlayerController>();
            manager = GetComponentInParent<ForceTriggerManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController controller = other.GetComponent<PlayerController>();
                manager.AddForce(controller, forceApplied, damageApplied);

//                    // TODO: Maybe add a tag for hitable for team battles
            }
        }
    }
}