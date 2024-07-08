using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState : PlayerGroundedState
{
    private Timer stateTransitTimer;

    public PlayerLandingState(Player player, string animBoolName) : base(player, animBoolName)
    {
        stateTransitTimer = new Timer(playerData.landingTime);
        stateTransitTimer.timerAction += () => { allowStateTransition = true; };
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        allowStateTransition = false;
        player.movement.SetVelocityX(0.0f);
        player.inputHandler.PreventInputX(inputX);
        stateTransitTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        player.inputHandler.AvailInputX();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        stateTransitTimer.Tick();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            #region State Transition Logic
            if (allowStateTransition)
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
            #endregion

            #region Physics Logic
            player.movement.SetVelocityX(0.0f);

            if (isOnSlope)
            {
                player.rigidBody.gravityScale = 0.0f;
                player.movement.SetVelocityY(0.0f);
            }
            else
            {
                player.rigidBody.gravityScale = 9.5f;
            }
            #endregion
        }
    }
}
