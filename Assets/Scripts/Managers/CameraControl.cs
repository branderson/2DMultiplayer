using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Managers
{
    public class CameraControl : MonoBehaviour
    {
        private List<Transform> players;
        private LevelManager levelManager;
        private Camera camera;

        private void Awake()
        {
            levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>(); //Pointer to LevelManager script
            camera = GetComponent<Camera>();//Pointer to camera
        }

        // Use this for initialization
        private void Start()
        {
            List<GameObject> playerObjects = levelManager.GetPlayers(); //List of player objects
            foreach (GameObject playerObject in playerObjects)
            {
                players.Add(playerObject.transform); //Adds player transformations to the list players, contains position, scale, and rotation
            }
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void LateUpdate()
        {
            CameraPosition();
        }

        //Checks the positioning for each player
        private void CameraPosition()
        {
            //List of player objects
            List<GameObject> playerObjects = levelManager.GetPlayers();

            //Max/min x and y positions
            float minX = 20;
            float minY = 20;
            float maxX = -20;
            float maxY = -20;

            Vector3 cameraFocus = Vector3.zero;

            /*foreach (GameObject playerObject in playerObjects)
            {
                Vector2 tempPlayer = playerObject.transform.position;

                //X bounds
                if (tempPlayer.x < minX)
                    minX = tempPlayer.x;

                if (tempPlayer.x > maxX)
                    maxX = tempPlayer.x;

                //Y bounds
                if (tempPlayer.y < minY)
                    minY = tempPlayer.y;

                if (tempPlayer.y > maxY)
                    maxY = tempPlayer.y;
            }*/

            foreach (GameObject playerObject in playerObjects)
                cameraFocus += playerObject.transform.position;

            cameraFocus = cameraFocus/playerObjects.Count;

            float sizeX = maxX - minX;
            float sizeY = maxY - minY;

            float cameraSize = (sizeX > sizeY ? sizeX : sizeY);

            camera.orthographicSize = cameraSize*0.5f;
        }
    }

}