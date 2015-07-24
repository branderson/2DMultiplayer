using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player.Triggers
{
    public class PlayerForces
    {
        public PlayerController PlayerController;
        public List<Vector2> Forces = new List<Vector2>();
        public List<int> Damages = new List<int>();
        public List<bool> Vibrate = new List<bool>();
        public List<bool> Stun = new List<bool>(); 
        public bool Overridden = false;
        public bool Applied = false;
    }

    public class ForceTriggerManager : MonoBehaviour
    {
        private PlayerController playerController;
        private List<PlayerForces> pendingForces;

        private void Awake()
        {
            playerController = transform.parent.GetComponentInChildren<PlayerController>();
            pendingForces = new List<PlayerForces>();
        }

        private void OnEnable()
        {
//            print("OnEnable");
            pendingForces.Clear();
        }

        private void Update()
        {
//            int forceLength = pendingForces.Count();
            foreach (PlayerForces pendingForce in pendingForces)
            {
                if (!pendingForce.Applied && !pendingForce.PlayerController.Invincible)
                {
                    if (pendingForce.Overridden)
                    {
                        ApplyForce(pendingForce.PlayerController, pendingForce.Forces.Last(), pendingForce.Damages.Last(), pendingForce.Vibrate.Last(), pendingForce.Stun.Last());
                    }
                    else
                    {
                        float highestX = pendingForce.Forces.OrderByDescending(item => Mathf.Abs(item.x)).First().x;
                        float highestY = pendingForce.Forces.OrderByDescending(item => Mathf.Abs(item.y)).First().y;
                        int highestDamage = pendingForce.Damages.Max();
                        bool stun = pendingForce.Stun.Any(item => item);
                        bool vibrate = pendingForce.Vibrate.Any(item => item);
                        ApplyForce(pendingForce.PlayerController, new Vector2(highestX, highestY), highestDamage, vibrate, stun);
                    }
                }
            }
            pendingForces.ForEach(SetApplied);
        }

        private void SetApplied(PlayerForces force)
        {
            force.Applied = true;
        }

        public void AddForce(PlayerController player, Vector2 force, int damage, bool stun, bool overrideOthers, bool vibrate)
        {
            if (player != playerController && !player.Invincible)
            {
//                print("Adding force: x = " + force.x + ", y = " + force.y);
                if (pendingForces.Any(playerForce => playerForce.PlayerController == player))
                {
//                    print("Existing player");
                    PlayerForces forces = pendingForces.Where(playerForces => playerForces.PlayerController == player).FirstOrDefault();

                    if (!forces.Overridden)
                    {
                        forces.Forces.Add(force);
                        forces.Damages.Add(damage);
                        forces.Vibrate.Add(vibrate);
                        forces.Stun.Add(stun);
                        forces.Overridden = overrideOthers;
                    }
                }
                else
                {
//                    print("New player");
                    PlayerForces forces = new PlayerForces()
                    {
                        PlayerController = player, 
                    };
                    forces.Forces.Add(force);
                    forces.Damages.Add(damage);
                    forces.Vibrate.Add(vibrate);
                    forces.Stun.Add(stun);
                    if (overrideOthers)
                    {
                        forces.Overridden = true;
                    }
                    pendingForces.Add(forces);
                }
            }
        }

        private void ApplyForce(PlayerController player, Vector2 force, int damage, bool vibrate, bool stun)
        {
            // Do I want attacks cancelling out opponents' momentum?
//            print("Applying force: x = " + force.x + ", y = " + force.y);
            if (playerController.facingRight)
            {
                player.IncrementVelocity(force*playerController.GetAttackRatio()*player.GetDamageRatio()/player.WeightRatio - player.GetVelocity());
            }
            else
            {
                player.IncrementVelocity(-force.x*playerController.GetAttackRatio()*player.GetDamageRatio()/player.WeightRatio - player.GetVelocityX(), force.y*playerController.GetAttackRatio()*player.GetDamageRatio()/player.WeightRatio - player.GetVelocityY());
            }
            player.TakeDamage(damage);

            int stunFrames = (int) Mathf.Ceil(force.x + force.y);
            player.Stun(stunFrames);

            if (vibrate)
            {
                player.SetVibrate(15, .8f, .5f);
            }
        }
    }
}