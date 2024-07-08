using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    private Timer dashMaintainTimer;
    // dash 상태에서 좌우로 전환시 짧은 순간 idle이 되면서 dash 상태가 풀리는 현상을 방지

    public PlayerIdleState(Player player, string animBoolName) : base(player, animBoolName)
    {
        dashMaintainTimer = new Timer(playerData.dashMaintainTime);
        dashMaintainTimer.timerAction += () => { player.moveState.PreventDash(); };
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.movement.SetVelocityX(0.0f);
        dashMaintainTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        player.rigidBody.gravityScale = 9.5f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        dashMaintainTimer.Tick();

        if (!onStateExit)
        {
            if (inputY == -1)
            {
                if (inputX == 0)
                {
                    stateMachine.ChangeState(player.crouchIdleState);
                }
                else
                {
                    stateMachine.ChangeState(player.crouchMoveState);
                }
            }
            else if (inputX != 0)
            {
                stateMachine.ChangeState(player.moveState);
            }
            else if (player.normalAttackState.currentAmmo < playerData.maxAmmo)
            {
                stateMachine.ChangeState(player.reloadState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            /*#region State Transition Logic
            if (inputY == -1)
            {
                if (inputX == 0)
                {
                    stateMachine.ChangeState(player.crouchIdleState);
                }
                else
                {
                    stateMachine.ChangeState(player.crouchMoveState);
                }
            }
            else if (inputX != 0)
            {
                stateMachine.ChangeState(player.moveState);
            }
            else if (player.normalAttackState.currentAmmo < playerData.maxAmmo)
            {
                stateMachine.ChangeState(player.reloadState);
            }
            #endregion*/

            #region Physics Logic
            player.movement.SetVelocityX(0.0f);

            if (isOnSlope)
            {
                player.rigidBody.gravityScale = 0.0f;
                player.movement.SetVelocityY(0.0f);
            }
            else
            {
                player.rigidBody.gravityScale = 9.5f;
            }
            #endregion
        }
    }
}
