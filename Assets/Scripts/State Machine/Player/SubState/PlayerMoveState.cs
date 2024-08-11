using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public Timer dashInputTimer;

    private int prevInputX;
    private bool isDashInput;
    public bool isDashing;

    public PlayerMoveState(Player player, string animBoolName) : base(player, animBoolName)
    {
        dashInputTimer = new Timer(playerData.dashInputTime);
        dashInputTimer.timerAction += () => { isDashInput = false; };
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.rigidBody.gravityScale = 9.5f;

        if (!isDashInput)
        {
            dashInputTimer.StartSingleUseTimer();
            isDashInput = true;
            prevInputX = inputX;
        }
        else if (prevInputX == inputX)
        {
            isDashInput = false;
            isDashing = true;
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.movement.SetVelocityMultiplier(Vector2.one);
        
        if (stateMachine.nextState != player.idleState)
        {
            prevInputX = 0;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        /*if (!onStateExit)
        {
            if (inputY == -1)
            {
                stateMachine.ChangeState(player.crouchMoveState);
            }
            else if (inputX == 0)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
        if (!onStateExit)
        {
            if (inputY == -1)
            {
                stateMachine.ChangeState(player.crouchMoveState);
            }
            else if (inputX == 0)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
        #endregion

        #region Physics Logic
        if (!onStateExit)
        {
            player.movement.RigidBodyController();

            if (isDashing)
            {
                player.movement.SetVelocityX(playerData.dashSpeed * inputX, true);
            }
            else
            {
                player.movement.SetVelocityX(playerData.moveSpeed * inputX, true);
            }
        }
        #endregion
    }

    public bool IsDashing() => isDashing;

    public void PreventDash() => isDashing = false;
}
