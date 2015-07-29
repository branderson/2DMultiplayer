using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class PlayerCard : MonoBehaviour, ISelectable
    {
        [SerializeField] private int slot;
        private CharacterMenuManager manager;
        private MenuSelectable menuSelectable;
        private GameObject title;
        private GameObject instruction;
        private ToggleBox tapJumpBox;
        private ToggleBox vibrationBox;
        private ToggleBox dPadBox;
        private Image panelImage;
        private Text titleText;
        private Text instructionText;
        internal MenuInputController inputController;
        internal MenuPlayerController playerController;
        internal int number;
        internal bool computer = false;
        private bool active;
        private bool ready;

        public void Init(PlayerConfig config)
        {
            playerController.playerNumber = config.Slot;
            computer = config.Computer;
            playerController.computer = config.Computer;
            inputController.ControllerNumber = config.ControllerIndex;
//            inputController.TapJump = config.TapJump;
//            inputController.Vibration = config.Vibration;
            inputController.XIndex = config.XIndex;
            inputController.UseXIndex = config.UseXIndex;
            tapJumpBox.SetToggle(config.TapJump);
            vibrationBox.SetToggle(config.Vibration);
            dPadBox.SetToggle(config.DPad);
            this.active = config.Active;
            playerController.active = config.Active;
            this.number = config.Slot;
            this.titleText.text = "None";
            this.instructionText.text = "Press A";
        }

        private void Awake()
        {
            active = false;
            ready = false;
            manager = FindObjectOfType<CharacterMenuManager>();
            menuSelectable = GetComponent<MenuSelectable>();
            playerController = manager.transform.Find("Player"+slot).GetComponent<MenuPlayerController>();
            inputController = manager.transform.Find("Player"+slot).GetComponent<MenuInputController>();

            title = transform.Find("TitleText").gameObject;
            instruction = transform.Find("Instruction").gameObject;
            tapJumpBox = transform.Find("TapJump").gameObject.GetComponent<ToggleBox>();
            vibrationBox = transform.Find("Vibrate").gameObject.GetComponent<ToggleBox>();
            dPadBox = transform.Find("DPad").gameObject.GetComponent<ToggleBox>();
            panelImage = GetComponent<Image>();
            titleText = title.GetComponent<Text>();
            instructionText = instruction.GetComponent<Text>();
        }

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void SetText(string text)
        {
            instructionText.text = text;
        }

        public void ActivateComputer()
        {
            active = true;
            playerController.active = true;
            computer = true;
            playerController.computer = true;
            inputController.Computer = true;
            titleText.text = "Computer";
            Ready();
        }

        public void Activate(int controllerNumber)
        {
            inputController.Init(controllerNumber);
            active = true;
            playerController.active = true;
            panelImage.color = Color.yellow;
            titleText.text = "Player " + number;
            instructionText.text = "Press Start\nWhen Ready";
            playerController.SetVibrate(12, 0f, .8f);
            playerController.SetSelected(playerController.InitialSelection);
        }

        public void Deactivate()
        {
            // TODO: Conflicts with CharacterMenuManager's controller list
            manager.Deactivate(number);
            active = false;
            panelImage.color = Color.white;
            titleText.text = "None";
            instructionText.text = "Press A";
            playerController.Deactivate();
            inputController.Deactivate();
            // TODO: Deactivate inputController
        }

        public void Ready()
        {
            ready = true;
            panelImage.color = Color.green;
            instructionText.text = "Ready!";
//            playerController.SetVibrate(12, 0f, .8f);
        }

        public void UnReady()
        {
            ready = false;
            Activate(inputController.ControllerNumber);
        }

        public bool IsActive()
        {
            return active;
        }

        public bool IsReady()
        {
            return ready;
        }


        public void Select(int playerNumber, PointerEventData pointer)
        {
            titleText.color = Color.blue;
        }

        public void Unselect(int playerNumber, PointerEventData pointer)
        {
            if (!menuSelectable.IsSelected())
            {
                titleText.color = Color.black;
            }
        }


        public void Primary(MenuPlayerController player, PointerEventData pointer)
        {
            // TODO: Perhaps allow players to turn themselves to a computer? Should probably deactivate that player without moving queue down the line
            // TODO: Maybe a cycle of Player, Computer, None
            // TODO: Allow selection via mouse hover
            // TODO: Sometimes cycles Computer, None on player 1
            if (true) //player.ControllerNumber != number)
            {
                if (!computer)
                {
                    Deactivate();
                    manager.ActivateComputer(number);
                }
                else
                {
                    computer = false;
                    playerController.computer = false;
                    UnReady();
                    Deactivate();
                }
            }
        }


        public void Secondary(MenuPlayerController player)
        {
        }

        public void Left(MenuPlayerController player)
        {
        }

        public void Right(MenuPlayerController player)
        {
        }

        public bool AllowSelection(int playerNumber)
        {
            return true;
        }
    }
}
