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

    protected bool jumpInput { get; set; }
    protected bool jumpInputActive { get; set; }

    protected bool dodgeInput;
    protected bool dodgeInputPressed;

    protected bool attackInput;
    protected bool attackInputActive;
    protected bool rangedAttackInputPressed;
    protected bool rangedAttackInputActive;

    protected bool escapeInputPressed;

    protected bool skillAttackInput;

    protected bool shieldParryInput;

    protected bool cureInputPressed;
    #endregion

    #region Other Variables
    protected bool canTransit;
    #endregion

    public PlayerState(Player player, string animBoolName)
    {
        this.player = player;
        this.stateMachine = player.playerStateMachine;
        this.playerData = player.playerData;
        this.animBoolName = animBoolName;
        player.movement.synchronizeValues += SetMovementVariables;
        afterImageTimer = new Timer(0.1f);
        afterImageTimer.timerAction += () => { player.UseAfterImage(new Color(1.0f, 0.2f, 1.0f)); };
        afterImageTimer.StartMultiUseTimer();
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
        player.movement.SetVelocityMultiplier(Vector2.one);

        if (stateMachine.nextState != player.moveState && stateMachine.nextState != player.idleState && stateMachine.nextState != player.jumpState && stateMachine.nextState != player.inAirState)
        {
            player.moveState.PreventDash();
        }
    }

    public virtual void LogicUpdate()
    {
        TickPublicTimers();
    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
        SetInputVariables();
        SetMovementVariables();
    }

    public virtual void LateLogicUpdate()
    {
        
    }

    protected void SetInputVariables()
    {
        inputX = player.inputHandler.normInputX;
        inputY = player.inputHandler.normInputY;
        jumpInput = player.inputHandler.jumpInput;
        jumpInputActive = player.inputHandler.jumpInputActive;
        dodgeInputPressed = player.inputHandler.dodgeInputPressed;
        attackInput = player.inputHandler.attackInput;
        attackInputActive = player.inputHandler.attackInputActive;
        rangedAttackInputPressed = player.inputHandler.rangedAttackInputPressed;
        rangedAttackInputActive = player.inputHandler.rangedAttackInputActive;
        escapeInputPressed = player.inputHandler.escapeInputPressed;
        skillAttackInput = player.inputHandler.skillAttackInput;
        shieldParryInput = player.inputHandler.shieldParryInput;
        cureInputPressed = player.inputHandler.cureInputPressed;
    }

    protected void SetMovementVariables()
    {
        currentPosition = player.transform.position;
        currentVelocity = player.rigidBody.velocity;
        facingDirection = player.movement.facingDirection;
    }

    private void TickPublicTimers()
    {
        afterImageTimer.Tick(player.moveState.isDashing, true);
        player.dodgeState.dodgeCoolDownTimer.Tick(player.detection.isGrounded());
        player.wallJumpState.preventInputXTimer.Tick();
        player.escapeState.escapeCoolDownTimer.Tick();
        player.dashAttackState.dashCoolDownTimer.Tick();
        player.shieldParryState.shieldParryCoolDownTimer.Tick();
        player.shieldParryState.shieldParryInAirCoolDownTimer.Tick();
        player.moveState.dashInputTimer.Tick();
        player.meleeAttackState.attackComboResetTimer.Tick();
        player.wallSlideState.wallJumpAvailTimer.Tick();
    }
}
