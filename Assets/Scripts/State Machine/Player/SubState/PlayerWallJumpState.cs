using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallJumpState : PlayerAbilityState
{
    public Timer preventInputXTimer;

    private int jumpDirection;
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

        variableJumpHeightAvail = true;
        player.wallSlideState.InavailWallJump();
        player.inputHandler.InactiveJumpInput();
        player.inputHandler.PreventInputX(jumpDirection);
        preventInputXTimer.StartSingleUseTimer();
        // player.jumpState.IncreaseJumpCount();
        if (facingDirection != jumpDirection)
        {
            player.movement.Flip();
        }
        workSpace.Set(playerData.wallJumpAngleVector.x * jumpDirection, playerData.wallJumpAngleVector.y);
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
        // Wall Jump State�� ���� ª�� �ð� ���� �����ȴ�.
        // ���� Air Attack�̳� Dash�� ���� �ٸ� State�� ��ȯ�� �� �ִ�.
        // Wall Jump�� �÷��̾ ���� �ð� ���� ���� �������ιۿ� �̵����� ���ϵ��� �����ϴµ�, �̸� Escape�� ĵ���� �� �ִ�.
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

    public void SetJumpDirection(int direction) => jumpDirection = direction;
}
