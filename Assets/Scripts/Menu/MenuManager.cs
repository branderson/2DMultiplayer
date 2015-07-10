using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using XInputDotNetPure;

namespace Assets.Scripts.Menu
{
    public class MenuManager : MonoBehaviour
    {
//        internal List<MenuControllerInput> Controllers = new List<MenuControllerInput>();
        internal bool[] Controllers = new bool[4];
        internal PlayerCard[] playerCards = new PlayerCard[4];
        private string sceneName = "Level1";

        private void Awake()
        {
        }

        // Use this for initialization
        private void Start()
        {
            Object.DontDestroyOnLoad(this);
            for (int i = 0; i <= 3; i++)
            {
                playerCards[i] = GameObject.Find("Panel" + (i + 1)).GetComponent<PlayerCard>();
                playerCards[i].Init(i + 1);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            // Allow no more than 4 controllers
            if (Controllers.Any(controller => !controller))
            {
                // Allow setting keyboard as a controller
                if (Input.GetButtonDown("PrimaryK") && !playerCards.Any(card => (card.controllerInput.playerNumber == 0 && card.IsActive()))) 
                    // Linq expression checks if any object in Controllers has a playerNumber == 0
                {
                    Activate(0);
                }

                // Allow setting any joystick as a controller
                // TODO: Separate input controller for XBox controllers
                for (int i = 1; i <= Input.GetJoystickNames().Count(); i++)
                {
                    if (Input.GetButtonDown("PrimaryJ" + i) && !playerCards.Any(card => (card.controllerInput.playerNumber == i && card.IsActive())))
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
            if (allReady && playerCards.Any(card => (card.IsActive() && !card.computer)))
            {
                Application.LoadLevel(sceneName);
            }
        }

        public void ActivateComputer(int slot)
        {
            Controllers[slot-1] = true;
            playerCards[slot-1].Activate(-1);
        }

        private void Activate(int inputIndex)
        {
            int slot = 3;

            // TODO: Can't add player below computer (possibly other players as well)
            for (int i = 3; i >= 0; i--)
            {
                if (!Controllers[i])
                {
                    slot = i;
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
                            !playerCards.Any(card => card.controllerInput.XIndex == controller))
                        {
                            playerCards[slot].controllerInput.XIndex = controller;
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