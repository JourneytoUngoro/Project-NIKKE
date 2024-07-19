using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    #region Check Variables
    protected bool isGrounded;
    protected bool isTouchingWall;
    #endregion

    protected bool isAbilityDone;

    public PlayerAbilityState(Player player, string animBoolName) : base(player, animBoolName)
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
        if (escapeInput && player.escapeState.isEscapeAvail())
        {
            stateMachine.ChangeState(player.escapeState);
        }
        else if (isAbilityDone)
        {
            if (attackInputActive)
            {
                stateMachine.ChangeState(player.meleeAttackState);
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
