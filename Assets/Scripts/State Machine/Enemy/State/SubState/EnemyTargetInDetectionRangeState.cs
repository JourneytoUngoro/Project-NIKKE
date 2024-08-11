using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetInDetectionRangeState : EnemyState
{
    protected bool isPlayerInRangedAttackRange;
    protected bool isPlayerInMidRAttackRange;
    protected bool isPlayerInMeleeAttackRange;
    protected bool isPlayerInAggroRange;

    public EnemyTargetInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        enemy.detection.DoAlert();
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInRangedAttackRange = enemy.combat.isTargetInRangedAttackRange();
        isPlayerInMidRAttackRange = enemy.combat.isTargetInMidRAttackRange();
        isPlayerInMeleeAttackRange = enemy.combat.isTargetInMeleeAttackRange();
        isPlayerInAggroRange = enemy.detection.isPlayerInAggroRange();
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = !enemyData.canAlert;
        enemy.movement.SetVelocityX(0.0f);
        
        if (enemyData.canAlert)
        {
            enemy.detection.DoAlert();
            enemy.animator.SetBool("move", false);
            enemy.animator.SetBool("alert", true);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (isAbilityDone)
        {
            if (enemyData.canMeleeAttack && enemy.meleeAttackState.canAttack && isPlayerInMeleeAttackRange)
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
            else if (enemyData.canMidRAttack && enemy.midRAttackState.canAttack && isPlayerInMidRAttackRange)
            {
                stateMachine.ChangeState(enemy.midRAttackState);
            }
            else if (enemyData.canRangedAttack && enemy.rangedAttackState.canAttack && isPlayerInRangedAttackRange)
            {
                stateMachine.ChangeState(enemy.rangedAttackState);
            }
            else if (isPlayerInAggroRange)
            {
                stateMachine.ChangeState(enemy.targetInAggroRangeState);
            }
            else
            {
                stateMachine.ChangeState(enemy.lookForTargetState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
