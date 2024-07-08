using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReloadState : PlayerGroundedState
{
    private bool isReloadDone;

    public PlayerReloadState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isReloadDone = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        isReloadDone = false;
        player.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (inputY == -1)
            {
                stateMachine.ChangeState(player.crouchIdleState);
            }
            else if (inputX != 0)
            {
                stateMachine.ChangeState(player.moveState);
            }
            else if (isReloadDone)
            {
                player.normalAttackState.ReloadAmmo();
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
