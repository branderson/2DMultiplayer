using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class AnimationEvents : MonoBehaviour
    {
        [SerializeField] private GameObject GroundSmashEffect;
        private PlayerController playerController;
        private SpriteRenderer sprite;
        private PlayerSpriteController spriteController;
        private Animator animator;
        private List<GameObject> instantiatedObjects; 

        private void Awake()
        {
            playerController = GetComponentInChildren<PlayerController>();
            sprite = GetComponentInChildren<SpriteRenderer>();
            spriteController = sprite.GetComponent<PlayerSpriteController>();
            animator = GetComponent<Animator>();
            instantiatedObjects = new List<GameObject>();
        }

        public void PauseAnimation(int frames)
        {
            playerController.StartCoroutine("PauseAnimation", frames);
        }

        public void ApplySpeedX(float x)
        {
            playerController.IncrementSpeedX(x);
        }

        public void ApplySpeedY(float y)
        {
            playerController.IncrementSpeedY(y);
        }

        public void ApplyVelocityX(float x)
        {
            playerController.IncrementVelocityX(x);
        }

        public void ApplyVelocityY(float y)
        {
            playerController.IncrementVelocityY(y);
        }

        public void ShakeSpriteX(int frames)
        {
            // inputs.x is frames, inputs.y is intensity
            playerController.ShakeSpriteX(frames);
        }

        public void StopShakingSprite()
        {
            playerController.StopShaking();
        }

        public void VibrateControllerHard(int frames)
        {
            playerController.SetVibrate(frames, 0f, 1f);
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
            instantiatedObjects.Add((GameObject) GameObject.Instantiate(GroundSmashEffect, new Vector3(xPosition, playerController.transform.position.y, playerController.transform.position.z), Quaternion.identity));
        }

        public void DeleteLastInstantiated()
        {
            Destroy(instantiatedObjects.Last());
            instantiatedObjects.RemoveAt(instantiatedObjects.IndexOf(instantiatedObjects.Last()));
        }
    }
}