using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttackState : EnemyState
{
    protected bool isPlayerInMeleeAttackRange;
    protected bool isPlayerInRangedAttackRange;
    protected bool isPlayerInAggroRange;

    public EnemyMeleeAttackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMeleeAttackRange = enemy.detection.isPlayerInMeleeAttackRange();
        isPlayerInRangedAttackRange = enemy.detection.isPlayerInRangedAttackRange();
        isPlayerInAggroRange = enemy.detection.isPlayerInAggroRange();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.movement.SetVelocityX(0.0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(enemy.playerInAggroRangeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
