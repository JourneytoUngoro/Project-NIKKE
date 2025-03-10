using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyLookForPlayerState : EnemyState
{
    protected Timer turnTimer;

    protected int currentTurnCount;
    protected bool isPlayerInDetectionRange;

    public EnemyLookForPlayerState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        turnTimer = new Timer(enemyData.timeDelayforEachTurn);
        turnTimer.timerAction += TurnBack;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInDetectionRange = enemy.detection.isTargetInDetectionRange();
    }

    public override void Enter()
    {
        base.Enter();

        /*if ((enemy.detection.currentTarget != null && enemy.detection.currentTarget.transform.rotation.y != enemy.transform.rotation.y))
        {
            enemy.movement.Flip();
        }*/
        TurnBack();
        turnTimer.StartMultiUseTimer(enemyData.totalTurnAmount);
    }

    public override void Exit()
    {
        currentTurnCount = 0;

        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            turnTimer.Tick();

            if (isPlayerInDetectionRange)
            {
                stateMachine.ChangeState(enemy.targetInDetectionRangeState);
            }
            else if (currentTurnCount >= enemyData.totalTurnAmount)
            {
                stateMachine.ChangeState(enemy.idleState);
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

    public void TurnBack()
    {
        currentTurnCount += 1;
        enemy.movement.Flip();
    }
}
