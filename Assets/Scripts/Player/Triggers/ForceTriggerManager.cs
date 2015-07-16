using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player.Triggers
{
    public struct PlayerForces
    {
        public PlayerController PlayerController;
        public List<Vector2> Forces;
        public List<int> Damages;
    }

    public class ForceTriggerManager : MonoBehaviour
    {
        private PlayerController playerController;
        private List<PlayerForces> pendingForces;

        private void Awake()
        {
            playerController = transform.parent.GetComponentInChildren<PlayerController>();
        }

        private void OnEnable()
        {
//            print("OnEnable");
            pendingForces = new List<PlayerForces>();
        }

        public void AddForce(PlayerController player, Vector2 force, int damage)
        {
            if (player != playerController && !player.Invincible)
            {
//                print("Adding force: x = " + force.x + ", y = " + force.y);
                if (pendingForces.Any(playerForce => playerForce.PlayerController == player))
                {
//                    print("Existing player");
                    PlayerForces forces = pendingForces.Where(playerForces => playerForces.PlayerController == player).FirstOrDefault();

                    Vector2 appliedForce = new Vector2();
                    int appliedDamage = 0;

                    float highestX = Mathf.Abs(forces.Forces.OrderByDescending(item => item.x).First().x);
                    float highestY = Mathf.Abs(forces.Forces.OrderByDescending(item => item.y).First().y);
                    int highestDamage = Mathf.Abs(forces.Damages.Max());

//                    print("Highest x = " + highestX + ", highest y = " + highestY);

                    if (Mathf.Abs(force.x) > highestX)
                    {
                        appliedForce.x = force.x - highestX;
                    }
                    else
                    {
                        appliedForce.x = 0;
                    }
                    if (Mathf.Abs(force.y) > highestY)
                    {
                        appliedForce.y = force.y - highestY;
                    }
                    else
                    {
                        appliedForce.y = 0;
                    }
                    if (damage > highestDamage)
                    {
                        appliedDamage = damage - highestDamage;
                    }
                    else
                    {
                        appliedDamage = 0;
                    }

                    forces.Forces.Add(force);
                    forces.Damages.Add(damage);

                    ApplyForce(player, appliedForce, appliedDamage);
                }
                else
                {
//                    print("New player");
                    PlayerForces forces = new PlayerForces()
                    {
                        PlayerController = player,
                        Forces = new List<Vector2>(),
                        Damages = new List<int>()
                    };
                    forces.Forces.Add(force);
                    forces.Damages.Add(damage);
                    pendingForces.Add(forces);
                    ApplyForce(player, force, damage);
                }
            }
        }

        private void ApplyForce(PlayerController player, Vector2 force, int damage)
        {
//            print("Applying force: x = " + force.x + ", y = " + force.y);
            if (playerController.facingRight)
            {
                player.IncrementVelocity(force);
            }
            else
            {
                player.IncrementVelocity(-force.x, force.y);
            }
        }
    }
}