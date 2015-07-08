using UnityEngine;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts.Menu
{
    public class MenuPlayerController : MonoBehaviour
    {
        private MenuSelectable selected;
        private PlayerCard playerCard;
        internal int playerNumber;
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

        public void Deactivate()
        {
            if (selected != null)
            {
                selected.Unselect(playerNumber);
            }
        }

        public void SetSelected(MenuSelectable selection)
        {
            if (selected != null)
            {
                selected.Unselect(playerNumber);
            }
            selected = selection;
            selection.Select(playerNumber);
        }

        public void PressUp()
        {
            selected.Up(playerNumber);
            if (selected.up != null)
            {
                if (selected.up.AllowSelection(playerNumber))
                {
                    SetSelected(selected.up);
                }
            }
        }

        public void PressDown()
        {
            selected.Down(playerNumber);
            if (selected.down != null)
            {
                if (selected.down.AllowSelection(playerNumber))
                {
                    SetSelected(selected.down);
                }
            }
        }

        public void PressLeft()
        {
            selected.Left(playerNumber);
            if (selected.left != null)
            {
                if (selected.left.AllowSelection(playerNumber))
                {
                    SetSelected(selected.left);
                }
            }
        }

        public void PressRight()
        {
            selected.Right(playerNumber);
            if (selected.right != null)
            {
                if (selected.right.AllowSelection(playerNumber))
                {
                    SetSelected(selected.right);
                }
            }
        }

        public void PressPrimary()
        {
            selected.Primary(playerNumber);
        }

        public void PressSecondary()
        {
            selected.Secondary(playerNumber);
            if (playerCard.IsReady())
            {
                playerCard.UnReady();
            }
            else if (playerCard.IsActive())
            {
                playerCard.Deactivate();
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