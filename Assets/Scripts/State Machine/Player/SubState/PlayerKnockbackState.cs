using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    private bool shouldTransitToStunnedState;

    private bool canTransit;

    private bool isGrounded;

    private Vector2 knockbackVelocity;

    private Timer allowTransitTimer;

    public PlayerKnockbackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        allowTransitTimer = new Timer(0.0f);
        allowTransitTimer.timerAction += () => { canTransit = true; };
        // player.stats.posture.OnCurrentValueMax += () => { shouldTransitToStunnedState = true; };
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.detection.isGrounded();
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;
        allowTransitTimer.StartSingleUseTimer();
        player.rigidBody.gravityScale = 9.5f;
        player.movement.SetVelocityMultiplier(Vector2.one);
        // player.movement.SetVelocity(knockbackVelocity);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        allowTransitTimer.Tick();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
        if (!onStateExit)
        {
            if (canTransit)
            {
                if (shouldTransitToStunnedState)
                {
                    stateMachine.ChangeState(player.stunnedState);
                }
                else
                {
                    if (escapeInput && player.escapeState.IsEscapeAvail())
                    {
                        stateMachine.ChangeState(player.escapeState);
                    }
                    else if (dodgeInputActive && player.dodgeState.IsDodgeAvail())
                    {
                        stateMachine.ChangeState(player.dodgeState);
                    }
                    else if (isGrounded && currentVelocity.y < epsilon)
                    {
                        stateMachine.ChangeState(player.idleState);
                    }
                    else
                    {
                        stateMachine.ChangeState(player.inAirState);
                    }
                }
            }
        }
        #endregion

        #region Physics Logic
        if (!onStateExit)
        {
            player.movement.RigidBodyController(false, false);
        }
        #endregion
    }

    public void SetKnockback(float duration)//, Vector2 knockbackVelocity)
    {
        allowTransitTimer.ChangeDuration(duration);
        // this.knockbackVelocity = knockbackVelocity;
    }

    public void ShouldTransitToStunnedState()
    {
        shouldTransitToStunnedState = true;
    }
}
