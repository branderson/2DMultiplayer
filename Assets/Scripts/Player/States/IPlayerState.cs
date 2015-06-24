using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public interface IPlayerState
    {
        void Jump();
        void Move(float x, float y);
        void Action1(float x, float y);
        void Action2(float x, float y);
        void Block();
        void Throw();
    }
}