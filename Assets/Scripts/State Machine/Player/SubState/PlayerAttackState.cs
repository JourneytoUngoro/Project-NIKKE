using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    public PlayerAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.stateMachineToAnimator.state = this;
        player.inputHandler.InactiveAttackInput();
    }

    public override void Exit()
    {
        base.Exit();

        player.combat.ClearDamagedTargets();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (dodgeInputActive && player.dodgeState.IsDodgeAvail())
        {
            stateMachine.ChangeState(player.dodgeState);
        }
    }
}
