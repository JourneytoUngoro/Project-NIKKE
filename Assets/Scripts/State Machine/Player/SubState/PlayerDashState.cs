using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerGroundedState
{
    public bool isDashing;

    public PlayerDashState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        isDashing = true;
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
                stateMachine.ChangeState(player.crouchMoveState);
            }
            else if (inputX == 0)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (isOnSlope)
            {
                workSpace.Set(player.detection.slopePerpNormal.x, player.detection.slopePerpNormal.y * inputX);
                player.movement.SetVelocity(workSpace, playerData.dashSpeed);
            }
            else
            {
                player.movement.SetVelocityX(inputX * playerData.dashSpeed);
                player.movement.SetVelocityY(0.0f);
            }
        }
    }

    public void SetIsDashingToFalse() => isDashing = false;
}
