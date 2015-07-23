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
                        ApplyForce(pendingForce.PlayerController, pendingForce.Forces.Last(), pendingForce.Damages.Last(), pendingForce.Vibrate.Last());
                    }
                    else
                    {
                        float highestX = pendingForce.Forces.OrderByDescending(item => Mathf.Abs(item.x)).First().x;
                        float highestY = pendingForce.Forces.OrderByDescending(item => Mathf.Abs(item.y)).First().y;
                        int highestDamage = pendingForce.Damages.Max();
                        bool vibrate = pendingForce.Vibrate.Any(item => item);
                        ApplyForce(pendingForce.PlayerController, new Vector2(highestX, highestY), highestDamage, vibrate);
                    }
                }
            }
            pendingForces.ForEach(SetApplied);
        }

        private void SetApplied(PlayerForces force)
        {
            force.Applied = true;
        }

        public void AddForce(PlayerController player, Vector2 force, int damage, bool overrideOthers, bool vibrate)
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
                    if (overrideOthers)
                    {
                        forces.Overridden = true;
                    }
                    pendingForces.Add(forces);
                }
            }
        }

        private void ApplyForce(PlayerController player, Vector2 force, int damage, bool vibrate)
        {
            // Do I want attacks cancelling out opponents' momentum?
//            print("Applying force: x = " + force.x + ", y = " + force.y);
            if (playerController.facingRight)
            {
                player.IncrementVelocity(force*player.GetDamageRatio()*(1f/player.WeightRatio) - player.GetVelocity());
            }
            else
            {
                player.IncrementVelocity(-force.x*player.GetDamageRatio()*(1f/player.WeightRatio) - player.GetVelocityX(), force.y*player.GetDamageRatio()*(1f/player.WeightRatio) - player.GetVelocityY());
            }
            player.TakeDamage(damage);
            if (vibrate)
            {
                player.SetVibrate(15, .8f, .5f);
            }
        }
    }
}