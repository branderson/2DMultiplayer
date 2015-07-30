using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerSpriteController : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Vector2 localPosition;

        public void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}