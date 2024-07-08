using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEscapeState : PlayerState
{
    public Timer escapeCoolDownTimer;

    private Timer escapeTimer;

    private bool isGrounded;
    private bool escapeAvail;
    private bool setVelocityZero;

    public PlayerEscapeState(Player player, string animBoolName) : base(player, animBoolName)
    {
        escapeAvail = true;
        escapeTimer = new Timer(playerData.escapeTime);
        escapeTimer.timerAction += () => { setVelocityZero = true; };
        escapeCoolDownTimer = new Timer(playerData.escapeCoolDownTime);
        escapeCoolDownTimer.timerAction += () => { escapeAvail = true; };
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.detection.isGrounded();
    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.prevState.AnimationFinishTrigger();
        // 공격하다가 도중에 빠져나왔을 경우
        player.inputHandler.AvailInputX();
        SetInputVariables();
        escapeAvail = false;
        player.stateMachineToAnimator.state = this;
        player.animator.SetInteger("inputX", inputX);

        if (inputX == 0 || stateMachine.prevState.GetType().BaseType == typeof(PlayerTouchingWallState))
        {
            player.movement.SetVelocityWithDirection(playerData.escapeAngleVector, -facingDirection, playerData.escapeSpeed);
        }
        else
        {
            player.movement.SetVelocityWithDirection(Vector2.right, inputX, playerData.escapeSpeed);
            player.movement.Flip();
        }
        escapeCoolDownTimer.StartSingleUseTimer();
        escapeTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        setVelocityZero = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        escapeTimer.Tick();

        if (isAnimationFinished)
        {
            if (isGrounded)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.inAirState);
            }
        }
        else if (setVelocityZero)
        {
            player.movement.SetVelocityZero();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public bool isEscapeAvail() => escapeAvail;
}
