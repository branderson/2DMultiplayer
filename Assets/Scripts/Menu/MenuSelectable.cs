using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public void Primary(MenuPlayerController player)
        {
            uiElement.Primary(player);
        }

        public void Secondary(MenuPlayerController player)
        {
            uiElement.Secondary(player);
        }

        public bool AllowSelection(MenuPlayerController player)
        {
            return uiElement.AllowSelection(player.playerNumber);
        }

        public void Select(MenuPlayerController player)
        {
            selectors.Add(player);
            uiElement.Select(player.playerNumber);
        }

        public void Unselect(MenuPlayerController player)
        {
            selectors.Remove(player);
            uiElement.Unselect(player.playerNumber);
        }

        public bool SelectedBy(int playerNumber)
        {
            return selectors.Any(selector => selector.playerNumber == playerNumber);
        }

        public bool IsSelected()
        {
            return selectors.Any();
        }
    }
}