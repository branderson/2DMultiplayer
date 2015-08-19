using System.Runtime.InteropServices;
using Assets.Scripts.Player.Triggers;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class BlockStartState : PlayerState
    {
        [SerializeField] private int parryFrames = 10;
        [SerializeField] private int stunFrames = 30;
        [SerializeField] private int parryableStagger = 4;
        private int dodgeCountdown = 2;
        private bool blockReleased = false;
        private int parryCountdown;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            dodgeCountdown = 2;
            playerController.Run = false;
            if (playerController.BlockStrength <= 0)
            {
                playerAnimator.SetTrigger("BlockReleased");
                playerController.Blocking = false;
            }
            playerController.Blocking = true;
            // Player is invincible during the first few frames, during which they will parry attacks
            playerController.IFrames = parryFrames;

            blockReleased = false;
            parryCountdown = parryFrames;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (!PlayerInputController.ButtonActive("Block"))
            {
                playerAnimator.SetTrigger("BlockReleased");
                blockReleased = true;
                playerController.Blocking = false;
            }
            parryCountdown--;
            dodgeCountdown--;
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            playerController.Blocking = false;
        }

        override public void Left()
        {
            if (dodgeCountdown <= 0)
            {
//                base.Left();
            }
        }

        override public void Right()
        {
            if (dodgeCountdown <= 0)
            {
//                base.Right();
            }
        }
        
        override public void TakeHit(AttackData attackData)
        {
            if (parryCountdown > 1)
            {
                if (attackData.Stagger > parryableStagger || playerController.GetShield() != 0)
                {
                    playerController.IFrames = 0;
                    playerController.Invincible = false;
                    parryCountdown = 0;
                    playerController.TakeKnockback(attackData);
                }
                else
                {
                    if (attackData.Player.transform.position.x < playerController.transform.position.x)
                    {
                        playerController.Flip();
                    }
                    attackData.Player.Stun(stunFrames, false);
                    playerAnimator.SetTrigger("Parry");
                }
            }
            else
            {
                playerController.BlockStrength -= attackData.Damage + attackData.ShieldDamage;
                if (playerController.BlockStrength <= 0)
                {
                    playerController.BlockStrength -= playerController.BlockBreakPenalty;
                    playerController.Stun(playerController.BlockBreakStun, false);
                }
            }
        }

        public override string GetName()
        {
            return "BlockStart";
        }
    }
}