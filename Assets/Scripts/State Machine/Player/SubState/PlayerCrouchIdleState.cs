using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchIdleState : PlayerGroundedState
{
    public PlayerCrouchIdleState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.movement.SetVelocityZero();
        player.SetCapsuleColliderSize(playerData.crouchColliderSize);
    }

    public override void Exit()
    {
        base.Exit();

        player.SetCapsuleColliderSize(playerData.standColliderSize);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (inputX != 0 && (inputY == -1 || isTouchingCeiling))
            {
                stateMachine.ChangeState(player.crouchMoveState);
            }
            else if (inputY != -1 && !isTouchingCeiling)
            {
                stateMachine.ChangeState(player.idleState);
            }
            /*else if (player.normalAttackState.currentAmmo < playerData.maxAmmo)
            {
                stateMachine.ChangeState(player.crouchReloadState);
            }*/
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
