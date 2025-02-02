using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedAttackState : PlayerAbilityState
{
    private int rangedAttackIndex = 0;
    private int currentRangedAttackIndex = 0;

    public PlayerRangedAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        if (!player.animator.GetBool("gotoNextIndex"))
        {
            isAbilityDone = true;
        }
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        canTransit = true;
        isAbilityDone = false;
        currentRangedAttackIndex = (currentRangedAttackIndex + 1) % 3;
        player.combat.DoAttack(player.combat.rangedAttacks[0]);
        GameObject attackEffect = Manager.Instance.objectPoolingManager.GetGameObject("PlayerRangedAttackEffect");
        attackEffect.transform.position = player.transform.position;
        attackEffect.transform.rotation = player.transform.rotation;
        attackEffect.GetComponent<Animator>().SetInteger("rangedAttackIndex", currentRangedAttackIndex);
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;
        rangedAttackIndex = 0;
        currentRangedAttackIndex = -1;
        player.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        player.animator.ResetTrigger("gotoNextIndex");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (canTransit)
            {
                if (rangedAttackInputPressed && currentRangedAttackIndex == rangedAttackIndex)
                {
                    rangedAttackIndex = (rangedAttackIndex + 1) % 3;
                    player.animator.SetTrigger("gotoNextIndex");
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region Physics Logic
        if (!onStateExit)
        {
            if (isGrounded)
            {
                player.movement.SetVelocityX(0.0f);

                if (isOnSlope)
                {
                    player.movement.SetVelocityY(0.0f);
                }

                player.movement.RigidBodyController();
            }
            else
            {

            }
        }
        #endregion
    }
}
