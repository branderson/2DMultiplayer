using UnityEngine;
using System.Collections;
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
                for (int controller = 0; controller < menuManager.Controllers.Count; controller++)
                {
                    GameObject player = (GameObject)Instantiate(Player, spawnPoints[menuManager.Controllers[controller]].transform.position, Quaternion.identity);
                    player.GetComponent<PlayerControllerInput>().playerNumber = menuManager.Controllers[controller];
                    player.GetComponent<PlayerControllerInput>().XIndex = menuManager.XIndices[controller];
                    player.GetComponent<PlayerController>().SetLayerOrder((int)spawnPoints[controller].transform.position.z);
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