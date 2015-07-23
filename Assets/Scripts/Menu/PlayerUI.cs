using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class PlayerUI : MonoBehaviour
    {
        public int Shield = 100;
        public int Lives = 3;
        public int PlayerNumber = 0;
        private Text percentText;

        private void Awake()
        {
            percentText = transform.Find("Percent").GetComponent<Text>();
        }

        public void Init(PlayerController playerController)
        {
            gameObject.SetActive(true);
            PlayerNumber = playerController.playerNumber;
            transform.Find("Player").GetComponent<Text>().text += " " + PlayerNumber;
        }

        public void Update()
        {
            percentText.text = Shield + "%";
        }
    }
}