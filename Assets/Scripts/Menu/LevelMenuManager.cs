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
    public class LevelMenuManager : MonoBehaviour
    {
//        internal List<MenuInputController> Controllers = new List<MenuInputController>();
        internal GameManager gameManager;
        internal CharacterMenuManager menuManager;
        internal MenuInputController input;
        internal MenuPlayerController controller;
        [SerializeField] private GameObject tournamentText;
//        private string sceneName = "Level1";

        private void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            menuManager = GameObject.Find("MenuManager").GetComponent<CharacterMenuManager>();
//            int firstPlayerIndex = menuManager.inputControllers.First(inputController => inputController.ControllerNumber >= 0);          
//            input = menuManager.inputControllers[0];
            input = menuManager.inputControllers.First(inputController => !inputController.Computer && 
                menuManager.playerControllers[Array.IndexOf(menuManager.inputControllers, inputController)].active);
//            print(Array.IndexOf(menuManager.inputControllers, input));
            controller = menuManager.playerControllers[Array.IndexOf(menuManager.inputControllers, input)]; //menuManager.playerControllers[0];
            controller.SetSelected(GameObject.Find("Level1").GetComponent<MenuSelectable>());

            for (int card = 0; card < 4; card++)
            {
                gameManager.PlayerConfig[card].Vibration = menuManager.inputControllers[card].Vibration;
                gameManager.PlayerConfig[card].TapJump = menuManager.inputControllers[card].TapJump;
                gameManager.PlayerConfig[card].DPad = menuManager.inputControllers[card].DPad;
                gameManager.PlayerConfig[card].Computer = menuManager.inputControllers[card].Computer;
                gameManager.PlayerConfig[card].Active = menuManager.playerControllers[card].active;
                gameManager.PlayerConfig[card].Slot = card + 1;
                gameManager.PlayerConfig[card].ControllerIndex =
                    menuManager.inputControllers[card].ControllerNumber;
                gameManager.PlayerConfig[card].XIndex = menuManager.inputControllers[card].XIndex;
                gameManager.PlayerConfig[card].UseXInput = menuManager.inputControllers[card].UseXInput;
                gameManager.PlayerConfig[card].Keyboard = menuManager.inputControllers[card].Keyboard;
                gameManager.PlayerConfig[card].Character = menuManager.playerControllers[card].CharacterIndex;
            }
        }

        // Use this for initialization
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
//            LoadScene("Level1");
        }

        private void Update()
        {
            // If no controllers active, next back goes to title
            if (Input.GetButtonDown("GlobalBack"))
            {
                Destroy(menuManager.gameObject);
                Application.LoadLevel("CharacterMenu");
            }
        }

        // Update is called once per frame
        private void LateUpdate()
        {
        }

        public void LoadScene(string scene)
        {
            print("Setting scene to " + scene);
            Application.LoadLevel(scene);
        }
    }
}