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

        public void Awake()
        {
            uiElement = GetComponent<ISelectable>();
        }

        public void Up(int playerNumber)
        {
            
        }

        public void Down(int playerNumber)
        {
            
        }

        public void Left(int playerNumber)
        {
            uiElement.Left(playerNumber);
        }

        public void Right(int playerNumber)
        {
            uiElement.Right(playerNumber);
        }

        public void Primary(int playerNumber)
        {
            uiElement.Primary(playerNumber);
        }

        public void Secondary(int playerNumber)
        {
            uiElement.Secondary(playerNumber);
        }

        public bool AllowSelection(int playerNumber)
        {
            return uiElement.AllowSelection(playerNumber);
        }

        public void Select(int playerNumber)
        {
            uiElement.Select(playerNumber);
        }

        public void Unselect(int playerNumber)
        {
            uiElement.Unselect(playerNumber);
        }
    }
}