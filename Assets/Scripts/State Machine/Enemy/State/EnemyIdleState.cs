using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    protected bool haveSeenPlayer;
    protected bool isPlayerInDetectionRange;

    public EnemyIdleState(Enemy entity, string animBoolName) : base(entity, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInDetectionRange = enemy.detection.isPlayerInDetectionRange();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.movement.SetVelocityZero();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isPlayerInDetectionRange)
        {
            haveSeenPlayer = true;
            stateMachine.ChangeState(enemy.playerInDetectionRangeState);
        }
        else if (haveSeenPlayer)
        {
            if (isDetectingWall || isDetectingLedge)
            {
                enemy.movement.Flip();
            }
            else
            {
                enemy.movement.SetVelocityX(enemy.movement.facingDirection * enemyData.moveSpeed);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
