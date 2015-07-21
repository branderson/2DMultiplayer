using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class AnimationEvents : MonoBehaviour
    {
        [SerializeField] private GameObject GroundSmashEffect;
        private PlayerController playerController;
        private SpriteRenderer sprite;
        private Animator animator;

        private void Awake()
        {
            playerController = GetComponentInChildren<PlayerController>();
            sprite = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public void PauseAnimation(int frames)
        {
            StartCoroutine(playerController.PauseAnimation(frames));
        }

        public void ApplySpeedX(float x)
        {
            playerController.IncrementSpeedX(x);
        }

        public void ApplySpeedY(float y)
        {
            playerController.IncrementSpeedY(y);
        }

        public void ShakeSpriteX(float intensity)
        {
            
        }

        public void InstantiateGroundSmashEffect(float xOffset)
        {
            float xPosition = 0f;
            if (playerController.facingRight)
            {
                xPosition = playerController.transform.position.x + xOffset;
            }
            else
            {
                xPosition = playerController.transform.position.x - xOffset;
            }
            GameObject.Instantiate(GroundSmashEffect, new Vector3(xPosition, playerController.transform.position.y, playerController.transform.position.z), Quaternion.identity);
        }
    }
}