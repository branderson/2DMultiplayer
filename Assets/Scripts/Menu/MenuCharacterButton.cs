using UnityEngine;
using System.Collections;
using Assets.Scripts.Menu;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    [RequireComponent(typeof (Button))]
    [RequireComponent(typeof (AllowedSelections))]
    [RequireComponent(typeof (MenuSelectable))]
    public class MenuCharacterButton : MonoBehaviour, ISelectable
    {
        [SerializeField] private int index;
        private MenuSelectable menuSelectable;
        private AllowedSelections allowedSelections;

        private void Awake()
        {
//            button = GetComponent<Button>();
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

        public void Select(int playerNumber, PointerEventData pointer)
        {
            ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerEnterHandler);
//            button.targetGraphic.color = Color.blue;
        }

        public void Unselect(int playerNumber, PointerEventData pointer)
        {
            if (!menuSelectable.IsSelected())
            {
                ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerExitHandler);
            }
//            button.targetGraphic.color = Color.white;
        }

        public bool AllowSelection(int playerNumber)
        {
            return allowedSelections.Allow(playerNumber);
        }


        public void Primary(MenuPlayerController player, PointerEventData pointer)
        {
            ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerClickHandler);
            player.SetCharacter(index);
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
