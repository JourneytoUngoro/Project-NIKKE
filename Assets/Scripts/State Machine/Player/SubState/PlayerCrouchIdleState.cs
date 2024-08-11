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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
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
        #endregion

        #region Physics Logic
        if (!onStateExit)
        {
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
        }
        #endregion
    }
}
