using UnityEngine;
using System.Collections;
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
        private MenuManager menuManager; // Reference to menu manager
        // TODO: Can add field for order of spawn points
        private GameObject[] spawnPoints; // Array of spawn points for players

        void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        }

        void Start()
        {
            spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

            // Assign controllers to players and instantiate players at spawn locations
            for (int card = 0; card < 4; card++)
            {
                // Instantiate human players
                if (!menuManager.playerCards[card].computer && menuManager.Controllers[card])
                {
                    print("Instantiating player " + menuManager.playerCards[card].playerController.playerNumber);
                    GameObject player = (GameObject)Instantiate(Player, spawnPoints[menuManager.playerCards[card].playerController.playerNumber - 1].transform.position, Quaternion.identity);
                    player.AddComponent<PlayerInputController>();
                    player.GetComponent<PlayerInputController>().Init(menuManager.playerCards[card].InputController);
                    player.GetComponent<PlayerController>().Init((int)spawnPoints[card].transform.position.z); // TODO: Eventually pass menuPlayerController in
                }
                // Instantiate AI players
                if (menuManager.playerCards[card].computer && menuManager.Controllers[card])
                {
                    // TODO: Computer player instantiation here
                    print("Instantiating computer " + menuManager.playerCards[card].playerController.playerNumber);
                    GameObject player = (GameObject)Instantiate(Player, spawnPoints[menuManager.playerCards[card].playerController.playerNumber - 1].transform.position, Quaternion.identity);
                    player.AddComponent<AIInputController>();
                    player.GetComponent<AIInputController>().Init(menuManager.playerCards[card].InputController);
                    player.GetComponent<PlayerController>().Init((int)spawnPoints[card].transform.position.z); // TODO: Eventually pass menuPlayerController in
                }
                // Save playerCard states to GameManager
                // TODO: I broke toggle boxes and also playerCards aren't being loaded in the menu from settings properly
                gameManager.PlayerConfig[card].Vibration = menuManager.playerCards[card].InputController.Vibration;
                gameManager.PlayerConfig[card].TapJump = menuManager.playerCards[card].InputController.TapJump;
                gameManager.PlayerConfig[card].Computer = menuManager.playerCards[card].computer;
                gameManager.PlayerConfig[card].Active = menuManager.playerCards[card].IsActive();
                gameManager.PlayerConfig[card].Slot = card + 1;
                gameManager.PlayerConfig[card].ControllerIndex = menuManager.playerCards[card].InputController.ControllerNumber;
                gameManager.PlayerConfig[card].XIndex = menuManager.playerCards[card].InputController.XIndex;
            }
            
            // Get rid of menu manager so it's not still waiting for inputs
            Destroy(menuManager.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Restart"))
            {
                Application.LoadLevel("MenuScene");
            }
        }
    }
}