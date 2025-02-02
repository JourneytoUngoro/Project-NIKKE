using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    private bool shouldTransitToStunnedState;

    private bool isGrounded;

    public Timer knockbackTimer { get; private set; }

    public PlayerKnockbackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        knockbackTimer = new Timer(0.0f);
        knockbackTimer.timerAction += () => { canTransit = true; };
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

        if (knockbackTimer.duration != 0.0f)
        {
            knockbackTimer.StartSingleUseTimer();
        }
        player.rigidBody.gravityScale = 9.5f;
        player.movement.SetVelocityMultiplier(Vector2.one);
        player.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        shouldTransitToStunnedState = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        knockbackTimer.Tick();
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
                    if (escapeInputPressed && player.escapeState.IsEscapeAvail())
                    {
                        stateMachine.ChangeState(player.escapeState);
                    }
                    else if (dodgeInputPressed && player.dodgeState.IsDodgeAvail())
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

        // TODO: Fix first parameter of RigidBodyController
        #region Physics Logic
        if (!onStateExit)
        {
            if (knockbackTimer.duration == 0.0f)
            {
                canTransit = player.rigidBody.velocity.y < epsilon && isGrounded;
            }

            player.movement.RigidBodyController(false, false);
        }
        #endregion
    }

    public void ShouldTransitToStunnedState() => shouldTransitToStunnedState = true;
}
