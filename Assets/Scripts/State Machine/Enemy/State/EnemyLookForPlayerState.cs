using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyLookForPlayerState : EnemyState
{
    protected Timer turnTimer;

    protected bool isPlayerInAggroRange;
    protected int currentTurnCount;

    public EnemyLookForPlayerState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        turnTimer = new Timer(enemyData.timeDelayforEachTurn);
        turnTimer.timerAction += TurnBack;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInAggroRange = enemy.detection.isPlayerInAggroRange();
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.detection.target != null && enemy.detection.target.transform.rotation.y != enemy.transform.rotation.y)
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

        turnTimer.Tick();

        if (isPlayerInAggroRange)
        {
            stateMachine.ChangeState(enemy.playerInDetectionRangeState);
        }
        else if (currentTurnCount >= enemyData.totalTurnAmount)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void TurnBack()
    {
        currentTurnCount += 1;
        enemy.movement.Flip();
    }
}
