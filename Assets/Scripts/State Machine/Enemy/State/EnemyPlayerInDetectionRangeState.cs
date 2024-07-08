using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerInDetectionRangeState : EnemyState
{
    protected bool isPlayerInRangedAttackRange;
    protected bool isPlayerInMeleeAttackRange;
    protected bool isPlayerInAggroRange;

    public EnemyPlayerInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInRangedAttackRange = enemy.detection.isPlayerInRangedAttackRange();
        isPlayerInMeleeAttackRange = enemy.detection.isPlayerInMeleeAttackRange();
        isPlayerInAggroRange = enemy.detection.isPlayerInAggroRange();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.movement.SetVelocityX(0.0f);
        // TODO: Alert adjacent enemies
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isPlayerInMeleeAttackRange)
        {
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if (isPlayerInRangedAttackRange)
        {
            stateMachine.ChangeState(enemy.rangedAttackState);
        }
        else if (isPlayerInAggroRange)
        {
            stateMachine.ChangeState(enemy.playerInAggroRangeState);
        }
        else
        {
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
