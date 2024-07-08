using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallAttackState : PlayerTouchingWallState
{
    public PlayerWallAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
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
            player.movement.SetVelocityY(-playerData.wallSlideSpeed);

            if (isAnimationFinished)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
