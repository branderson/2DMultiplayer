using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class TitleController : MonoBehaviour
    {
        private GameManager gameManager;
        private Text tournamentText;
        private readonly string[] tournamentCode = {"up", "up", "down", "down", "left", "right", "left", "right", "b", "a"};
        private int inputStringIndex;

        private float threshold = .5f;
        private bool xActive = false;
        private bool yActive = false;

        // Use this for initialization
        private void Start()
        {
            inputStringIndex = 0;
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            tournamentText = transform.Find("TournamentText").GetComponent<Text>();
            if (gameManager.GameConfig.TournamentMode)
            {
                tournamentText.enabled = true;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (inputStringIndex < 10)
            {
                if (Mathf.Abs(Input.GetAxisRaw("GlobalHorizontal")) > threshold)
                {
                    if (!xActive)
                    {
                        if (Input.GetAxisRaw("GlobalHorizontal") < -threshold)
                        {
                            if (tournamentCode[inputStringIndex] == "left")
                            {
                                inputStringIndex++;
                                print("Left");
                            }
                            else
                            {
                                inputStringIndex = 0;
                            }
                        }
                        else if (Input.GetAxisRaw("GlobalHorizontal") > threshold)
                        {
                            if (tournamentCode[inputStringIndex] == "right")
                            {
                                inputStringIndex++;
                                print("Right");
                            }
                            else
                            {
                                inputStringIndex = 0;
                            }
                        }
                        xActive = true;
                    }
                }
                else
                {
                    xActive = false;
                }

                if (Mathf.Abs(Input.GetAxisRaw("GlobalVertical")) > threshold)
                {
                    if (!yActive)
                    {
                        if (Input.GetAxisRaw("GlobalVertical") < -threshold)
                        {
                            if (tournamentCode[inputStringIndex] == "down")
                            {
                                inputStringIndex++;
                                print("Down");
                            }
                            else
                            {
                                inputStringIndex = 0;
                            }
                        }
                        else if (Input.GetAxisRaw("GlobalVertical") > threshold)
                        {
                            if (tournamentCode[inputStringIndex] == "up")
                            {
                                inputStringIndex++;
                                print("Up");
                            }
                            else
                            {
                                inputStringIndex = 0;
                            }
                        }
                        yActive = true;
                    }
                }
                else
                {
                    yActive = false;
                }

                if (Input.GetButtonDown("GlobalPrimary"))
                {
                    if (tournamentCode[inputStringIndex] == "a")
                    {
                        inputStringIndex++;
                        print("A");
                    }
                    else
                    {
                        inputStringIndex = 0;
                    }
                }

                if (Input.GetButtonDown("GlobalSecondary"))
                {
                    if (tournamentCode[inputStringIndex] == "b")
                    {
                        inputStringIndex++;
                        print("B");
                    }
                    else
                    {
                        inputStringIndex = 0;
                    }
                }
            }
            else
            {
                if (!tournamentText.enabled)
                {
                    tournamentText.enabled = true;
                }
            }

            if (Input.GetButtonDown("GlobalStart"))
            {
                if (inputStringIndex == 10)
                {
                    gameManager.GameConfig.TournamentMode = true;
                }
                Application.LoadLevel("MenuScene");
            }
            if (Input.GetButtonDown("GlobalBack"))
            {
                Application.Quit();
            }
        }
    }
}