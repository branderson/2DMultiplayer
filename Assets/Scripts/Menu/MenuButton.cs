using UnityEngine;
using System.Collections;
using Assets.Scripts.Menu;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    [RequireComponent(typeof (Button))]
    [RequireComponent(typeof (AllowedSelections))]
    public class MenuButton : MonoBehaviour, ISelectable
    {
        private Button button;
        private MenuSelectable menuSelectable;
        private AllowedSelections allowedSelections;

        private void Awake()
        {
            button = GetComponent<Button>();
            menuSelectable = GetComponent<MenuSelectable>();
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
//            button.targetGraphic.color = Color.blue;
        }

        public void Unselect(int playerNumber)
        {
            button.targetGraphic.color = Color.white;
        }

        public bool AllowSelection(int playerNumber)
        {
            return allowedSelections.Allow(playerNumber);
        }


        public void Primary(MenuPlayerController player)
        {
        }

        public void Secondary(MenuPlayerController player)
        {
        }

        public void Left(MenuPlayerController player)
        {
        }

        public void Right(MenuPlayerController player)
        {
        }
    }
}
