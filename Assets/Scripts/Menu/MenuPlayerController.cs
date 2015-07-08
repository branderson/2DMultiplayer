using UnityEngine;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts.Menu
{
    public class MenuPlayerController : MonoBehaviour
    {
        public GameObject Selected;
        private PlayerCard playerCard;
        internal bool timedVibrate = false;
        internal int vibrate = 0;
        internal float leftIntensity = 0f;
        internal float rightIntensity = 0f;

        private MenuControllerInput input;

        void Awake()
        {
            input = GetComponent<MenuControllerInput>();
            playerCard = GetComponent<PlayerCard>();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if (timedVibrate)
            {
                Vibrate();
            }
        }

        public void PressPrimary()
        {
            
        }

        public void PressSecondary()
        {
            if (playerCard.IsReady())
            {
                playerCard.UnReady();
            }
        }

        public void PressStart()
        {
            if (!playerCard.IsReady())
            {
                playerCard.Ready();
            }
        }

        public void PressRestart()
        {
            
        }

        private void Vibrate()
        {
            if (vibrate == 0)
            {
                timedVibrate = false;
                input.StopVibration();
            }
            else
            {
                // TODO: Set tuple to control Vibration intensity
                input.VibrateController(leftIntensity, rightIntensity);
                vibrate -= 1;
            }
        }

        public void SetTimedVibrate(int frames, float leftIntensity, float rightIntensity)
        {
            timedVibrate = true;
            this.vibrate = frames;
            this.leftIntensity = leftIntensity;
            this.rightIntensity = rightIntensity;
        }

        public bool IsActive()
        {
            return playerCard.IsActive();
        }
    }
}