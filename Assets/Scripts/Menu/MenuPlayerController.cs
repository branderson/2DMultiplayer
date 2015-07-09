using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts.Menu
{
    public class MenuPlayerController : MonoBehaviour
    {
        private MenuSelectable selected;
        private PlayerCard playerCard;
        private PointerEventData pointer = new PointerEventData(EventSystem.current);
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
                selected.Unselect(this);
            }
        }

        public void SetSelected(MenuSelectable selection)
        {
            if (selected != null)
            {
                selected.Unselect(this);
                ExecuteEvents.Execute(selected.gameObject, pointer, ExecuteEvents.pointerExitHandler);
            }
            selected = selection;
            selection.Select(this);
            ExecuteEvents.Execute(selected.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        }

        public void PressUp()
        {
            selected.Up(this);
            if (selected.up != null)
            {
                if (selected.up.AllowSelection(this))
                {
                    SetSelected(selected.up);
                }
            }
        }

        public void PressDown()
        {
            selected.Down(this);
            if (selected.down != null)
            {
                if (selected.down.AllowSelection(this))
                {
                    SetSelected(selected.down);
                }
            }
        }

        public void PressLeft()
        {
            selected.Left(this);
            if (selected.left != null)
            {
                if (selected.left.AllowSelection(this))
                {
                    SetSelected(selected.left);
                }
            }
        }

        public void PressRight()
        {
            selected.Right(this);
            if (selected.right != null)
            {
                if (selected.right.AllowSelection(this))
                {
                    SetSelected(selected.right);
                }
            }
        }

        public void PressPrimary()
        {
            selected.Primary(this);
            ExecuteEvents.Execute(selected.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }

        public void PressSecondary()
        {
            selected.Secondary(this);
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