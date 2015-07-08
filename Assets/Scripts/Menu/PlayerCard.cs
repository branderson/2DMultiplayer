using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class PlayerCard : MonoBehaviour, ISelectable
    {
        private Transform transform;
        private MenuSelectable menuSelectable;
        private GameObject title;
        private GameObject instruction;
        private Image panelImage;
        private Text titleText;
        private Text instructionText;
        private MenuControllerInput controllerInput;
        private MenuPlayerController playerController;
        private int number;
        private bool computer = false;
        private bool active;
        private bool ready;

        public void Init(int playerNumber)
        {
            playerController.playerNumber = playerNumber;
            this.number = playerNumber;
            this.titleText.text = "Player " + number;
            this.instructionText.text = "Press A";
        }

        private void Awake()
        {
            active = false;
            ready = false;
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
            instructionText.text = "Press Start\nWhen Ready";
            playerController.SetTimedVibrate(12, 0f, .8f);
        }

        public void Activate(int controllerNumber)
        {
            controllerInput.Init(controllerNumber);
            Activate();
            playerController.SetSelected(menuSelectable);
        }

        public void Deactivate()
        {
            // TODO: Conflicts with MenuManager's controller list
            active = false;
            panelImage.color = Color.white;
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


        public void Select(int playerNumber)
        {
            titleText.color = Color.blue;
        }

        public void Unselect(int playerNumber)
        {
            titleText.color = Color.black;
        }


        public void Primary(int playerNumber)
        {
            if (playerNumber != number)
            {
                if (computer == false)
                {
                    computer = true;
                    titleText.text = "Computer";
                    Ready();
                }
                else
                {
                    computer = false;
                    titleText.text = "Player" + number;
                    UnReady();
                    Deactivate();
                }
            }
        }


        public void Secondary(int playerNumber)
        {
//            if (IsReady())
//            {
//                UnReady();
//            }
        }

        public void Left(int playerNumber)
        {
        }

        public void Right(int playerNumber)
        {
        }

        public bool AllowSelection(int playerNumber)
        {
            return true;
        }
    }
}
