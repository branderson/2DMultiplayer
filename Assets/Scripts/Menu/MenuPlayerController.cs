using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts.Menu
{
    public class MenuPlayerController : MonoBehaviour
    {
        [SerializeField] public MenuSelectable InitialSelection;
        internal MenuSelectable Selected;
        private PlayerCard playerCard;
        private PointerEventData pointer = new PointerEventData(EventSystem.current);
        internal int PlayerNumber;
        internal int CharacterIndex = 0;

        internal bool active;

        private MenuInputController input;

        public void Init(PlayerCard card)
        {
            playerCard = card;
        }

        void Awake()
        {
            active = false;
            input = GetComponent<MenuInputController>();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Deactivate()
        {
            active = false;
            if (Selected != null)
            {
                Selected.Unselect(this, pointer);
            }
        }

        public void SetCharacter(int index)
        {
            CharacterIndex = index;
        }

        public void SetSelected(MenuSelectable selection)
        {
            if (Selected != null)
            {
                Selected.Unselect(this, pointer);
            }
            Selected = selection;
            selection.Select(this, pointer);
        }

        public void PressUp()
        {
            if (Selected.up != null)
            {
                Selected.Up(this);
                if (Selected.up.AllowSelection(this))
                {
                    SetSelected(Selected.up);
                }
            }
        }

        public void PressDown()
        {
            if (Selected.down != null)
            {
                Selected.Down(this);
                if (Selected.down.AllowSelection(this))
                {
                    SetSelected(Selected.down);
                }
            }
        }

        public void PressLeft()
        {
            // TODO: Selections are becoming null or something when deactivating
            if (Selected.left != null)
            {
                Selected.Left(this);
                if (Selected.left.AllowSelection(this))
                {
                    SetSelected(Selected.left);
                }
            }
        }

        public void PressRight()
        {
            if (Selected.right != null)
            {
                Selected.Right(this);
                if (Selected.right.AllowSelection(this))
                {
                    SetSelected(Selected.right);
                }
            }
        }

        public void PressPrimary()
        {
            if (Selected != null)
            {
                Selected.Primary(this, pointer);
            }
        }

        public void PressSecondary()
        {
            if (Selected != null)
            {
                Selected.Secondary(this);
                if (playerCard != null)
                {
                    if (playerCard.IsReady() && !input.Computer)
                    {
                        playerCard.UnReady();
                    }
                        // TODO: Not a good place to put this. Prevents backing up through menus
                    else if (playerCard.IsActive() && !input.Computer)
                    {
                        playerCard.Deactivate();
                    }
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

        public IEnumerator Vibrate(int frames, float leftIntensity, float rightIntensity)
        {
            for (int i = 0; i < frames; i++)
            {
                input.VibrateController(leftIntensity, rightIntensity);
                yield return null;
            }
            input.StopVibration();
        }

        public void SetVibrate(int frames, float leftIntensity, float rightIntensity)
        {
            StartCoroutine(Vibrate(frames, leftIntensity, rightIntensity));
        }

        public bool IsActive()
        {
            return playerCard.IsActive();
        }
    }
}