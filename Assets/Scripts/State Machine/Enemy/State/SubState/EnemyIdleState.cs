using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class EnemyIdleState : EnemyState
{
    protected float waitForSeconds;

    public EnemyIdleState(Enemy entity, string animBoolName) : base(entity, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.movement.SetVelocityX(0.0f);
        if (isGrounded)
        {
            enemy.movement.SetVelocityY(0.0f);
        }

        waitForSeconds = UtilityFunctions.RandomFloat(enemyData.minWaitTime, enemyData.maxWaitTime);
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
            if (isTargetInDetectionRange && enemy.detection.currentPlatform == enemy.detection.currentTarget.entityDetection.currentPlatform)
            {
                stateMachine.ChangeState(enemy.targetInDetectionRangeState);
            }
            else if (enemy.got[(int)GotConditions.Hit])
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
