using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGotHitState : EnemyState
{
    public Timer gotHitTimer { get; private set; }
    private bool canTransit;

    public EnemyGotHitState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        gotHitTimer = new Timer(0.0f);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = true;
        gotHitTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LateLogicUpdate()
    {
        base.LateLogicUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            gotHitTimer.Tick();

            if (canTransit)
            {
                if (enemy.enemyStateMachine.prevState != enemy.targetInAggroRangeState)
                {
                    if (isTargetInDetectionRange)
                    {
                        enemy.enemyStateMachine.ChangeState(enemy.targetInDetectionRangeState);
                    }
                    else
                    {
                        enemy.enemyStateMachine.ChangeState(enemy.lookForTargetState);
                    }
                }
                else
                {
                    if (isTargetInAggroRange)
                    {
                        enemy.enemyStateMachine.ChangeState(enemy.targetInDetectionRangeState);
                    }
                    else
                    {
                        enemy.enemyStateMachine.ChangeState(enemy.lookForTargetState);
                    }
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
