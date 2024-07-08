using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchMoveState : PlayerGroundedState
{
    public PlayerCrouchMoveState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

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
            player.movement.SetVelocityX(playerData.crouchMoveSpeed * inputX);
            player.movement.CheckIfShouldFlip(inputX);

            if (inputY != -1 && !isTouchingCeiling)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else if (inputX == 0)
            {
                stateMachine.ChangeState(player.crouchIdleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
