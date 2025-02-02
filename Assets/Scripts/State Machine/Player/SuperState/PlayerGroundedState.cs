using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    #region Check Variables
    protected bool isGrounded;
    protected bool isTouchingCeiling;
    protected bool isOnSlope;
    #endregion

    #region Other Variables
    
    #endregion

    public PlayerGroundedState(Player player, string animBoolName) : base(player, animBoolName)
    {
        canTransit = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.detection.isGrounded();
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

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
        if (escapeInputPressed && player.escapeState.IsEscapeAvail())
        {
            stateMachine.ChangeState(player.escapeState);
        }
        else if (canTransit)
        {
            if (shieldParryInput && player.shieldParryState.isShieldParryAvail)
            {
                stateMachine.ChangeState(player.shieldParryState);
            }
            else if (cureInputPressed)
            {
                stateMachine.ChangeState(player.cureState);
            }
            else if (attackInputActive)
            {
                stateMachine.ChangeState(player.meleeAttackState);
            }
            else if (rangedAttackInputPressed)
            {
                stateMachine.ChangeState(player.rangedAttackState);
            }
            else if (skillAttackInput && player.dashAttackState.IsDashAttackAvail())
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
            else if (dodgeInputPressed && player.dodgeState.IsDodgeAvail())
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
