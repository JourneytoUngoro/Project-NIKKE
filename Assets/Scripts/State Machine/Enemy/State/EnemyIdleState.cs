using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    protected System.Random random;
    
    protected float waitForSeconds;
    protected bool isPlayerInDetectionRange;

    public EnemyIdleState(Enemy entity, string animBoolName) : base(entity, animBoolName)
    {
        random = new System.Random();
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
        waitForSeconds = (float)random.NextDouble() * (enemyData.maxWaitTime - enemyData.minWaitTime) + enemyData.minWaitTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isPlayerInDetectionRange)
            {
                stateMachine.ChangeState(enemy.playerInDetectionRangeState);
            }
            else if (GotHit())
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
            else if (Time.time - startTime >= waitForSeconds)
            {
                enemy.movement.Flip();
                stateMachine.ChangeState(enemy.moveState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            enemy.movement.SetVelocityZero();
        }
    }
}
