using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyMoveState : EnemyState
{
    protected bool isPlayerInDetectionRange;

    public EnemyMoveState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();


    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInDetectionRange = enemy.detection.isPlayerInDetectionRange();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.movement.SetVelocityMultiplier(Vector2.one);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (GotHit())
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
            else if (isPlayerInDetectionRange)
            {
                stateMachine.ChangeState(enemy.playerInDetectionRangeState);
            }
            else if (isDetectingLedge)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
            else if (isDetectingWall)
            {
                enemy.movement.Flip();
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (isOnSlope)
            {
                if (enemy.detection.slopePerpNormal.y * facingDirection > 0)
                {
                    enemy.movement.SetVelocityMultiplier(Vector2.one * 0.8f);
                }
                else
                {
                    enemy.movement.SetVelocityMultiplier(Vector2.one * 1.4f);
                }
                
                workSpace.Set(enemy.detection.slopePerpNormal.x * facingDirection, enemy.detection.slopePerpNormal.y * facingDirection);
                
                enemy.movement.SetVelocity(workSpace * enemyData.moveSpeed);
            }
            else
            {
                enemy.movement.SetVelocityMultiplier(Vector2.one);

                enemy.movement.SetVelocityX(enemyData.moveSpeed * facingDirection);
            }
        }
    }
}
