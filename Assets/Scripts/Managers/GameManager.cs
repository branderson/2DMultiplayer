using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using XInputDotNetPure;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Managers
{
    // Don't ever delete this
    public class GameManager : MonoBehaviour
    {
        public PlayerConfig[] PlayerConfig = new PlayerConfig[4];
        public GameConfig GameConfig = new GameConfig();
        public GameSaveData SaveData;
        private readonly BinaryFormatter bf = new BinaryFormatter();

        private void Awake()
        {
            // Delete self if GameManager already exists
            if (GameObject.FindGameObjectsWithTag("GameManager").Any(manager => manager != gameObject))
            {
                Destroy(gameObject);
            }
            // Otherwise keep this alive
            Object.DontDestroyOnLoad(this);

            // Load in game save data here

            // Create temporary controller slot settings
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
                settings.XIndex = PlayerIndex.One;
                settings.Active = false;
            }
        }

        public void SaveGhostAIData(BinaryTree<CaseBase> cases, string characterName)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/SaveData/AIData/");
            FileStream file = File.Create(Application.persistentDataPath + "/SaveData/AIData/" + characterName + ".ai");
            List<CaseBase> caseList = cases.InOrder().Select(item => item.Data).Where(item => !item.Empty() && item.ResponseStateList.Any(response => response.Effectiveness > 0)).ToList();
            print("There are " + caseList.Count + " cases in the tree");
            bf.Serialize(file, caseList);
//            bf.Serialize(file, cases);
            file.Close();
        }

        public KeyValuePair<string, BinaryTree<CaseBase>> LoadGhostAIData(string characterName)
        {
            FileStream file;
            try
            {
                file = File.OpenRead(Application.persistentDataPath + "/SaveData/AIData/" + characterName + ".ai");
            }
            catch (FileNotFoundException)
            {
                return new KeyValuePair<string, BinaryTree<CaseBase>>(characterName, new BinaryTree<CaseBase>());
            }
            catch (DirectoryNotFoundException)
            {
                return new KeyValuePair<string, BinaryTree<CaseBase>>(characterName, new BinaryTree<CaseBase>());
            }
            List<CaseBase> caseList = (List<CaseBase>) bf.Deserialize(file);

            BinaryTree<CaseBase> loadedTree = new BinaryTree<CaseBase>();
            foreach (CaseBase caseBase in caseList)
            {
                loadedTree.Insert(caseBase);
            }
//            BinaryTree<CaseBase> loadedTree = (BinaryTree<CaseBase>)bf.Deserialize(file).;
            print("Cases loaded by GameManager: " + loadedTree.Count);
            return new KeyValuePair<string, BinaryTree<CaseBase>>(characterName, loadedTree);
        } 

        void Start()
        {
//            Application.LoadLevel("TitleMenu");
        }

        void Update()
        {

        }
    }
}