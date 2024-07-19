using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
    #region Components
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;
    protected string animBoolName;
    #endregion

    #region Input Variables
    protected int inputX;
    protected int inputY;

    protected bool jumpInput;
    protected bool jumpInputActive;

    protected bool dodgeInput;
    protected bool dodgeInputActive;

    protected bool attackInput;
    protected bool attackInputActive;

    protected bool escapeInput;

    protected bool dashAttackInput;

    protected bool blockParryInput;
    #endregion

    public PlayerState(Player player, string animBoolName)
    {
        this.player = player;
        this.stateMachine = player.playerStateMachine;
        this.playerData = player.playerData;
        this.animBoolName = animBoolName;
        player.movement.synchronizeValues += SetMovementVariables;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.animator.SetBool(animBoolName, true);
        SetInputVariables();
        SetMovementVariables();
        
        DoChecks();
    }

    public virtual void Exit()
    {
        player.animator.SetBool(animBoolName, false);
        onStateExit = true;

        if (stateMachine.nextState != player.moveState && stateMachine.nextState != player.idleState && stateMachine.nextState != player.jumpState && stateMachine.nextState != player.inAirState)
        {
            player.moveState.PreventDash();
        }
    }

    public virtual void LogicUpdate()
    {
        TickPublicTimers();
        /*if (player.dashState.isDashing)
        {
            player.afterImagePool.StartAfterImage(player.spriteRenderer, 0.2f, 0.8f);
        }
        else
        {
            player.afterImagePool.StopAfterImage();
        }*/
    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
        SetInputVariables();
        SetMovementVariables();
    }

    protected void SetInputVariables()
    {
        inputX = player.inputHandler.normInputX;
        inputY = player.inputHandler.normInputY;
        jumpInput = player.inputHandler.jumpInput;
        jumpInputActive = player.inputHandler.jumpInputActive;
        dodgeInput = player.inputHandler.dodgeInput;
        dodgeInputActive = player.inputHandler.dodgeInputActive;
        attackInput = player.inputHandler.attackInput;
        attackInputActive = player.inputHandler.attackInputActive;
        escapeInput = player.inputHandler.escapeInput;
        dashAttackInput = player.inputHandler.dashAttackInput;
        blockParryInput = player.inputHandler.blockParryInput;
    }

    protected void SetMovementVariables()
    {
        currentPosition = player.transform.position;
        currentVelocity = player.rigidBody.velocity;
        facingDirection = player.movement.facingDirection;
    }

    private void TickPublicTimers()
    {
        player.dodgeState.dodgeCoolDownTimer.Tick(player.detection.isGrounded());
        player.wallJumpState.preventInputXTimer.Tick();
        player.escapeState.escapeCoolDownTimer.Tick();
        player.dashAttackState.dashCoolDownTimer.Tick();
        player.shieldParryState.defendCoolDownTimer.Tick();
        player.moveState.dashInputTimer.Tick();
        player.meleeAttackState.attackComboResetTimer.Tick();
        player.wallSlideState.wallJumpAvailTimer.Tick();
    }
}
