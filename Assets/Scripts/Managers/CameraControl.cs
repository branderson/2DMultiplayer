﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Managers
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private float minimumSize = 8f;
        [SerializeField] private float maximumSize = 20f;
        [SerializeField] private Transform[] maxCameraBounds;
        [SerializeField] private float translationRate = .1f;
        [SerializeField] private float resizeRate = .1f;
        private List<Transform> players;
        private LevelManager levelManager;
        private Camera camera;
        private float cameraPadding = 4f;
        private float verticalPadding = 3f;
        private float maxX = 0f;
        private float minX = 0f;
        private float maxY = 0f;
        private float minY = 0f;

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
//            print(maxX + " " + minX + " " + maxY + " " + minY);
            List<GameObject> playerObjects = levelManager.GetPlayers();
            foreach (GameObject playerObject in playerObjects)
            {
                players.Add(playerObject.transform);
//                print("Adding player");
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
            Vector3 averagePosition = MiddlePosition(players);
            transform.position = Vector3.Lerp(transform.position, averagePosition, translationRate);
//            print("X: " + camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x + " " + maxX);
//            print("Y: " + camera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y + " " + maxY);
//            print(transform.position.x + " " + averagePosition.x);
            // TODO: Weird nudge backward when coming back from max
//            if (averagePosition.x - transform.position.x + camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x < maxX &&
//                averagePosition.x - transform.position.x + camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x > minX)
//            {
//            transform.position = Vector3.Lerp(transform.position, new Vector3(averagePosition.x, transform.position.y, transform.position.z), .1f);
                //                transform.position = Vector3.MoveTowards(transform.position, new Vector3(MiddlePosition(players).x, transform.position.y, transform.position.z), 5*Time.fixedDeltaTime);
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
//                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, MiddlePosition(players).y, transform.position.z), 5*Time.fixedDeltaTime);
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
//            transform.position = Vector3.MoveTowards(camera.transform.position, MiddlePosition(players), 5*Time.fixedDeltaTime);
            List<Transform> inBoundsX = players.Where(item => (item.position.x < maxX && item.position.x > minX)).ToList();
            List<Transform> inBoundsY = players.Where(item => (item.position.y < maxY && item.position.y > minY)).ToList();
            if (inBoundsX.Any())
            {
                maxDistance.x = Mathf.Abs(inBoundsX.Max(item => (item.position.x - transform.position.x))); // - transform.position.x;
            }
            else
            {
                maxDistance.x = 0;
            }
            if (inBoundsY.Any())
            {
                maxDistance.y = Mathf.Abs(inBoundsY.Max(item => (item.position.y - transform.position.y))); // - transform.position.y;
            }
            else
            {
                maxDistance.y = 0;
            }
//            print(maxDistance.x + " " + maxDistance.y);
            // Don't resize if all players out of bounds
            if (maxDistance.x < 0 || maxDistance.y < 0)
            {
                return;
            }

            if (maxDistance.x/camera.aspect > maxDistance.y + verticalPadding && maxDistance.x/camera.aspect + cameraPadding > minimumSize)
            {
                if (maxDistance.x/camera.aspect + cameraPadding > maximumSize)
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maximumSize, resizeRate*1.5f); // Time.fixedDeltaTime);
                }
                else if (camera.orthographicSize < maxDistance.x/camera.aspect + cameraPadding)
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxDistance.x/camera.aspect + cameraPadding, resizeRate*1.5f); // Time.fixedDeltaTime);
                }
                else
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxDistance.x/camera.aspect + cameraPadding, resizeRate); // Time.fixedDeltaTime);
                }
            }
            else if (maxDistance.y + cameraPadding + verticalPadding > minimumSize)
            {
                if (maxDistance.y + cameraPadding + verticalPadding > maximumSize)
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maximumSize, resizeRate*1.5f);
                }
                else if (camera.orthographicSize < maxDistance.y + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding + cameraPadding + verticalPadding)
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxDistance.y + cameraPadding + verticalPadding, resizeRate*1.5f);
                        //Time.fixedDeltaTime);
                }
                else
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxDistance.y + cameraPadding + verticalPadding, resizeRate);
                        //Time.fixedDeltaTime);
                }
            }
            else
            {
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, minimumSize, resizeRate);
            }
        }

        private Vector3 MiddlePosition(List<Transform> playerList)
        {
            Vector3 position;
            List<Transform> inBoundsX = playerList.Where(item => (item.position.x < maxX && item.position.x > minX)).ToList();
            List<Transform> inBoundsY = playerList.Where(item => (item.position.y < maxY && item.position.y > minY)).ToList();
//            print("In bounds X: " + inBoundsX.Count() + " In bounds Y: " + inBoundsY.Count());
            if (inBoundsX.Any())
            {
                position.x = (inBoundsX.Max(item => item.position.x) + inBoundsX.Min(item => item.position.x))/2;
            }
            else
            {
                position.x = 0;
            }
            if (inBoundsY.Any())
            {
                position.y = (inBoundsY.Max(item => item.position.y) + inBoundsY.Min(item => item.position.y))/2;
            }
            else
            {
                position.y = 0;
            }
            position.z = transform.position.z;
//            print("Average x: " + playerList[0].position.x + "+" + playerList[1].position.x + "\\2=" + position.x + " " +
//                  ((playerList[0].position.x + playerList[1].position.x)/2) +
//                  "\nAverage y: " + playerList[0].position.y + "+" + playerList[1].position.y + "\\2=" + position.y +
//                  " " + ((playerList[0].position.y + playerList[1].position.y)/2));
//            print(averageX + " " + averageY);
            return position;
        }
    }

}