using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;

namespace Assets.Scripts.Player
{
    public class PushOpponents : MonoBehaviour {

        private void Awake()
        {

        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            print("OnTriggerEnter");
            print(other.gameObject.tag);
            if (other.gameObject.tag == "PlayerTrigger")
            {
                other.GetComponentInParent<PlayerController>().IncrementVelocityX(transform.parent.GetComponent<Rigidbody2D>().velocity.x * .05f);
                print("Moving");
            }
        }
    }
}