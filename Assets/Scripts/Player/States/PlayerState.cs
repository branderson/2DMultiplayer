using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public abstract class PlayerState : StateMachineBehaviour, IPlayerState
    {
        public abstract PlayerController2 playerController { set; }

        public abstract string GetName();

        public abstract void Jump();

        public void Move(float h, float v)
        {
            throw new System.NotImplementedException();
        }

        public void Action1(float h, float v)
        {
            throw new System.NotImplementedException();
        }

        public void Action2(float h, float v)
        {
            throw new System.NotImplementedException();
        }

        public void Block()
        {
            throw new System.NotImplementedException();
        }

        public void Throw()
        {
            throw new System.NotImplementedException();
        }
    }
}