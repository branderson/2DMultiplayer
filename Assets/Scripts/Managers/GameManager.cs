using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Player;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Managers
{
    // Don't ever delete this
    public class GameManager : MonoBehaviour
    {
        public PlayerConfig[] PlayerConfig = new PlayerConfig[4];

        void Awake()
        {
            if (GameObject.FindGameObjectsWithTag("GameManager").Any(manager => manager != gameObject))
            {
                Destroy(gameObject);
            }
            Object.DontDestroyOnLoad(this);

            for (int i = 0; i < 4; i++)
            {
                PlayerConfig[i] = new PlayerConfig();
            }

            // Set up PlayerConfig
            foreach (PlayerConfig settings in PlayerConfig)
            {
                settings.Vibration = true;
                settings.TapJump = true;
                settings.Slot = Array.IndexOf(PlayerConfig, settings) + 1;
                settings.Computer = false;
                settings.ControllerIndex = -1;
                settings.XIndex = -1;
                settings.Active = false;
            }
        }

        void Start()
        {
            Application.LoadLevel("TitleMenu");
        }

        void Update()
        {

        }
    }
}