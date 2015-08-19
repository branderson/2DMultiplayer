using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Assets.Scripts.Player.Triggers
{
    public class DamageAffectorTrigger : MonoBehaviour
    {
        [SerializeField] private bool overrideOthers;
        [SerializeField] public float damagePerFrame;
        [SerializeField] public int baseDamage;
        [SerializeField] public int Knockback;
        [SerializeField] public float Scaling;
        [SerializeField] public bool SetKnockback;
        [Range(0, 360)] [SerializeField] public int Direction;
        [SerializeField] public bool Stun = true;
        [SerializeField] public int Hitlag;
        [SerializeField] public int Stagger = 1;
        [SerializeField] private bool vibrateSelf;
        [SerializeField] private bool vibrateOpponent;
        internal float ForceMultiplier = 1f;
        internal int DamageSupplement = 0;
        private float forceReductionFactor = 50f;
        private Dictionary<PlayerController, int> hitFrames; 
        private PlayerController playerController;
        private ForceTriggerManager manager;

        private void Awake()
        {
            hitFrames = new Dictionary<PlayerController, int>();
            playerController = transform.parent.parent.GetComponentInChildren<PlayerController>();
            manager = GetComponentInParent<ForceTriggerManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController controller = other.GetComponentInParent<PlayerController>();
                if (!hitFrames.ContainsKey(controller)&& controller != playerController)
                {
                    hitFrames.Add(controller, 0);
                    controller.Stun(1, false);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController controller = other.GetComponentInParent<PlayerController>();
                if (hitFrames.ContainsKey(controller) && controller != playerController)
                {
                    hitFrames[controller] += 1;
                    // TODO: This doesn't work if you want to deal more than 1 damage per frame(although that would be a bit ridiculous)
                    if (Mathf.Approximately(damagePerFrame*hitFrames[controller], Mathf.RoundToInt(damagePerFrame*hitFrames[controller])))
                    {
                        controller.TakeDamage(1);
                    }
                    if (vibrateSelf && !controller.Invincible)
                    {
                        playerController.SetVibrate(6, .8f, .5f);
                    }
                    if (controller.resistance < Stagger)
                    {
                        controller.Stun(1, false);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerController controller = other.GetComponentInParent<PlayerController>();
                if (hitFrames.ContainsKey(controller) && controller != playerController)
                {
                    hitFrames.Remove(controller);
                }
//                    // TODO: Maybe add a tag for hitable for team battles
            }
        }

        private void OnDisable()
        {
            foreach (PlayerController controller in hitFrames.Keys)
            {
                if (controller.resistance < Stagger)
                {
                    int appliedKnockback =
                        (int) ((hitFrames[controller]*Knockback/forceReductionFactor + Knockback)*ForceMultiplier);
                    AttackData attackData = new AttackData()
                    {
                        Player = playerController, 
                        Knockback = appliedKnockback,
                        Scaling = Scaling,
                        SetKnockback = SetKnockback, 
//                        Direction = new Vector2(DirectionVectorX, DirectionVectorY),
//                        Direction = new Vector2(Mathf.Cos(Direction * Mathf.Deg2Rad), Mathf.Sin(Direction * Mathf.Deg2Rad)), 
                        Direction = Direction, 
                        Damage = baseDamage+DamageSupplement,
                        Stagger = Stagger, 
                        Stun = Stun,
                        Hitlag = Hitlag, 
                        Vibrate = vibrateOpponent,
                    };
                    manager.AddForce(controller, attackData, overrideOthers);
                }
                if (vibrateSelf && !controller.Invincible)
                {
                    playerController.SetVibrate(15, 1f, 1f);
                }
            }
            hitFrames.Clear();
        }
    }
}