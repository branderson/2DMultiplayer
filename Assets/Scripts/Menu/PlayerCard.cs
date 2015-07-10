using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class PlayerCard : MonoBehaviour, ISelectable
    {
        private MenuManager manager;
        private Transform transform;
        private MenuSelectable menuSelectable;
        private GameObject title;
        private GameObject instruction;
        private Image panelImage;
        private Text titleText;
        private Text instructionText;
        internal MenuControllerInput controllerInput;
        internal MenuPlayerController playerController;
        private int number;
        internal bool computer = false;
        private bool active;
        private bool ready;

        public void Init(int playerNumber)
        {
            playerController.playerNumber = playerNumber;
            this.number = playerNumber;
            this.titleText.text = "None";
            this.instructionText.text = "Press A";
        }

        private void Awake()
        {
            active = false;
            ready = false;
            manager = FindObjectOfType<MenuManager>();
            transform = GetComponent<Transform>();
            menuSelectable = GetComponent<MenuSelectable>();
            playerController = GetComponent<MenuPlayerController>();
            controllerInput = GetComponent<MenuControllerInput>();

            title = transform.Find("TitleText").gameObject;
            instruction = transform.Find("Instruction").gameObject;
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

        public void Activate()
        {
            active = true;
            panelImage.color = Color.yellow;
            titleText.text = "Player " + number;
            instructionText.text = "Press Start\nWhen Ready";
            playerController.SetTimedVibrate(12, 0f, .8f);
        }

        public void Activate(int controllerNumber)
        {
            if (controllerNumber >= 0)
            {
                controllerInput.Init(controllerNumber);
                Activate();
                playerController.SetSelected(playerController.InitialSelection);
            }
            else
            {
                Activate();
            }
        }

        public void Deactivate()
        {
            // TODO: Conflicts with MenuManager's controller list
            manager.Deactivate(number);
            active = false;
            panelImage.color = Color.white;
            titleText.text = "None";
            instructionText.text = "Press A";
            playerController.Deactivate();
            // TODO: Deactivate controllerInput
        }

        public void Ready()
        {
            ready = true;
            panelImage.color = Color.green;
            instructionText.text = "Ready!";
//            playerController.SetTimedVibrate(12, 0f, .8f);
        }

        public void UnReady()
        {
            ready = false;
            Activate();
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
            if (player.playerNumber != number)
            {
                if (computer == false)
                {
                    computer = true;
                    Deactivate();
                    manager.ActivateComputer(number);
                    titleText.text = "Computer";
                    Ready();
                }
                else
                {
                    computer = false;
                    UnReady();
                    Deactivate();
                }
            }
        }


        public void Secondary(MenuPlayerController player)
        {
//            if (IsReady())
//            {
//                UnReady();
//            }
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
