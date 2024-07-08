using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchingWallState : PlayerState
{
    #region Check Variables
    protected bool isGrounded;
    protected bool isTouchingWall;

    protected float initialXPosition;
    #endregion

    public PlayerTouchingWallState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.detection.isGrounded();
        isTouchingWall = player.detection.isTouchingWall();
    }

    public override void Enter()
    {
        base.Enter();

        initialXPosition = currentPosition.x;
        player.inputHandler.AvailInputX();
        player.inAirState.SetVariableJumpHeightAvail();
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

        #region State Transition Logic
        if (isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (!isTouchingWall || inputX != facingDirection)
        {
            stateMachine.ChangeState(player.inAirState);
        }
        else if (dashAttackInput && player.dashAttackState.IsDashAttackAvail())
        {
            stateMachine.ChangeState(player.dashAttackState);
        }
        #endregion

        #region Physics Logic
        player.movement.SetPositionX(initialXPosition);
        #endregion
    }
}
