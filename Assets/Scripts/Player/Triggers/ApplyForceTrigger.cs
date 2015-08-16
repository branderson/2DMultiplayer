using System;
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
        [Range(0, 360)] [SerializeField] public int Direction;
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
                        Player = playerController, 
                        Damage = damageApplied + DamageSupplement,
                        Knockback = (int) (Knockback*ForceMultiplier),
                        Scaling = Scaling,
                        SetKnockback = SetKnockback,
//                        Direction = new Vector2(DirectionVectorX, DirectionVectorY),
                        Direction = new Vector2(Mathf.Cos(Direction * Mathf.Deg2Rad), Mathf.Sin(Direction * Mathf.Deg2Rad)),
                        Stagger = Stagger,
                        Stun = Stun,
                        Vibrate = vibrateOpponent,
                    };
                    manager.AddForce(controller, attack, overrideOthers);
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