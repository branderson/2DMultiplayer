using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Menu
{
    [RequireComponent(typeof (ISelectable))]
    public class MenuSelectable : MonoBehaviour
    {
        [SerializeField] public MenuSelectable up;
        [SerializeField] public MenuSelectable down;
        [SerializeField] public MenuSelectable left;
        [SerializeField] public MenuSelectable right;
        private ISelectable uiElement;
        private List<MenuPlayerController> selectors;

        public void Awake()
        {
            uiElement = GetComponent<ISelectable>();
            selectors = new List<MenuPlayerController>();
        }

        public void Activate()
        {
            uiElement.Activate();
        }

        public void Deactivate()
        {
            uiElement.Deactivate();
        }

        public void Up(MenuPlayerController player)
        {
            
        }

        public void Down(MenuPlayerController player)
        {
            
        }

        public void Left(MenuPlayerController player)
        {
            uiElement.Left(player);
        }

        public void Right(MenuPlayerController player)
        {
            uiElement.Right(player);
        }

        public void Primary(MenuPlayerController player, PointerEventData pointer)
        {
            uiElement.Primary(player, pointer);
        }

        public void Secondary(MenuPlayerController player)
        {
            uiElement.Secondary(player);
        }

        public bool AllowSelection(MenuPlayerController player)
        {
            if (uiElement == null)
            {
                return false;
            }
            return uiElement.AllowSelection(player.PlayerNumber);
        }

        public void Select(MenuPlayerController player, PointerEventData pointer)
        {
            selectors.Add(player);
            uiElement.Select(player.PlayerNumber, pointer);
        }

        public void Unselect(MenuPlayerController player, PointerEventData pointer)
        {
            selectors.Remove(player);
            uiElement.Unselect(player.PlayerNumber, pointer);
        }

        public bool SelectedBy(int playerNumber)
        {
            return selectors.Any(selector => selector.PlayerNumber == playerNumber);
        }

        public bool IsSelected()
        {
            return selectors.Any();
        }
    }
}