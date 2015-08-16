using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player.Triggers
{
    public struct AttackData
    {
        public PlayerController Player;
        public int Knockback;
        public float Scaling;
        public bool SetKnockback;
        public Vector2 Direction;
        public int Damage;
        public int Stagger;
        public bool Vibrate;
        public bool Stun;
    }

    public class PlayerAttackData
    {
        public PlayerController PlayerController;
        public List<AttackData> Attacks = new List<AttackData>(); 
        public bool Overridden = false;
        public bool Applied = false;
    }

    public class ForceTriggerManager : MonoBehaviour
    {
        private PlayerController playerController;
        private List<PlayerAttackData> pendingForces;

        private void Awake()
        {
            playerController = transform.parent.GetComponentInChildren<PlayerController>();
            pendingForces = new List<PlayerAttackData>();
        }

        private void OnEnable()
        {
//            print("OnEnable");
            pendingForces.Clear();
        }

        private void Update()
        {
//            int forceLength = pendingForces.Count();
            foreach (PlayerAttackData pendingForce in pendingForces)
            {
                if (!pendingForce.Applied)
                {
                    if (pendingForce.Overridden)
                    {
                        ApplyForce(pendingForce.PlayerController, pendingForce.Attacks.Last());
                    }
                    else
                    {
                        ApplyForce(pendingForce.PlayerController, pendingForce.Attacks.OrderByDescending(item => item.Knockback).First());
                    }
                }
            }
            pendingForces.ForEach(SetApplied);
        }

        private void SetApplied(PlayerAttackData attackData)
        {
            attackData.Applied = true;
        }

        public void AddForce(PlayerController player, AttackData attackData, bool overrideOthers)
        {
            if (player != playerController)
            {
//                print("Adding attackData: x = " + attackData.x + ", y = " + attackData.y);
                if (pendingForces.Any(playerForce => playerForce.PlayerController == player))
                {
//                    print("Existing player");
                    PlayerAttackData playerAttackData= pendingForces.Where(playerForces => playerForces.PlayerController == player).FirstOrDefault();

                    if (!playerAttackData.Overridden)
                    {
                        playerAttackData.Attacks.Add(attackData);
                        playerAttackData.Overridden = overrideOthers;
                    }
                }
                else
                {
//                    print("New player");
                    PlayerAttackData playerAttackData = new PlayerAttackData()
                    {
                        PlayerController = player, 
                    };
                    playerAttackData.Attacks.Add(attackData);
                    if (overrideOthers)
                    {
                        playerAttackData.Overridden = true;
                    }
                    pendingForces.Add(playerAttackData);
                }
            }
        }

        private void ApplyForce(PlayerController player, AttackData attackData)
        {
            // Do I want attacks cancelling out opponents' momentum?
//            print("Applying attackData: x = " + attackData.x + ", y = " + attackData.y);
            // Stagger can be used to cancel stun and launch for weak attacks against strong enemies. Might be better used for other purposes
            if (!attackData.SetKnockback)
            {
                attackData.Knockback = (int) (attackData.Knockback*playerController.GetAttackRatio());
            }
            if (!playerController.facingRight)
            {
                attackData.Direction.x *= -1;
            }

            player.TakeKnockback(attackData);

            if (attackData.Vibrate)
            {
                player.SetVibrate(15, 0f, 1f);
            }
        }
    }
}