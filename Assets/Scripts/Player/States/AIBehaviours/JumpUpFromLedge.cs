using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Player.States.AIBehaviours
{
    public class JumpUpFromLedge : AIBehaviour
    {
        override public void Process(List<Transform> opponentPositions)
        {
            PlayerInputController.Jump();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
        {
            base.OnStateEnter(animator, stateinfo, layerindex);
        }
    }
}