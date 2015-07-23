using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Assets.Scripts.Player.Triggers
{
    public class ApplyForceTrigger : MonoBehaviour
    {
        [SerializeField] private bool overrideOthers;
        [SerializeField] public int damageApplied;
        [SerializeField] public Vector2 ForceApplied;
        [SerializeField] private bool vibrateSelf;
        [SerializeField] private bool vibrateOpponent;
        internal float ForceMultiplier = 1f;
        internal int DamageSupplement = 0;
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
                if (controller != playerController)
                {
                    manager.AddForce(controller, ForceApplied*ForceMultiplier, damageApplied+DamageSupplement, overrideOthers, vibrateOpponent);
                    if (vibrateSelf && !controller.Invincible)
                    {
                        playerController.SetVibrate(12, .8f, .5f);
                    }
                }
//                    // TODO: Maybe add a tag for hitable for team battles
            }
        }
    }
}