using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts.Menu
{
    public class MenuPlayerController : MonoBehaviour
    {
        [SerializeField] public MenuSelectable InitialSelection;
        internal MenuSelectable selected;
        private PlayerCard playerCard;
        private PointerEventData pointer = new PointerEventData(EventSystem.current);
        internal int playerNumber;
        internal bool timedVibrate = false;
        internal int vibrate = 0;
        internal float leftIntensity = 0f;
        internal float rightIntensity = 0f;

        private MenuInputController input;

        void Awake()
        {
            input = GetComponent<MenuInputController>();
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
                selected.Unselect(this, pointer);
            }
        }

        public void SetSelected(MenuSelectable selection)
        {
            if (selected != null)
            {
                selected.Unselect(this, pointer);
            }
            selected = selection;
            selection.Select(this, pointer);
        }

        public void PressUp()
        {
            if (selected.up != null)
            {
                selected.Up(this);
                if (selected.up.AllowSelection(this))
                {
                    SetSelected(selected.up);
                }
            }
        }

        public void PressDown()
        {
            if (selected.down != null)
            {
                selected.Down(this);
                if (selected.down.AllowSelection(this))
                {
                    SetSelected(selected.down);
                }
            }
        }

        public void PressLeft()
        {
            // TODO: Selections are becoming null or something when deactivating
            if (selected.left != null)
            {
                selected.Left(this);
                if (selected.left.AllowSelection(this))
                {
                    SetSelected(selected.left);
                }
            }
        }

        public void PressRight()
        {
            if (selected.right != null)
            {
                selected.Right(this);
                if (selected.right.AllowSelection(this))
                {
                    SetSelected(selected.right);
                }
            }
        }

        public void PressPrimary()
        {
            if (selected != null)
            {
                selected.Primary(this, pointer);
            }
        }

        public void PressSecondary()
        {
            if (selected != null)
            {
                selected.Secondary(this);
                if (playerCard.IsReady() && !playerCard.computer)
                {
                    playerCard.UnReady();
                }
                    // TODO: Not a good place to put this. Prevents backing up through menus
                else if (playerCard.IsActive() && !playerCard.computer)
                {
                    playerCard.Deactivate();
                }
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