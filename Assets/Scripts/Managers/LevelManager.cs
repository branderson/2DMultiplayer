using UnityEngine;
using System.Collections;
using Assets.Scripts.Menu;
using Assets.Scripts.Player;
using XInputDotNetPure;

namespace Assets.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private bool useMenu = true; // Whether to assign players based on menu selections
        // TODO: Once there is more than one character, need separate fields for every player
        [SerializeField] public GameObject Player; // Prefab to use for instantiating player
        private MenuManager menuManager; // Reference to menu manager
        // TODO: Can add field for order of spawn points
        private GameObject[] spawnPoints; // Array of spawn points for players

        void Awake()
        {
            if (useMenu)
            {
                menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
            }
        }

        void Start()
        {
            spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

            if (useMenu) {
                // Assign controllers to players and instantiate players at spawn locations
                for (int card = 0; card < 4; card++)
                {
                    if (!menuManager.playerCards[card].computer && menuManager.Controllers[card])
                    {
                        print(menuManager.playerCards[card].playerController.playerNumber);
                        GameObject player = (GameObject)Instantiate(Player, spawnPoints[menuManager.playerCards[card].playerController.playerNumber - 1].transform.position, Quaternion.identity);
                        // TODO: Put all of this stuff into Init() for PlayerControllerInput
                        player.GetComponent<PlayerControllerInput>().Init(menuManager.playerCards[card].controllerInput);
                        player.GetComponent<PlayerController>().Init((int)spawnPoints[card].transform.position.z); // TODO: Eventually pass menuPlayerController in
                    }
                    if (menuManager.playerCards[card].computer && menuManager.Controllers[card])
                    {
                        // TODO: Computer player instantiation here
                    }
                }
            }
            
            // Get rid of menu manager so it's not still waiting for inputs
            Destroy(menuManager);
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