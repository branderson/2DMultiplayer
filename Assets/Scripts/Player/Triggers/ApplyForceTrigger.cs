using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Assets.Scripts.Player.Triggers
{
    public class ApplyForceTrigger : MonoBehaviour
    {
        [SerializeField] private bool overrideOthers;
        [SerializeField] public int damageApplied;
        [SerializeField] public int Knockback;
        [SerializeField] public float Scaling = 1;
        [SerializeField] public bool SetKnockback = false;
        [Range(-1, 1)] [SerializeField] public float DirectionVectorX;
        [Range(-1, 1)] [SerializeField] public float DirectionVectorY;
        [SerializeField] public Vector2 ForceApplied;
        [SerializeField] public bool Stun = true;
        [SerializeField] public int Stagger = 1;
        [SerializeField] private bool vibrateSelf = true;
        [SerializeField] private bool vibrateOpponent = true;
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
                PlayerController controller = other.GetComponentInParent<PlayerController>();
                if (controller != playerController)
                {
                    AttackData attack = new AttackData()
                    {
                        Damage = damageApplied + DamageSupplement,
                        Knockback = (int) (Knockback*ForceMultiplier),
                        Scaling = Scaling,
                        SetKnockback = SetKnockback,
                        Direction = new Vector2(DirectionVectorX, DirectionVectorY),
                        Stagger = Stagger,
                        Stun = Stun,
                        Vibrate = vibrateOpponent,
                    };
                    manager.AddForce(controller, attack, overrideOthers);
//                    manager.AddForce(controller, (int)(Knockback*ForceMultiplier), Scaling, new Vector2(DirectionVectorX, DirectionVectorY), damageApplied+DamageSupplement, Stun, Stagger, overrideOthers, vibrateOpponent);
//                    manager.AddForce(controller, ForceApplied*ForceMultiplier, damageApplied+DamageSupplement, Stun, Stagger, overrideOthers, vibrateOpponent);
                    if (vibrateSelf && !controller.Invincible)
                    {
                        playerController.SetVibrate(12, 0f, 1f);
                    }
                }
//                    // TODO: Maybe add a tag for hitable for team battles
            }
        }
    }
}