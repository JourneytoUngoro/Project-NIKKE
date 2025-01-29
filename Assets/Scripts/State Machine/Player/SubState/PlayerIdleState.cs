using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    private Timer dashMaintainTimer;
    // dash ���¿��� �¿�� ��ȯ�� ª�� ���� idle�� �Ǹ鼭 dash ���°� Ǯ���� ������ ����

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
        if (player.rigidBody.IsTouchingLayers(player.detection.whatIsGround))
        {
            player.movement.SetVelocityY(0.0f);
        }
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
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
        }
        #endregion

        #region Physics Logic
        if (!onStateExit)
        {
            player.movement.SetVelocityX(0.0f);

            if (isOnSlope)
            {
                player.movement.SetVelocityY(0.0f);
            }

            player.movement.RigidBodyController();
        }
        #endregion
    }
}
