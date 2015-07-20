﻿using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class PlayerCard : MonoBehaviour, ISelectable
    {
        private MenuManager manager;
        private MenuSelectable menuSelectable;
        private GameObject title;
        private GameObject instruction;
        private ToggleBox tapJumpBox;
        private ToggleBox vibrationBox;
        private ToggleBox dPadBox;
        private Image panelImage;
        private Text titleText;
        private Text instructionText;
        internal MenuInputController InputController;
        internal MenuPlayerController PlayerController;
        internal int number;
        internal bool computer = false;
        private bool active;
        private bool ready;

        public void Init(PlayerConfig config)
        {
            PlayerController.playerNumber = config.Slot;
            computer = config.Computer;
            InputController.ControllerNumber = config.ControllerIndex;
//            InputController.TapJump = config.TapJump;
//            InputController.Vibration = config.Vibration;
            InputController.XIndex = config.XIndex;
            InputController.UseXIndex = config.UseXIndex;
            tapJumpBox.SetToggle(config.TapJump);
            vibrationBox.SetToggle(config.Vibration);
            dPadBox.SetToggle(config.DPad);
            this.active = config.Active;
            this.number = config.Slot;
            this.titleText.text = "None";
            this.instructionText.text = "Press A";
        }

        private void Awake()
        {
            active = false;
            ready = false;
            manager = FindObjectOfType<MenuManager>();
            menuSelectable = GetComponent<MenuSelectable>();
            PlayerController = GetComponent<MenuPlayerController>();
            InputController = GetComponent<MenuInputController>();

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
            computer = true;
            titleText.text = "Computer";
            Ready();
        }

        public void Activate(int controllerNumber)
        {
            InputController.Init(controllerNumber);
            active = true;
            panelImage.color = Color.yellow;
            titleText.text = "Player " + number;
            instructionText.text = "Press Start\nWhen Ready";
            PlayerController.SetVibrate(12, 0f, .8f);
            PlayerController.SetSelected(PlayerController.InitialSelection);
        }

        public void Deactivate()
        {
            // TODO: Conflicts with MenuManager's controller list
            manager.Deactivate(number);
            active = false;
            panelImage.color = Color.white;
            titleText.text = "None";
            instructionText.text = "Press A";
            PlayerController.Deactivate();
            InputController.Deactivate();
            // TODO: Deactivate InputController
        }

        public void Ready()
        {
            ready = true;
            panelImage.color = Color.green;
            instructionText.text = "Ready!";
//            PlayerController.SetVibrate(12, 0f, .8f);
        }

        public void UnReady()
        {
            ready = false;
            Activate(InputController.ControllerNumber);
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
