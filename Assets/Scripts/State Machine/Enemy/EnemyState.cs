using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class EnemyState : State
{
    #region Components
    protected Enemy enemy;
    protected EnemyStateMachine stateMachine;
    protected EnemyData enemyData;
    protected string animBoolName;
    #endregion

    #region Other Variables
    protected bool isAbilityDone;

    public bool gotHit;
    #endregion

    #region Shared Detection
    protected bool isGrounded;
    protected bool isDetectingLedge;
    protected bool isDetectingWall;
    protected bool isOnSlope;
    #endregion

    public EnemyState(Enemy enemy, string animBoolName)
    {
        this.enemy = enemy;
        this.stateMachine = enemy.enemyStateMachine;
        this.enemyData = enemy.enemyData;
        this.animBoolName = animBoolName;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isOnSlope = enemy.detection.isOnSlope();
        isGrounded = enemy.detection.isGrounded();
        isDetectingLedge = enemy.detection.isDetectingLedge();
        isDetectingWall = enemy.detection.isDetectingWall();
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
        enemy.animator.SetBool(animBoolName, true);
        SetMovementVariables();

        DoChecks();
    }

    public virtual void Exit()
    {
        enemy.animator.SetBool(animBoolName, false);
        onStateExit = true;
    }

    public virtual void LogicUpdate()
    {
        TickPublicTimers();
    }

    public virtual void LateLogicUpdate()
    {
        gotHit = false;
    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();

        if (!isGrounded)
        {
            // enemy.rigidBody.gravityScale = 9.5f;
        }

        SetMovementVariables();
    }

    protected void SetMovementVariables()
    {
        currentPosition = enemy.transform.position;
        currentVelocity = enemy.rigidBody.velocity;
        facingDirection = enemy.movement.facingDirection;
    }

    private void TickPublicTimers()
    {
        enemy.teleportState.teleportCoolDownTimer.Tick();
        enemy.meleeAttackState.meleeAttackCoolDownTimer.Tick();
        enemy.rangedAttackState.rangedAttackCoolDownTimer.Tick();
    }

    protected bool GotHit()
    {
        if (gotHit)
        {
            gotHit = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
