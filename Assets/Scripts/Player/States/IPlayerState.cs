using System.Collections.Generic;
using Assets.Scripts.Player.Triggers;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public interface IPlayerState
    {
        void Jump();
        void Run();
        void Move(float x, float y);
        void Up();
        void Down();
        void Left();
        void Right();
        void Primary(float x, float y);
        void Secondary(float x, float y);
        void Block();
        void Grab();
        void TakeHit(AttackData attackData);
        void ProcessAI(List<Transform> opponentPositions);
        byte GetStateID();
    }
}