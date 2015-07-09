using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public interface ISelectable
    {
        bool AllowSelection(int playerNumber);
        void Select(int playerNumber);
        void Unselect(int playerNumber);
        void Primary(MenuPlayerController player);
        void Secondary(MenuPlayerController player);
        void Left(MenuPlayerController player);
        void Right(MenuPlayerController player);
    }
}