using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Menu;
using Assets.Scripts.Player;
using XInputDotNetPure;

namespace Assets.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        // TODO: Once there is more than one character, need separate fields for every player
        [SerializeField] public GameObject Player; // Prefab to use for instantiating player
        private GameManager gameManager;
        private CharacterMenuManager characterMenuManager; // Reference to menu manager
        // TODO: Can add field for order of spawn points
        private GameObject[] spawnPoints; // Array of spawn points for players
        private List<GameObject> players = new List<GameObject>(); 

        void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            characterMenuManager = GameObject.Find("MenuManager").GetComponent<CharacterMenuManager>();
        }

        void Start()
        {
            spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

            // Assign controllers to players and instantiate players at spawn locations
            for (int card = 0; card < 4; card++)
            {
                // Instantiate human players
                if (characterMenuManager.Controllers[card])
                {
                    GameObject player = (GameObject)Instantiate(Player, spawnPoints[characterMenuManager.playerCards[card].PlayerController.playerNumber - 1].transform.position, Quaternion.identity);
                    if (!characterMenuManager.playerCards[card].computer)
                    {
                        print("Instantiating player " + characterMenuManager.playerCards[card].PlayerController.playerNumber);
                        player.transform.FindChild("Rigidbody").gameObject.AddComponent<PlayerInputController>();
                        player.GetComponentInChildren<PlayerInputController>().Init(characterMenuManager.playerCards[card].InputController);
                    }
                    else
                    {
                        print("Instantiating computer " + characterMenuManager.playerCards[card].PlayerController.playerNumber);
                        player.transform.FindChild("Rigidbody").gameObject.AddComponent<AIInputController>();
                        player.GetComponentInChildren<AIInputController>().Init(characterMenuManager.playerCards[card].InputController);
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
                            player.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
                            break;
                        case 3:
                            player.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                            break;
                    }
                    player.GetComponentInChildren<PlayerController>().Init((int)spawnPoints[card].transform.position.z, card); // TODO: Eventually pass menuPlayerController in
                    players.Add(player);
                }
                // Save playerCard states to GameManager
                // TODO: I broke toggle boxes and also playerCards aren't being loaded in the menu from settings properly
                gameManager.PlayerConfig[card].Vibration = characterMenuManager.playerCards[card].InputController.Vibration;
                gameManager.PlayerConfig[card].TapJump = characterMenuManager.playerCards[card].InputController.TapJump;
                gameManager.PlayerConfig[card].DPad = characterMenuManager.playerCards[card].InputController.DPad;
                gameManager.PlayerConfig[card].Computer = characterMenuManager.playerCards[card].computer;
                gameManager.PlayerConfig[card].Active = characterMenuManager.playerCards[card].IsActive();
                gameManager.PlayerConfig[card].Slot = card + 1;
                gameManager.PlayerConfig[card].ControllerIndex = characterMenuManager.playerCards[card].InputController.ControllerNumber;
                gameManager.PlayerConfig[card].XIndex = characterMenuManager.playerCards[card].InputController.XIndex;
                gameManager.PlayerConfig[card].UseXIndex = characterMenuManager.playerCards[card].InputController.UseXIndex;
            }
            foreach (GameObject player in players)
            {
                player.GetComponentInChildren<PlayerController>().FindPlayers(players);
            }
            
            // Get rid of menu manager so it's not still waiting for inputs
            Destroy(characterMenuManager.gameObject);
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
            player.Respawn(spawnPoints[player.playerNumber - 1].transform.position);
        }

        public List<GameObject> GetPlayers()
        {
            return players;
        }
    }
}