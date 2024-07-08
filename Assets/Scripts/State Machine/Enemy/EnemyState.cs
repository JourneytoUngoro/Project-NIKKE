using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    #region Components
    protected Enemy enemy;
    protected EnemyStateMachine stateMachine;
    protected EnemyData enemyData;
    protected string animBoolName;
    #endregion

    #region Other Variables
    protected bool onStateExit;
    protected bool isAnimationFinished;

    protected Vector2 currentPosition;
    protected Vector2 currentVelocity;

    protected int facingDirection;

    protected float epsilon = 0.001f;
    #endregion

    #region Shared Detection
    protected bool isGrounded;
    protected bool isDetectingLedge;
    protected bool isDetectingWall;
    protected bool isOnSlope;
    #endregion

    public float startTime;

    public EnemyState(Enemy enemy, string animBoolName)
    {
        this.enemy = enemy;
        this.stateMachine = enemy.enemyStateMachine;
        this.enemyData = enemy.enemyData;
        this.animBoolName = animBoolName;
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
        isOnSlope = enemy.detection.isOnSlope();
        isGrounded = enemy.detection.isGrounded();
        isDetectingLedge = enemy.detection.isDetectingLedge();
        isDetectingWall = enemy.detection.isDetectingWall();
    }

    public virtual void Enter()
    {
        startTime = Time.time;
        enemy.animator.SetBool(animBoolName, true);
        onStateExit = false;
        isAnimationFinished = false;
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
        SetMovementVariables();
        TickPublicTimers();
    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
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
    }
}
