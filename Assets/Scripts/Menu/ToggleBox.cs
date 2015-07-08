using UnityEngine;
using System.Collections;
using Assets.Scripts.Menu;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    [RequireComponent(typeof (Toggle))]
    [RequireComponent(typeof (AllowedSelections))]
    public class ToggleBox : MonoBehaviour, ISelectable
    {
        private Toggle toggle;
        private AllowedSelections allowedSelections;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            allowedSelections = GetComponent<AllowedSelections>();
        }

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void Select(int playerNumber)
        {
            toggle.targetGraphic.color = Color.blue;
        }

        public void Unselect(int playerNumber)
        {
            toggle.targetGraphic.color = Color.white;
        }

        public void Primary(int playerNumber)
        {
//            GetComponent<Transform>().Find("Label").gameObject.GetComponent<Text>().text = "It works!";
            toggle.isOn = !toggle.isOn;
        }

        public void Secondary(int playerNumber)
        {
        }

        public void Left(int playerNumber)
        {
        }

        public void Right(int playerNumber)
        {
        }

        public bool AllowSelection(int playerNumber)
        {
            return allowedSelections.Allow(playerNumber);
        }
    }
}