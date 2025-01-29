using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    public Timer landingStateTimer;
    private Timer coyoteTimer;

    #region Check Variables
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isOnSlope;
    #endregion

    #region Other Variables
    private bool variableJumpHeightAvail;
    private bool coyoteTimeAvail;
    private bool gotoLandingState;
    #endregion

    public PlayerInAirState(Player player, string animBoolName) : base(player, animBoolName)
    {
        coyoteTimer = new Timer(playerData.coyoteTime);
        coyoteTimer.timerAction += CoyoteTimeOver;
        landingStateTimer = new Timer(playerData.gotoLandingStateTime);
        landingStateTimer.timerAction += () => { gotoLandingState = true; };
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isOnSlope = player.detection.isOnSlope();
        isGrounded = player.detection.isGrounded();
        if (isGrounded && currentVelocity.y < epsilon && isOnSlope)
        {
            player.movement.SetVelocityY(0.0f);
        }
        isTouchingWall = player.detection.isDetectingWall(CheckPositionHorizontal.Front, CheckPositionVertical.Top);
    }

    public override void Enter()
    {
        base.Enter();

        player.rigidBody.gravityScale = 9.5f;
        gotoLandingState = false;
        coyoteTimer.StartSingleUseTimer();
        landingStateTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        player.movement.SetVelocityLimitY(0.0f);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        coyoteTimer.Tick(coyoteTimeAvail);
        landingStateTimer.Tick();

        /*player.animator.SetFloat("yVelocity", currentVelocity.y);

        VariableJumpHeight();

        if (escapeInput && player.escapeState.isEscapeAvail())
        {
            stateMachine.ChangeState(player.escapeState);
        }
        else if (attackInput)
        {
            stateMachine.ChangeState(player.normalAttackState);
        }
        else if (jumpInputActive && player.jumpState.IsJumpAvail())
        {
            stateMachine.ChangeState(player.jumpState);
        }
        else if (dodgeInputActive && player.dodgeState.IsDodgeAvail())
        {
            stateMachine.ChangeState(player.dodgeState);
        }
        else if (dashAttackInput && player.dashAttackState.IsDashAttackAvail())
        {
            stateMachine.ChangeState(player.dashAttackState);
        }
        else if (isTouchingWall && inputX == facingDirection && currentVelocity.y < -playerData.wallSlideSpeed)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
        else if (isGrounded)
        {
            if (gotoLandingState)
            {
                stateMachine.ChangeState(player.landingState);
            }
            else if (currentVelocity.y < Mathf.Max(epsilon, Mathf.Abs(player.detection.slopePerpNormal.y * currentVelocity.magnitude)) * 1.1f)
            {
                if (inputX == 0)
                {
                    stateMachine.ChangeState(player.idleState);
                }
                else
                {
                    stateMachine.ChangeState(player.moveState);
                }
            }
        }
        else if (isGrounded && currentVelocity.y < epsilon && inputX == 0)
        {
            stateMachine.ChangeState(player.idleState);
            // 제자리 점프시 점점 경사면 방향으로 내려감
            // 경사면 점프 문제, 경사면에서 점프 후 반대 방향으로 전환시 캐릭터가 가끔 뛰어오름
        }
        else if (isGrounded && inputX != 0 && currentVelocity.y < Mathf.Max(epsilon, Mathf.Abs(player.detection.slopePerpNormal.y * currentVelocity.magnitude)) * 1.1f)
        {
            stateMachine.ChangeState(player.moveState);
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
        if (!onStateExit)
        {
            if (escapeInputPressed && player.escapeState.IsEscapeAvail())
            {
                stateMachine.ChangeState(player.escapeState);
            }
            else if (attackInputActive)
            {
                stateMachine.ChangeState(player.meleeAttackState);
            }
            else if (shieldParryInput && player.shieldParryState.isShieldParryAvail)
            {
                stateMachine.ChangeState(player.shieldParryState);
            }
            else if (jumpInputActive && player.wallSlideState.IsWallJumpAvail())
            {
                stateMachine.ChangeState(player.wallJumpState);
            }
            else if (jumpInputActive && player.jumpState.IsJumpAvail())
            {
                stateMachine.ChangeState(player.jumpState);
            }
            else if (dodgeInputPressed && player.dodgeState.IsDodgeAvail())
            {
                stateMachine.ChangeState(player.dodgeState);
            }
            else if (skillAttackInput && player.dashAttackState.IsDashAttackAvail())
            {
                stateMachine.ChangeState(player.dashAttackState);
            }
            else if (isTouchingWall && inputX == facingDirection && currentVelocity.y < -playerData.wallSlideSpeed)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
            else if (isGrounded)
            {
                if (gotoLandingState)
                {
                    stateMachine.ChangeState(player.landingState);
                }
                else if (currentVelocity.y < Mathf.Max(epsilon, Mathf.Abs(player.detection.slopePerpNormal.y * currentVelocity.magnitude)) * 1.1f)
                {
                    if (inputX == 0)
                    {
                        stateMachine.ChangeState(player.idleState);
                    }
                    else
                    {
                        stateMachine.ChangeState(player.moveState);
                    }
                }
            }
        }
        #endregion

        #region Physics Logic
        if (!onStateExit)
        {
            player.movement.CheckIfShouldFlip(inputX);

            player.animator.SetFloat("yVelocity", currentVelocity.y);

            VariableJumpHeight();

            if (player.moveState.IsDashing())
            {
                player.movement.SetVelocityX(inputX * playerData.dashSpeed);
            }
            else
            {
                player.movement.SetVelocityX(inputX * playerData.moveSpeed);
            }
        }
        #endregion
    }

    public void SetVariableJumpHeightAvail() => variableJumpHeightAvail = true;

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

    private void CoyoteTimeOver()
    {
        player.jumpState.IncreaseJumpCount();
        coyoteTimeAvail = false;
    }

    public void InavailCoyoteTime() => coyoteTimeAvail = false;

    public void AvailCoyoteTime() => coyoteTimeAvail = true;
}
