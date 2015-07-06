using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XInputDotNetPure;

namespace Assets.Scripts.Managers
{
    public class MenuManager : MonoBehaviour
    {
        internal List<int> Controllers = new List<int>();
        internal List<int> XIndices = new List<int>();
//        private GameObject canvas;
        private Text[] playerInstructions = new Text[4];
        private bool[] ready = new bool[4];

        private void Awake()
        {
            
        }

        // Use this for initialization
        private void Start()
        {
            Object.DontDestroyOnLoad(this);
//            canvas = GameObject.Find("Canvas");
            for (int i = 0; i <= 3; i++)
            {
                playerInstructions[i] = GameObject.Find("Instruction" + (i + 1)).GetComponent<Text>();
                playerInstructions[i].text = "Press A";
            }
        }

        // Update is called once per frame
        private void Update()
        {
            // Allow no more than 4 controllers
            if (Controllers.Count() < 4)
            {
                // Allow setting keyboard as a controller
                if (Input.GetButtonDown("PrimaryK") && !Controllers.Contains(0))
                {
                    Controllers.Add(0);
                    XIndices.Add(-1);
                    playerInstructions[Controllers.Count() - 1].text = "Press Start\nWhen Ready";
                    print("Adding keyboard");
                }

                // Allow setting any joystick as a controller
                for (int i = 1; i <= Input.GetJoystickNames().Count(); i++)
                {
                    if (Input.GetButtonDown("PrimaryJ" + i) && !Controllers.Contains(i))
                    {
                        Controllers.Add(i);
                        playerInstructions[Controllers.Count() - 1].text = "Press Start\nWhen Ready";
                        bool xIndexAdded = false;
                        for (int controller = 0; controller < 4; controller++)
                        {
                            if (GamePad.GetState((PlayerIndex) controller).IsConnected)
                            {
                                // TODO: Vibrations can be mapped to the wrong controller if selected on the exact same frame
//                                print("Controller " + (PlayerIndex) controller + " is connected!");
                                if (GamePad.GetState((PlayerIndex) controller).Buttons.A == ButtonState.Pressed && !XIndices.Contains(controller))
                                {
//                                    GamePad.SetVibration((PlayerIndex)controller, 0.8f, 0.8f);
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

            // Wait for start input on all added controllers
            for (int controller = 0; controller < Controllers.Count(); controller++)
            {
                if (Controllers[controller] == 0)
                {
                    if (Input.GetButtonDown("StartK"))
                    {
                        ready[controller] = true;
                        playerInstructions[controller].text = "Ready!";
                    }
                }

                else
                {
                    if (Input.GetButtonDown("StartJ" + Controllers[controller]))
                    {
                        ready[controller] = true;
                        playerInstructions[controller].text = "Ready!";
                    } 
                }
            }

            bool allReady = true;

            // Check if all added controllers have pressed start
            for (int controller = 0; controller < Controllers.Count(); controller++)
            {
                if (ready[controller] != true)
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

//        private void LateUpdate()
//        {
//            for (int i = 0; i < 4; i++)
//            {
//                if (GamePad.GetState((PlayerIndex) i).IsConnected)
//                {
//                    GamePad.SetVibration((PlayerIndex) i, 0f, 0f);
//                }
//            }
//        }
    }
}