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

        isPlayerInDetectionRange = enemy.detection.isTargetInDetectionRange();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.movement.SetVelocityX(0.0f);
        if (isGrounded)
        {
            enemy.movement.SetVelocityY(0.0f);
        }
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
                stateMachine.ChangeState(enemy.targetInDetectionRangeState);
            }
            else if (GotHit())
            {
                stateMachine.ChangeState(enemy.lookForTargetState);
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
            RigidBodyController(true);

            if (isGrounded)
            {
                enemy.movement.SetVelocityX(0.0f);

                if (isOnSlope)
                {
                    enemy.movement.SetVelocityY(0.0f);
                }
            }
        }
    }
}
