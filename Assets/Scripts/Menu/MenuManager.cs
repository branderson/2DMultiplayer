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
        public class PlayerCard
        {
            private GameObject panel;
            private GameObject title;
            private GameObject instruction;
            private Image panelImage;
            private Text titleText;
            private Text instructionText;
            private int number;
            private bool active;
            private bool ready;

            public PlayerCard(GameObject panel, int playerNumber)
            {
                this.number = playerNumber;
                this.panel = panel;
                this.title = panel.transform.Find("TitleText").gameObject;
                this.instruction = panel.transform.Find("Instruction").gameObject;
                this.panelImage = panel.GetComponent<Image>();
                this.titleText = title.GetComponent<Text>();
                this.instructionText = instruction.GetComponent<Text>();
                this.titleText.text = "Player " + playerNumber;
                this.instructionText.text = "Press A";
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
            }

            public void Ready()
            {
                ready = true;
                panelImage.color = Color.green;
                instructionText.text = "Ready!";
            }

            public bool IsReady()
            {
                return ready;
            }
        }

        internal List<MenuInput> Controllers = new List<MenuInput>();
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
                playerCards[i] = new PlayerCard(GameObject.Find("Panel" + (i + 1)), i + 1);
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
                    MenuInput input = gameObject.AddComponent<MenuInput>();
                    input.Init(0);
                    Controllers.Add(input);
                    XIndices.Add(-1);
                    playerCards[Controllers.Count() - 1].Activate();
                    print("Adding keyboard");
                }

                // Allow setting any joystick as a controller
                for (int i = 1; i <= Input.GetJoystickNames().Count(); i++)
                {
                    if (Input.GetButtonDown("PrimaryJ" + i) && !Controllers.Any(input => input.playerNumber == i))
                    {
                        MenuInput input = gameObject.AddComponent<MenuInput>();
                        input.Init(i);
                        Controllers.Add(input);
                        playerCards[Controllers.Count() - 1].Activate();
                        bool xIndexAdded = false;
                        for (int controller = 0; controller < 4; controller++)
                        {
                            if (GamePad.GetState((PlayerIndex) controller).IsConnected)
                            {
                                // TODO: Vibrations can be mapped to the wrong controller if selected on the exact same frame
                                if (GamePad.GetState((PlayerIndex) controller).Buttons.A == ButtonState.Pressed &&
                                    !XIndices.Contains(controller))
                                {
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
                if (Controllers[controller].playerNumber == 0)
                {
                    if (Input.GetButtonDown("StartK"))
                    {
                        playerCards[controller].Ready();
                    }
                }

                else
                {
                    if (Input.GetButtonDown("StartJ" + Controllers[controller].playerNumber))
                    {
                        playerCards[controller].Ready();
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