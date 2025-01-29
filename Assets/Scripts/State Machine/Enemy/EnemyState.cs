using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    #endregion

    #region Shared Detection
    protected bool isGrounded;
    protected bool isDetectingLedgeFront;
    protected bool isDetectingLedgeBack;
    protected bool isDetectingWall;
    protected bool isOnSlope;
    protected bool isTargetInAggroRange;
    protected bool isTargetOnlyInAggroRange;
    protected bool isTargetInDetectionRange;
    #endregion

    public EnemyState(Enemy enemy, string animBoolName)
    {
        this.enemy = enemy;
        this.stateMachine = enemy.enemyStateMachine;
        this.enemyData = enemy.enemyData;
        this.animBoolName = animBoolName;
        enemy.movement.synchronizeValues += SetMovementVariables;
        afterImageTimer = new Timer(0.1f);
        afterImageTimer.timerAction += () => { enemy.UseAfterImage(new Color(0.5f, 0.5f, 1.0f)); };
        afterImageTimer.StartMultiUseTimer();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isOnSlope = enemy.detection.isOnSlope();
        isGrounded = enemy.detection.isGrounded();
        isDetectingWall = enemy.detection.isDetectingWall(CheckPositionHorizontal.Front, CheckPositionVertical.Both, CheckLogicalOperation.OR);
        isDetectingLedgeFront = enemy.detection.isDetectingLedgeFront();
        isDetectingLedgeBack = enemy.detection.isDetectingLedgeBack();
        
        isTargetInAggroRange = enemy.detection.isTargetInAggroRange(false);
        isTargetOnlyInAggroRange = enemy.detection.isTargetInAggroRange(true);
        isTargetInDetectionRange = enemy.detection.isTargetInDetectionRange();
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
        enemy.movement.SetVelocityMultiplier(Vector2.one);
        enemy.animator.SetBool(animBoolName, false);
        onStateExit = true;
    }

    public virtual void LogicUpdate()
    {
        TickPublicTimers();
    }

    public virtual void LateLogicUpdate()
    {
        
    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
        SetMovementVariables();
    }

    protected void SetMovementVariables()
    {
        currentPosition = enemy.transform.position;
        currentVelocity = enemy.rigidBody.velocity;
        facingDirection = enemy.movement.facingDirection;
    }

    protected virtual void TickPublicTimers()
    {
        // enemy.teleportState.teleportCoolDownTimer.Tick();
        enemy.shieldParryState.shieldCoolDownTimer.Tick();
        enemy.shieldParryState.parryCoolDownTimer.Tick();

        foreach (EnemyAttackState enemyAttackState in enemy.attackStates)
        {
            enemyAttackState.attackCoolDownTimer.Tick();
        }

        if (enemy.GetType().Equals(typeof(Neon)))
        {
            Neon neon = enemy as Neon;
            NeonTargetInAggroRangeState neonTargetInAggroRangeState = neon?.targetInAggroRangeState as NeonTargetInAggroRangeState;
            // neonTargetInAggroRangeState?.shieldCoolDownTimer.Tick();
        }
    }

    protected void RigidBodyController(bool isMovingForward = true, bool limitYVelocity = true)
    {
        if (isGrounded)
        {
            if (isOnSlope)
            {
                enemy.rigidBody.gravityScale = 0.0f;

                if (isMovingForward)
                {
                    if (enemy.detection.slopePerpNormal.y * facingDirection > 0)
                    {
                        enemy.movement.SetVelocityMultiplier(Vector2.one * 0.8f);
                    }
                    else
                    {
                        enemy.movement.SetVelocityMultiplier(Vector2.one * 1.4f);
                    }
                }
                else
                {
                    if (enemy.detection.slopePerpNormal.y * -facingDirection > 0)
                    {
                        enemy.movement.SetVelocityMultiplier(Vector2.one * 0.8f);
                    }
                    else
                    {
                        enemy.movement.SetVelocityMultiplier(Vector2.one * 1.4f);
                    }
                }
            }
            else
            {
                enemy.movement.SetVelocityMultiplier(Vector2.one);
                enemy.rigidBody.gravityScale = 9.5f;
                
                if (limitYVelocity)
                {
                    enemy.movement.SetVelocityLimitY(0.0f);
                }
            }
        }
        else
        {
            enemy.movement.SetVelocityMultiplier(Vector2.one);
            enemy.rigidBody.gravityScale = 9.5f;
        }
    }
}
