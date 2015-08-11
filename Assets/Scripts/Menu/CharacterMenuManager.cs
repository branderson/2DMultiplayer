using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;
//using UnityEditor;
using UnityEngine.UI;
using XInputDotNetPure;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Menu
{
    public class CharacterMenuManager : MonoBehaviour
    {
//        internal List<MenuInputController> Controllers = new List<MenuInputController>();
        internal GameManager gameManager;
        internal bool[] Controllers = new bool[4];
        internal MenuPlayerController[] playerControllers = new MenuPlayerController[4];
        internal MenuInputController[] inputControllers = new MenuInputController[4];
        internal PlayerCard[] playerCards = new PlayerCard[4];
        [SerializeField] private GameObject tournamentText;

        private List<int> controllersHolding = new List<int>(); 
        private bool deactivatedThisFrame = false;
        private bool inCharacterMenu = true;

        private void Awake()
        {
            inCharacterMenu = true;
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }

        private void Start()
        {
            if (gameManager.GameConfig.TournamentMode)
            {
                tournamentText.SetActive(true);
            }
            else
            {
                tournamentText.SetActive(false);
            }
            for (int i = 0; i <= 3; i++)
            {
//                Controllers[i] = false;
                playerCards[i] = GameObject.Find("Panel" + (i + 1)).GetComponent<PlayerCard>();
                playerCards[i].Init(gameManager.PlayerConfig[i]);
                GameObject player = GameObject.Find("Player" + (i + 1));
                playerControllers[i] = player.GetComponent<MenuPlayerController>();
                inputControllers[i] = player.GetComponent<MenuInputController>();
                playerControllers[i].GetComponent<MenuPlayerController>().Init(playerCards[i]);
            }

            // Load player settings from gameManager
            for (int i = 0; i <= 3; i++)
            {
                // TODO: What about case where player has been added, then removed (no longer active), and later player added in place (should skip slot) (or should it?)
                if (gameManager.PlayerConfig[i].Active && !gameManager.PlayerConfig[i].Computer && !gameManager.PlayerConfig[i].UseXInput &&
                         gameManager.PlayerConfig[i].Keyboard)
                {
                    ActivateKeyboard();
                }
                else if (gameManager.PlayerConfig[i].Active && !gameManager.PlayerConfig[i].Computer && !gameManager.PlayerConfig[i].UseXInput)
                {
                    ActivateDirectInput(gameManager.PlayerConfig[i].ControllerIndex);
                }
                else if (gameManager.PlayerConfig[i].Active && !gameManager.PlayerConfig[i].Computer && gameManager.PlayerConfig[i].UseXInput)
                {
//                    ActivateDirectInput(gameManager.PlayerConfig[i].ControllerIndex);
                    ActivateXInput(gameManager.PlayerConfig[i].XIndex);
                }
                else if (gameManager.PlayerConfig[i].Active && gameManager.PlayerConfig[i].Computer)
                {
                    ActivateComputer(gameManager.PlayerConfig[i].Slot);
                }
            }
        }

        private void Update()
        {
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (inCharacterMenu)
            {
                // Allow setting keyboard as a controller
                if (!playerCards.Any(card => (card.inputController.Keyboard && card.IsActive())))
                    // Linq expression checks if any object in Controllers has a ControllerNumber == 0
                {
                    if (Input.GetButtonDown("PrimaryK"))
                    {
                        ActivateKeyboard();
                    }
                    if (Input.GetButtonDown("SecondaryK"))
                    {
                        ClearCards();
                        Application.LoadLevel("TitleMenu");
                    }
                }

                bool xInputPressed = false;
                for (int i = 0; i < 4; i++)
                {
                    PlayerIndex controller = (PlayerIndex) i;
                    if (GamePad.GetState(controller).IsConnected)
                    {
                        if (
                            !playerCards.Any(
                                card =>
                                    (card.inputController.UseXInput && card.inputController.XIndex == controller &&
                                     card.IsActive())))
                        {
                            if (GamePad.GetState(controller).Buttons.A == ButtonState.Pressed)
                            {
                                ActivateXInput(controller);
                            }
                            if (!deactivatedThisFrame && !controllersHolding.Contains(i) &&
                                GamePad.GetState(controller).Buttons.B == ButtonState.Pressed)
                            {
                                ClearCards();
                                Application.LoadLevel("TitleMenu");
                            }
                        }
//                        // If 
//                        if (playerCards.Any(card => (card.inputController.UseXInput && card.inputController.XIndex == controller && card.IsActive())))
//                        {
//                        }
                        // Stop listening for DirectInput if xInput pressed
                        if (GamePad.GetState(controller).Buttons.A == ButtonState.Pressed ||
                            GamePad.GetState(controller).Buttons.B == ButtonState.Pressed)
                        {
                            xInputPressed = true;
                            if (!controllersHolding.Contains(i))
                            {
                                controllersHolding.Add(i);
                            }
                        }
                        else
                        {
                            if (controllersHolding.Contains(i))
                            {
                                controllersHolding.Remove(i);
                            }
                        }
                    }
                }

                // Only allow adding DirectInput controllers if no xInput controller is pressing A to avoid control mismatches
                // TODO: Switch from checking for holding A to pressing A possibly (very low importance)
                for (int i = 1; i <= Input.GetJoystickNames().Count(); i++)
                {
                    if (!playerCards.Any(card => (card.inputController.ControllerNumber == i - 1 && card.IsActive())) &&
                        !xInputPressed)
                    {
                        if (Input.GetButtonDown("PrimaryJ" + i))
                        {
                            ActivateDirectInput(i - 1);
                        }
                        if (Input.GetButtonDown(("SecondaryJ" + i)))
                        {
                            ClearCards();
                            Application.LoadLevel("TitleMenu");
                        }
                    }
                }

                deactivatedThisFrame = false;

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
                if (allReady && playerCards.Any(card => (card.IsActive() && !card.inputController.Computer)) &&
                    Controllers.Where(item => item).Count() > 1)
                {
                    Object.DontDestroyOnLoad(this);
                    playerCards = null;
                    inCharacterMenu = false;
                    SaveCards();
                    //                Application.LoadLevel("Level1");
                    Application.LoadLevel("LevelMenu");
                }
            }
        }

        public void ActivateComputer(int slot)
        {
            if (!Controllers.Any(controller => !controller))
            {
                return;
            }
            Controllers[slot-1] = true;
            playerCards[slot-1].ActivateComputer();
        }

        private void ActivateXInput(PlayerIndex xIndex)
        {
            if (!Controllers.Any(controller => !controller))
            {
                return;
            }
            int slot = 3;

            for (int i = 3; i >= 0; i--)
            {
                if (!Controllers[i])
                {
                    slot = i;
                }
            }
//
//            foreach (PlayerCard card in playerCards)
//            {
//                if (card.inputController.XIndex == xIndex && card.inputController.UseXInput)
//                {
//                    slot = card.number - 1;
//                }
//            }

            Controllers[slot] = true;
            playerCards[slot].ActivateXInput();
//            print("Adding XInput");

            inputControllers[slot].UseXInput = true;
            inputControllers[slot].XIndex = xIndex;
        }

        private void ActivateKeyboard()
        {
            if (!Controllers.Any(controller => !controller))
            {
                return;
            }
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
                if (card.inputController.Keyboard)
                {
                    slot = card.number - 1;
                }
            }

            Controllers[slot] = true;
            playerCards[slot].ActivateKeyboard();
            inputControllers[slot].Keyboard = true;
//            print("Adding keyboard");
        }

        private void ActivateDirectInput(int inputIndex)
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
                if (card.inputController.ControllerNumber == inputIndex)
                {
                    slot = card.number - 1;
                }
            }

            Controllers[slot] = true;
            playerCards[slot].ActivateDirectInput();
            inputControllers[slot].ControllerNumber = inputIndex;
//            print("Adding DirectInput");
        }

        public void Deactivate(int number)
        {
            deactivatedThisFrame = true;
            Controllers[number - 1] = false;
            gameManager.PlayerConfig[number - 1].Active = false;
//            gameManager.PlayerConfig[number - 1] = new PlayerConfig();
        }

        private void SaveCards()
        {
            for (int card = 0; card < 4; card++)
            {
                gameManager.PlayerConfig[card].Vibration = inputControllers[card].Vibration;
                gameManager.PlayerConfig[card].TapJump = inputControllers[card].TapJump;
                gameManager.PlayerConfig[card].DPad = inputControllers[card].DPad;
                gameManager.PlayerConfig[card].Computer = inputControllers[card].Computer;
                gameManager.PlayerConfig[card].Active = playerControllers[card].active;
                gameManager.PlayerConfig[card].Slot = card + 1;
                gameManager.PlayerConfig[card].ControllerIndex = inputControllers[card].ControllerNumber;
                gameManager.PlayerConfig[card].XIndex = inputControllers[card].XIndex;
                gameManager.PlayerConfig[card].UseXInput = inputControllers[card].UseXInput;
                gameManager.PlayerConfig[card].Keyboard = inputControllers[card].Keyboard;
                gameManager.PlayerConfig[card].Character = playerControllers[card].CharacterIndex;
            }
        }

        private void ClearCards()
        {
            gameManager.RebuildPlayerConfig();
        }
    }
}