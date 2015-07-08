using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class PlayerCard : MonoBehaviour
    {
        private Transform transform;
        private GameObject title;
        private GameObject instruction;
        private Image panelImage;
        private Text titleText;
        private Text instructionText;
        private MenuControllerInput controllerInput;
        private MenuPlayerController playerController;
        private int number;
        private bool active;
        private bool ready;

        public void Init(int playerNumber)
        {
            this.number = playerNumber;
            this.titleText.text = "Player " + number;
        }

        private void Awake()
        {
            active = false;
            ready = false;
            transform = GetComponent<Transform>();
            playerController = GetComponent<MenuPlayerController>();
            controllerInput = GetComponent<MenuControllerInput>();
        }

        // Use this for initialization
        private void Start()
        {
            title = transform.Find("TitleText").gameObject;
            instruction = transform.Find("Instruction").gameObject;
            panelImage = GetComponent<Image>();
            titleText = title.GetComponent<Text>();
            instructionText = instruction.GetComponent<Text>();
            this.instructionText.text = "Press A";
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
            playerController.Selected = gameObject;
            Activate();
        }

        public void Ready()
        {
            ready = true;
            panelImage.color = Color.green;
            instructionText.text = "Ready!";
            playerController.SetTimedVibrate(12, 0f, .8f);
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

    }
}
