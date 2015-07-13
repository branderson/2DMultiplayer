using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;
using UnityEditor;
using UnityEngine.UI;
using XInputDotNetPure;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Menu
{
    public class MenuManager : MonoBehaviour
    {
//        internal List<MenuInputController> Controllers = new List<MenuInputController>();
        internal GameManager gameManager;
        internal bool[] Controllers = new bool[4];
        internal PlayerCard[] playerCards = new PlayerCard[4];
        private string sceneName = "Level1";

        private void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }

        // Use this for initialization
        private void Start()
        {
            for (int i = 0; i <= 3; i++)
            {
                playerCards[i] = GameObject.Find("Panel" + (i + 1)).GetComponent<PlayerCard>();
                playerCards[i].Init(gameManager.PlayerConfig[i]);
            }

            // Load player settings from gameManager
            for (int i = 0; i <= 3; i++)
            {
                // TODO: What about case where player has been added, then removed (no longer active), and later player added in place (should skip slot) (or should it?)
                if (gameManager.PlayerConfig[i].Active && !gameManager.PlayerConfig[i].Computer)
                {
                    Activate(gameManager.PlayerConfig[i].ControllerIndex);
                }
                else if (gameManager.PlayerConfig[i].Active && gameManager.PlayerConfig[i].Computer)
                {
                    ActivateComputer(gameManager.PlayerConfig[i].Slot);
                }
            }
        }

        private void Update()
        {
            // If no controllers active, next back goes to title
            if (!Controllers.Any(controller => controller))
            {
                if (Input.GetButtonDown("GlobalBack"))
                {
                    Application.LoadLevel("TitleMenu");
                }
            }
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            // TODO: Back out when someone presses back when no non-computer players left
            // Allow no more than 4 controllers
            if (Controllers.Any(controller => !controller))
            {
                // Allow setting keyboard as a controller
                if (Input.GetButtonDown("PrimaryK") && !playerCards.Any(card => (card.InputController.ControllerNumber == 0 && card.IsActive())))
                    // Linq expression checks if any object in Controllers has a ControllerNumber == 0
                {
                    Activate(0);
                }

                // Allow setting any joystick as a controller
                // TODO: Separate input controller for XBox controllers
                for (int i = 1; i <= Input.GetJoystickNames().Count(); i++)
                {
                    if (Input.GetButtonDown("PrimaryJ" + i) && !playerCards.Any(card => (card.InputController.ControllerNumber == i && card.IsActive())))
                    {
                        Activate(i);
                    }
                }
            }

            bool allReady = true;

            // Check if all added controllers have pressed start
            for (int controller = 0; controller < 4; controller++)
            {
                if (!playerCards[controller].IsReady() && playerCards[controller].IsActive())
                {
                    allReady = false;
                }
            }

            // If all players ready, load level
            // TODO: Load level select scene here instead
            if (allReady && playerCards.Any(card => (card.IsActive() && !card.computer)))
            {
                Object.DontDestroyOnLoad(this);
                Application.LoadLevel(sceneName);
            }
        }

        public void ActivateComputer(int slot)
        {
            Controllers[slot-1] = true;
            playerCards[slot-1].ActivateComputer();
        }

        private void Activate(int inputIndex)
        {
            int slot = 3;

            for (int i = 3; i >= 0; i--)
            {
                if (!Controllers[i])
                {
                    slot = i;
                }
            }

            foreach (PlayerCard card in playerCards)
            {
                if (card.InputController.ControllerNumber == inputIndex)
                {
                    slot = card.number - 1;
                }
            }

            if (inputIndex == 0)
            {
                Controllers[slot] = true;
                playerCards[slot].Activate(0);
                print("Adding keyboard");
            }
            else
            {
                Controllers[slot] = true;
                playerCards[slot].Activate(inputIndex);
                for (int controller = 0; controller < 4; controller++)
                {
                    if (GamePad.GetState((PlayerIndex) controller).IsConnected)
                    {
                        // TODO: Vibrations can be mapped to the wrong controller if selected on the exact same frame
                        if (GamePad.GetState((PlayerIndex) controller).Buttons.A == ButtonState.Pressed &&
                            !playerCards.Any(card => card.InputController.XIndex == controller))
                        {
                            playerCards[slot].InputController.XIndex = controller;
                        }
                    }
                }
                print("Adding joystick " + inputIndex);
            }
        }

        public void Deactivate(int number)
        {
            Controllers[number - 1] = false;
        }

        public void SetScene(string scene)
        {
            sceneName = scene;
            print("Setting scene to " + scene);
        }
    }
}