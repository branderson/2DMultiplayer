using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Menu
{
    public interface ISelectable
    {
        bool AllowSelection(int playerNumber);
        void Activate();
        void Deactivate();
        void Select(int playerNumber, PointerEventData pointer);
        void Unselect(int playerNumber, PointerEventData pointer);
        void Primary(MenuPlayerController player, PointerEventData pointer);
        void Secondary(MenuPlayerController player);
        void Left(MenuPlayerController player);
        void Right(MenuPlayerController player);
    }
}