using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    public class DestroySelf : MonoBehaviour
    {
        public void SelfDestruct()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}