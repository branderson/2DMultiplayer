using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XInputDotNetPure;

namespace Assets.Scripts.Menu
{
    public class MenuManager : MonoBehaviour
    {
        internal List<MenuControllerInput> Controllers = new List<MenuControllerInput>();
        internal List<int> XIndices = new List<int>();

        private PlayerCard[] playerCards = new PlayerCard[4];

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
            if (Controllers.Count() < 4)
            {
                // Allow setting keyboard as a controller
                if (Input.GetButtonDown("PrimaryK") && !Controllers.Any(input => input.playerNumber == 0)) // Linq expression checks if any object in Controllers has a playerNumber == 0
                {
                    MenuControllerInput controllerInput = playerCards[Controllers.Count()].GetComponent<MenuControllerInput>();
                    Controllers.Add(controllerInput);
                    XIndices.Add(-1);
                    playerCards[Controllers.Count() - 1].Activate(0);
                    print("Adding keyboard");
                }

                // Allow setting any joystick as a controller
                // TODO: Separate input controller for XBox controllers
                for (int i = 1; i <= Input.GetJoystickNames().Count(); i++)
                {
                    if (Input.GetButtonDown("PrimaryJ" + i) && !Controllers.Any(input => input.playerNumber == i))
                    {
                        MenuControllerInput controllerInput = playerCards[Controllers.Count()].GetComponent<MenuControllerInput>();
                        Controllers.Add(controllerInput);
                        playerCards[Controllers.Count() - 1].Activate(i);
                        bool xIndexAdded = false;
                        for (int controller = 0; controller < 4; controller++)
                        {
                            if (GamePad.GetState((PlayerIndex) controller).IsConnected)
                            {
                                // TODO: Vibrations can be mapped to the wrong controller if selected on the exact same frame
                                if (GamePad.GetState((PlayerIndex) controller).Buttons.A == ButtonState.Pressed &&
                                    !XIndices.Contains(controller))
                                {
                                    controllerInput.XIndex = controller;
                                    XIndices.Add(controller);
                                    xIndexAdded = true;
                                }
                            }
                        }
                        if (!xIndexAdded)
                        {
                            XIndices.Add(-1);
                        }
                        print("Adding joystick " + i);
                    }
                }
            }

            bool allReady = true;

            // Check if all added controllers have pressed start
            for (int controller = 0; controller < Controllers.Count(); controller++)
            {
                if (playerCards[controller].IsReady() != true)
                {
                    allReady = false;
                }
            }

            // If all players ready, load level
            if (allReady && Controllers.Any())
            {
                Application.LoadLevel("TestScene");
            }
        }
    }
}