using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts.Player.States
{
    public class PunchState : PlayerState
    {
        [SerializeField] private int waitFrames = 10;
        [SerializeField] private bool directionalControl = true;
        private bool flip = false;
        private int waitCounter;
        private Vector2 move;

        public virtual new void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            base.OnStateEnter(animator, stateinfo, layerindex);
            waitCounter = waitFrames;
            flip = false;
        }

        public virtual new void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            waitCounter -= 1;
            if (waitCounter > 0)
            {
                if (flip && directionalControl)
                {
                    flip = false;
//                    MonoBehaviour.print("Flipping");
                    playerController.Flip();
                }
            }
        }

        public virtual new void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        public override string GetName()
        {
            return "Punch";
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);
            move = new Vector2(x, y);
            if ((move.x < 0 && playerController.facingRight) || (move.x > 0 && !playerController.facingRight))
            {
//                MonoBehaviour.print("Flip = true");
                flip = true;
            }
        }

        public override void Up()
        {
            if (waitCounter > 7)
            {
                base.Up();
            }
        }

        public override void Down()
        {
            if (waitCounter > 7)
            {
                base.Down();
            }
        }

        public override void Left()
        {
            if (waitCounter > 7)
            {
                base.Left();
                if (playerController.facingRight)
                {
                    flip = false;
                    playerController.Flip();
                }
            }
        }

        public override void Right()
        {
            if (waitCounter > 7)
            {
                base.Right();
                if (!playerController.facingRight)
                {
                    flip = false;
                    playerController.Flip();
                }
            }
        }
    }
}