using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerAbilityState
{
    public Timer preventInputXTimer;

    private bool variableJumpHeightAvail;

    public PlayerWallJumpState(Player player, string animBoolName) : base(player, animBoolName)
    {
        preventInputXTimer = new Timer(playerData.preventInputXTime);
        preventInputXTimer.timerAction += player.inputHandler.AvailInputX;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.inputHandler.InactiveJumpInput();
        preventInputXTimer.StartSingleUseTimer();
        player.jumpState.IncreaseJumpCount();
        player.movement.Flip();
        workSpace.Set(playerData.wallJumpAngleVector.x * facingDirection, playerData.wallJumpAngleVector.y);
        player.movement.SetVelocity(workSpace.normalized * playerData.wallJumpSpeed);
    }

    public override void Exit()
    {
        base.Exit();

        if (stateMachine.nextState == player.inAirState)
        {
            player.inAirState.landingStateTimer.AdjustTimeFlow(Time.time - startTime);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        /*if (!onStateExit)
        {
            player.animator.SetFloat("yVelocity", currentVelocity.y);

            VariableJumpHeight();

            if (currentVelocity.y < epsilon)
            {
                isAbilityDone = true;
            }
        }*/
        // Wall Jump State는 아주 짧은 시간 동안 강제된다.
        // 이후 Air Attack이나 Dash와 같은 다른 State로 전환될 수 있다.
        // Wall Jump는 플레이어가 일정 시간 동안 한쪽 방향으로밖에 이동하지 못하도록 강제하는데, 이를 Escape로 캔슬할 수 있다.
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
        if (!onStateExit)
        {
            player.animator.SetFloat("yVelocity", currentVelocity.y);

            VariableJumpHeight();

            if (currentVelocity.y < epsilon)
            {
                isAbilityDone = true;
            }
        }
        #endregion
    }

    private void VariableJumpHeight()
    {
        if (variableJumpHeightAvail)
        {
            if (!jumpInput)
            {
                player.movement.SetVelocityY(currentVelocity.y * playerData.variableJumpHeightMultiplier);
                variableJumpHeightAvail = false;
            }
            else if (currentVelocity.y < epsilon)
            {
                variableJumpHeightAvail = false;
            }
        }
    }
}
