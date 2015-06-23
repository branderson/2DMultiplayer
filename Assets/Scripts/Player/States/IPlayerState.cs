using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public interface IPlayerState
    {
        void Jump();
        void Move(float h, float v);
        void Action1(float h, float v);
        void Action2(float h, float v);
        void Block();
        void Throw();
    }
}