using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Managers
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private Transform[] maxCameraBounds;
        private List<Transform> players;
        private LevelManager levelManager;
        private Camera camera;
        private float maxX = 0f;
        private float minX = 0f;
        private float maxY = 0f;
        private float minY = 0f;
        private float minOrthoSize = 5f;

        private void Awake()
        {
            levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            camera = GetComponent<Camera>();
            players = new List<Transform>();
        }

        private void Start()
        {
            maxX = maxCameraBounds.OrderByDescending(item => item.position.x).First().position.x;
            minX = maxCameraBounds.OrderByDescending(item => item.position.x).Last().position.x;
            maxY = maxCameraBounds.OrderByDescending(item => item.position.y).First().position.y;
            minY = maxCameraBounds.OrderByDescending(item => item.position.y).Last().position.y;
            print(maxX + " " + minX + " " + maxY + " " + minY);
            List<GameObject> playerObjects = levelManager.GetPlayers();
            foreach (GameObject playerObject in playerObjects)
            {
                players.Add(playerObject.transform);
                print("Adding player");
            }
        }

        private void LateUpdate()
        {
            SetCameraPosition();
            SetCameraBounds();
        }

        //Checks the positioning for each player
        private void SetCameraPosition()
        {
            Vector3 averagePosition = AveragePosition(players);
            transform.position = Vector3.Lerp(transform.position, averagePosition, .1f);
//            print("X: " + camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x + " " + maxX);
//            print("Y: " + camera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y + " " + maxY);
//            print(transform.position.x + " " + averagePosition.x);
            // TODO: Weird nudge backward when coming back from max
//            if (averagePosition.x - transform.position.x + camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x < maxX &&
//                averagePosition.x - transform.position.x + camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x > minX)
//            {
//            transform.position = Vector3.Lerp(transform.position, new Vector3(averagePosition.x, transform.position.y, transform.position.z), .1f);
                //                transform.position = Vector3.MoveTowards(transform.position, new Vector3(AveragePosition(players).x, transform.position.y, transform.position.z), 5*Time.fixedDeltaTime);
//            }
//            else if (averagePosition.x > transform.position.x && averagePosition.x > 0)
//            {
////                transform.position = new Vector3(maxX - camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x - transform.position.x, transform.position.y, transform.position.z); //Vector3.Lerp(transform.position, new Vector3(maxX, transform.position.y, transform.position.z), .1f));
//                transform.position = Vector3.Lerp(transform.position, new Vector3(maxX, transform.position.y, transform.position.z), .1f);
//            }
//            else if (averagePosition.x < transform.position.x && averagePosition.x < 0)
//            {
////                transform.position = new Vector3(minX - camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x - transform.position.x, transform.position.y, transform.position.z); // Vector3.Lerp(transform.position, new Vector3(minX, transform.position.y, transform.position.z), .1f));
//                transform.position = Vector3.Lerp(transform.position, new Vector3(minX, transform.position.y, transform.position.z), .1f);
//            }
//            if (averagePosition.y - transform.position.y + camera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y < maxY &&
//                averagePosition.y - transform.position.y + camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y > minY)
//            {
//            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, averagePosition.y, transform.position.z), .1f);
//                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, AveragePosition(players).y, transform.position.z), 5*Time.fixedDeltaTime);
//            }
//            else if (averagePosition.y > transform.position.y && averagePosition.y > 0)
//            {
////                transform.position = new Vector3(transform.position.x, maxY - camera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y - transform.position.y, transform.position.z); //Vector3.Lerp(transform.position, new Vector3(transform.position.x, maxY, transform.position.z), .1f));
//                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, maxY, transform.position.z), .1f);
//            }
//            else if (averagePosition.y < transform.position.y && averagePosition.y < 0)
//            {
////                transform.position = new Vector3(transform.position.x, minY - camera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y - transform.position.y, transform.position.z); //Vector3.Lerp(transform.position, new Vector3(transform.position.x, minY, transform.position.z), .1f));
//                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, minY, transform.position.z), .1f);
//            }
        }

        private void SetCameraBounds()
        {
            Vector2 maxDistance;
//            transform.position = Vector3.MoveTowards(camera.transform.position, AveragePosition(players), 5*Time.fixedDeltaTime);
            maxDistance.x = players.Max(item => (item.position.x - transform.position.x)); // - transform.position.x;
            maxDistance.y = players.Max(item => (item.position.y - transform.position.y)); // - transform.position.y;
            print(maxDistance.x + " " + maxDistance.y);

            if (maxDistance.x/camera.aspect > maxDistance.y && maxDistance.x/camera.aspect + 2 > minOrthoSize)
            {
                if (camera.orthographicSize < maxDistance.x/camera.aspect)
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxDistance.x/camera.aspect + 2, .1f); // Time.fixedDeltaTime);
                }
                else
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxDistance.x/camera.aspect + 2, .1f); // Time.fixedDeltaTime);
                }
            }
            else if (maxDistance.y + 1 > minOrthoSize)
            {
                if (camera.orthographicSize < maxDistance.y)
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxDistance.y + 2, .1f);//Time.fixedDeltaTime);
                }
                else
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxDistance.y + 2, .1f);//Time.fixedDeltaTime);
                }
            }
        }

        private Vector3 AveragePosition(List<Transform> playerList)
        {
            Vector3 position;
            position.x = playerList.Sum(item => item.position.x)/playerList.Count();
            position.y = playerList.Sum(item => item.position.y)/playerList.Count();
            position.z = transform.position.z;
//            print(averageX + " " + averageY);
            return position;
        }
    }

}