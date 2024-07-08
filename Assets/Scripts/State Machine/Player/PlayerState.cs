using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
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

    protected bool escapeInput;

    protected bool dashAttackInput;

    protected bool blockParryInput;
    #endregion

    #region Other Variables
    protected bool onStateExit;
    protected bool isAnimationStarted;
    protected bool isAnimationActionTriggered;
    protected bool isAnimationFinished;

    protected Vector2 currentPosition;
    protected Vector2 currentVelocity;
    protected Vector2 workSpace;

    protected int facingDirection;

    protected float baseVelocity;
    protected float speedMultiplier;
    protected float epsilon = 0.001f;
    #endregion

    public float startTime { get; protected set; }

    public PlayerState(Player player, string animBoolName)
    {
        this.player = player;
        this.stateMachine = player.playerStateMachine;
        this.playerData = player.PlayerData;
        this.animBoolName = animBoolName;
        player.movement.synchronizeValues += SetMovementVariables;
    }

    public virtual void AnimationStartTrigger()
    {

    }

    public virtual void AnimationFinishTrigger()
    {
        isAnimationFinished = true;
    }

    public virtual void AnimationActionTrigger()
    {

    }

    public virtual void DoChecks()
    {

    }

    public virtual void Enter()
    {
        startTime = Time.time;
        player.animator.SetBool(animBoolName, true);
        onStateExit = false;
        isAnimationActionTriggered = false;
        isAnimationFinished = false;
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
        player.blockParryState.defendCoolDownTimer.Tick();
        player.moveState.dashInputTimer.Tick();
    }

    public void SetBaseVelocity(float baseVelocity)
    {
        this.baseVelocity = baseVelocity;
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        this.speedMultiplier = speedMultiplier;
    }
}
