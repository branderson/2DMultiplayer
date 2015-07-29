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

        public void ShakeSpriteX(int frames)
        {
            localPosition = transform.localPosition;
            StartCoroutine("ShakeSprite", frames);
        }

        public void StopShaking()
        {
            StopCoroutine("ShakeSprite");
        }

        private IEnumerator ShakeSprite(int frames)
        {
            while (frames > 0)
            {
                frames--;
                if (frames%10 == 0)
                {
                    transform.localPosition = localPosition + new Vector2(.05f, 0);
                }
                else if (frames%5 == 0)
                {
                    transform.localPosition = localPosition - new Vector2(.05f, 0);
                }
                yield return null;
            }
            transform.localPosition = localPosition;
        }
    }
}