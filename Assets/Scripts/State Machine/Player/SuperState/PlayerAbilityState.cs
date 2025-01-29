using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    #region Check Variables
    protected bool isOnSlope;
    protected bool isGrounded;
    protected bool isTouchingWall;
    #endregion

    #region Other Variables
    protected bool isAbilityDone;
    #endregion

    public PlayerAbilityState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isOnSlope = player.detection.isOnSlope();
        isGrounded = player.detection.isGrounded();
        isTouchingWall = player.detection.isDetectingWall(CheckPositionHorizontal.Front, CheckPositionVertical.Top);
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        /*if (escapeInput && player.escapeState.isEscapeAvail())
        {
            stateMachine.ChangeState(player.escapeState);
        }
        else if (isAbilityDone)
        {
            if (isGrounded && currentVelocity.y < epsilon)
            {
                if (inputX != 0)
                {
                    stateMachine.ChangeState(player.moveState);
                }
                else
                {
                    stateMachine.ChangeState(player.idleState);
                }
            }
            else if (isTouchingWall && currentVelocity.y < -playerData.wallSlideSpeed)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
            else
            {
                stateMachine.ChangeState(player.inAirState);
            }
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
        if (escapeInputPressed && player.escapeState.IsEscapeAvail())
        {
            stateMachine.ChangeState(player.escapeState);
        }
        else if (isAbilityDone)
        {
            if (attackInputActive)
            {
                stateMachine.ChangeState(player.meleeAttackState);
            }
            else if (rangedAttackInputPressed)
            {
                stateMachine.ChangeState(player.rangedAttackState);
            }
            else if (isGrounded && currentVelocity.y < epsilon)
            {
                if (inputX != 0)
                {
                    stateMachine.ChangeState(player.moveState);
                }
                else
                {
                    stateMachine.ChangeState(player.idleState);
                }
            }
            else if (isTouchingWall && currentVelocity.y < -playerData.wallSlideSpeed)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
            else
            {
                stateMachine.ChangeState(player.inAirState);
            }
        }
        #endregion
    }
}
