using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public interface ISelectable
    {
        bool AllowSelection(int playerNumber);
        void Select(int playerNumber);
        void Unselect(int playerNumber);
        void Primary(int playerNumber);
        void Secondary(int playerNumber);
        void Left(int playerNumber);
        void Right(int playerNumber);
    }
}