﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menu;
using Assets.Scripts.Player;
using XInputDotNetPure;

namespace Assets.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        // TODO: Once there is more than one character, need separate fields for every player
        [SerializeField] public GameObject[] Players; // Prefab to use for instantiating player
        [SerializeField] private GameObject[] spawnPoints; // Array of spawn points for players
        [SerializeField] private GameObject[] playerUI;
        private GameManager gameManager;
        private CharacterMenuManager characterMenuManager; // Reference to menu manager
        // TODO: Can add field for order of spawn points
        internal List<GameObject> players = new List<GameObject>();
        private List<KeyValuePair<PlayerController, Vector3>> assignedSpawns; 

        void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            characterMenuManager = GameObject.Find("MenuManager").GetComponent<CharacterMenuManager>();
            assignedSpawns = new List<KeyValuePair<PlayerController, Vector3>>();

            // Assign controllers to players and instantiate players at spawn locations
            for (int card = 0; card < 4; card++)
            {
                // Instantiate human players
                if (characterMenuManager.Controllers[card])
                {
                    GameObject player = (GameObject)Instantiate(Players[characterMenuManager.playerControllers[card].CharacterIndex], spawnPoints[characterMenuManager.playerControllers[card].PlayerNumber - 1].transform.position, Quaternion.identity);
                    if (!characterMenuManager.inputControllers[card].Computer &&
                        !characterMenuManager.inputControllers[card].UseXInput &&
                        characterMenuManager.inputControllers[card].Keyboard)
                    {
//                        print("Instantiating keyboard player " + characterMenuManager.playerControllers[card].PlayerNumber);
                        player.transform.FindChild("Rigidbody").gameObject.AddComponent<PlayerKeyboardInputController>();
                        player.GetComponentInChildren<PlayerKeyboardInputController>().Init(characterMenuManager.inputControllers[card]);
                    }
                    else if (!characterMenuManager.inputControllers[card].Computer && !characterMenuManager.inputControllers[card].UseXInput)
                    {
//                        print("Instantiating DirectInput player " + characterMenuManager.playerControllers[card].PlayerNumber);
                        player.transform.FindChild("Rigidbody").gameObject.AddComponent<PlayerDirectInputController>();
                        player.GetComponentInChildren<PlayerDirectInputController>().Init(characterMenuManager.inputControllers[card]);
                    }
                    else if (!characterMenuManager.inputControllers[card].Computer)
                    {
//                        print("Instantiating XInput player " + characterMenuManager.playerControllers[card].PlayerNumber);
                        player.transform.FindChild("Rigidbody").gameObject.AddComponent<PlayerXInputController>();
                        player.GetComponentInChildren<PlayerXInputController>().Init(characterMenuManager.inputControllers[card]);
                    }
                    else
                    {
//                        print("Instantiating computer " + characterMenuManager.playerControllers[card].PlayerNumber);
                        player.transform.FindChild("Rigidbody").gameObject.AddComponent<AIInputController>();
                        player.GetComponentInChildren<AIInputController>().Init(characterMenuManager.inputControllers[card]);
                    }
                    switch (card)
                    {
                        case 0:
                            if (gameManager.GameConfig.TournamentMode)
                            {
                                player.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                            }
                            else
                            {
                                player.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                            }
                            break;
                        case 1:
                            player.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                            break;
                        case 2:
                            player.GetComponentInChildren<SpriteRenderer>().color = Color.magenta;
                            break;
                        case 3:
                            player.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                            break;
                    }

                    player.GetComponentInChildren<PlayerController>().Init((int)spawnPoints[card].transform.position.z, card, characterMenuManager.playerControllers[card].GetCharacterName(), characterMenuManager.inputControllers[card].Computer); // TODO: Eventually pass menuPlayerController in
                    players.Add(player);
                }
            }
            for (int i = 0; i < players.Count(); i++)
            {
                if (players.Count() == 1)
                {
                    players[i].GetComponentInChildren<PlayerController>().InitUI(playerUI[4].GetComponent<PlayerUI>());
                    assignedSpawns.Add(new KeyValuePair<PlayerController, Vector3>(players[i].GetComponentInChildren<PlayerController>(), spawnPoints[0].transform.position));
                }
                else if (players.Count() == 2)
                {
                    players[i].GetComponentInChildren<PlayerController>().InitUI(playerUI[i + 1].GetComponent<PlayerUI>());
                    assignedSpawns.Add(new KeyValuePair<PlayerController, Vector3>(players[i].GetComponentInChildren<PlayerController>(), spawnPoints[i].transform.position));
//                    switch (i)
//                    {
//                        case 0:
//                            players[i].GetComponentInChildren<PlayerController>().InitUI(playerUI[1].GetComponent<PlayerUI>());
//                            assignedSpawns.Add(new KeyValuePair<PlayerController, Vector3>(players[i].GetComponentInChildren<PlayerController>(), spawnPoints[0].transform.position));
//                            break;
//                        case 1:
//                            players[i].GetComponentInChildren<PlayerController>().InitUI(playerUI[2].GetComponent<PlayerUI>());
//                            assignedSpawns.Add(new KeyValuePair<PlayerController, Vector3>(players[i].GetComponentInChildren<PlayerController>(), spawnPoints[1].transform.position));
//                            break;
//                    }
                }
                else if (players.Count() == 3)
                {
                    switch (i)
                    {
                        case 0:
                            players[i].GetComponentInChildren<PlayerController>().InitUI(playerUI[i].GetComponent<PlayerUI>());
                            assignedSpawns.Add(new KeyValuePair<PlayerController, Vector3>(players[i].GetComponentInChildren<PlayerController>(), spawnPoints[0].transform.position));
                            break;
                        case 1:
                            players[i].GetComponentInChildren<PlayerController>().InitUI(playerUI[4].GetComponent<PlayerUI>());
                            players[i].transform.position = spawnPoints[4].transform.position;
                            assignedSpawns.Add(new KeyValuePair<PlayerController, Vector3>(players[i].GetComponentInChildren<PlayerController>(), spawnPoints[4].transform.position));
                            break;
                        case 2:
                            players[i].GetComponentInChildren<PlayerController>().InitUI(playerUI[3].GetComponent<PlayerUI>());
                            players[i].transform.position = spawnPoints[1].transform.position;
                            assignedSpawns.Add(new KeyValuePair<PlayerController, Vector3>(players[i].GetComponentInChildren<PlayerController>(), spawnPoints[1].transform.position));
                            break;
                    }
                }
                else
                {
                    players[i].GetComponentInChildren<PlayerController>().InitUI(playerUI[i].GetComponent<PlayerUI>());
                    assignedSpawns.Add(new KeyValuePair<PlayerController, Vector3>(players[i].GetComponentInChildren<PlayerController>(), spawnPoints[i].transform.position));
                }
            }
            foreach (GameObject player in players)
            {
                player.GetComponentInChildren<PlayerController>().FindPlayers(players);
            }
            
            // Get rid of menu manager so it's not still waiting for inputs
            Destroy(characterMenuManager.gameObject);
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Restart"))
            {
                Application.LoadLevel("CharacterMenu");
            }
        }

        public void Respawn(PlayerController player)
        {
            player.Respawn(assignedSpawns[player.playerNumber - 1].Value);
        }

        public List<GameObject> GetPlayers()
        {
            return players;
        }
    }
}