using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    #region Check Variables
    protected bool isGrounded;
    protected bool isTouchingWall;
    protected bool isTouchingCeiling;
    protected bool isOnSlope;
    #endregion

    #region
    protected bool allowStateTransition;
    #endregion

    public PlayerGroundedState(Player player, string animBoolName) : base(player, animBoolName)
    {
        allowStateTransition = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.detection.isGrounded();
        isTouchingWall = player.detection.isTouchingWall();
        isTouchingCeiling = player.detection.isTouchingCeiling();
        isOnSlope = player.detection.isOnSlope();
    }

    public override void Enter()
    {
        base.Enter();

        player.jumpState.ResetJumpCount();
        player.inAirState.AvailCoyoteTime();
        player.inputHandler.AvailInputX();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        /*if (allowStateTransition)
        {
            if (escapeInput && player.escapeState.isEscapeAvail())
            {
                stateMachine.ChangeState(player.escapeState);
            }
            else if (blockParryInput && player.blockParryState.IsBlockAvail())
            {
                stateMachine.ChangeState(player.blockParryState);
            }
            else if (attackInput)
            {
                stateMachine.ChangeState(player.normalAttackState);
            }
            else if (dashAttackInput && player.dashAttackState.IsDashAttackAvail())
            {
                stateMachine.ChangeState(player.dashAttackState);
            }
            else if (!isGrounded && !isOnSlope)
            {
                stateMachine.ChangeState(player.inAirState);
            }
            else if (jumpInputActive && player.jumpState.IsJumpAvail() && !isTouchingCeiling)
            {
                stateMachine.ChangeState(player.jumpState);
            }
            else if (dodgeInputActive && player.dodgeState.IsDodgeAvail())
            {
                stateMachine.ChangeState(player.dodgeState);
            }
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
        if (allowStateTransition)
        {
            if (escapeInput && player.escapeState.isEscapeAvail())
            {
                stateMachine.ChangeState(player.escapeState);
            }
            else if (blockParryInput && player.blockParryState.IsBlockAvail())
            {
                stateMachine.ChangeState(player.blockParryState);
            }
            else if (attackInput)
            {
                stateMachine.ChangeState(player.normalAttackState);
            }
            else if (dashAttackInput && player.dashAttackState.IsDashAttackAvail())
            {
                stateMachine.ChangeState(player.dashAttackState);
            }
            else if (!isGrounded && !isOnSlope)
            {
                stateMachine.ChangeState(player.inAirState);
            }
            else if (jumpInputActive && player.jumpState.IsJumpAvail() && !isTouchingCeiling)
            {
                stateMachine.ChangeState(player.jumpState);
            }
            else if (dodgeInputActive && player.dodgeState.IsDodgeAvail())
            {
                stateMachine.ChangeState(player.dodgeState);
            }
        }
        #endregion

        #region Physics Logic
        player.movement.CheckIfShouldFlip(inputX);
        #endregion
    }
}
