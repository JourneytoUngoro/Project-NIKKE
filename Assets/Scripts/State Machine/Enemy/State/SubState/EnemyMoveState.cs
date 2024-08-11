using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static Unity.Burst.Intrinsics.Arm;

public class EnemyMoveState : EnemyState
{
    protected bool isPlayerInDetectionRange;

    public EnemyMoveState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        isAnimationActionTriggered = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInDetectionRange = enemy.detection.isTargetInDetectionRange();
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
                stateMachine.ChangeState(enemy.lookForTargetState);
            }
            else if (isPlayerInDetectionRange)
            {
                stateMachine.ChangeState(enemy.targetInDetectionRangeState);
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
            RigidBodyController(true);
        }
        /*if (!onStateExit)
        {
            if (isOnSlope)
            {
                workSpace.Set(enemy.detection.slopePerpNormal.x * facingDirection, enemy.detection.slopePerpNormal.y * facingDirection);
                enemy.movement.SetVelocity(workSpace * enemyData.moveSpeed);
            }
            else
            {
                enemy.movement.SetVelocityX(enemyData.moveSpeed * facingDirection);
            }
        }*/
    }
}
