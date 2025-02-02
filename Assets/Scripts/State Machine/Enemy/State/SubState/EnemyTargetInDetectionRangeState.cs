using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetInDetectionRangeState : EnemyState
{
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
            if (isTargetInAggroRange)
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
