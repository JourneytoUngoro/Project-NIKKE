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

        isPlayerInDetectionRange = enemy.detection.isPlayerInDetectionRange();
    }

    public override void Enter()
    {
        base.Enter();

        if ((enemy.detection.target != null && enemy.detection.target.transform.rotation.y != enemy.transform.rotation.y) || enemy.detection.target == null)
        {
            enemy.movement.Flip();
        }
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
                stateMachine.ChangeState(enemy.playerInDetectionRangeState);
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
            enemy.movement.SetVelocityZero();
        }
    }

    public void TurnBack()
    {
        currentTurnCount += 1;
        enemy.movement.Flip();
    }
}
