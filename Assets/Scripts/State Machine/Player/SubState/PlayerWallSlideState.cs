using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerTouchingWallState
{
    private bool wasAttackInputted;
    private float attackInputHoldTime;

    public PlayerWallSlideState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        wasAttackInputted = false;
        attackInputHoldTime = 0.0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        /*if (!onStateExit)
        {
            if (attackInput)
            {
                wasAttackInputted = true;
                attackInputHoldTime += Time.deltaTime;

                if (attackInputHoldTime > 0.1f)
                {
                    stateMachine.ChangeState(player.wallGrabState);
                }
            }
            else if (wasAttackInputted)
            {
                stateMachine.ChangeState(player.wallAttackState);
            }
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            #region State Transition Logic
            if (attackInput)
            {
                wasAttackInputted = true;
                attackInputHoldTime += Time.deltaTime;

                if (attackInputHoldTime > 0.1f)
                {
                    stateMachine.ChangeState(player.wallGrabState);
                }
            }
            else if (wasAttackInputted)
            {
                stateMachine.ChangeState(player.wallAttackState);
            }
            #endregion

            #region Physics Logic
            player.movement.SetVelocityY(-playerData.wallSlideSpeed);
            #endregion
        }
    }
}
