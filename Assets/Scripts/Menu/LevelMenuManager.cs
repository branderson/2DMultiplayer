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
    public class LevelMenuManager : MonoBehaviour
    {
//        internal List<MenuInputController> Controllers = new List<MenuInputController>();
        internal GameManager gameManager;
        internal CharacterMenuManager menuManager;
//        internal PlayerCard[] playerCards;
        private Text tournamentText;
        private string sceneName = "Level1";

        private void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            menuManager = GameObject.Find("MenuManager").GetComponent<CharacterMenuManager>();
//            playerCards = menuManager.playerCards;
//            playerCards[0].PlayerController.SetSelected(GameObject.Find("Level1").GetComponent<MenuSelectable>());
            tournamentText = GameObject.Find("TournamentText").GetComponent<Text>();
        }

        // Use this for initialization
        private void Start()
        {
            if (gameManager.GameConfig.TournamentMode)
            {
                tournamentText.enabled = true;
            }
            else
            {
                tournamentText.enabled = false;
            }
        }

        private void Update()
        {
            // If no controllers active, next back goes to title
            if (Input.GetButtonDown("GlobalBack"))
            {
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