using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallGrabState : PlayerTouchingWallState
{
    public PlayerWallGrabState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.inputHandler.InactiveAttackInput();
        player.movement.SetVelocityY(0.0f);
    }

    public override void Exit()
    {
        base.Exit();

        player.rigidBody.gravityScale = 9.5f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        /*if (!onStateExit)
        {
            if (!attackInput)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            #region State Transition Logic
            if (!attackInput)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
            #endregion

            #region Physics Logic
            player.movement.SetVelocityY(0.0f);
            player.rigidBody.gravityScale = 0.0f;
            #endregion
        }
    }
}
