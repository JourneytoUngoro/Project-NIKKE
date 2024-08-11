using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCureState : PlayerAbilityState
{
    public PlayerCureState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.stats.health.IncreaseCurrentValue(player.stats.health.maxValue);
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

        player.movement.SetVelocityZero();
        player.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region Physics Logic
        if (!onStateExit)
        {
            player.movement.RigidBodyController();
            player.movement.SetVelocityX(0.0f);
        }
        #endregion
    }
}
