using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySleepState : EnemyState
{
    public bool isSleeping { get; private set; }

    protected bool isPlayerInDetectionRange;

    public EnemySleepState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInDetectionRange = enemy.detection.isPlayerInDetectionRange();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.stateMachineToAnimator.state = this;
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
            if (isSleeping)
            {
                if (isPlayerInDetectionRange || GotHit())
                {
                    WakeUp();
                }
            }
            else
            {
                if (isAbilityDone)
                {
                    if (isPlayerInDetectionRange)
                    {
                        stateMachine.ChangeState(enemy.playerInDetectionRangeState);
                    }
                    else
                    {
                        stateMachine.ChangeState(enemy.lookForPlayerState);
                    }
                }
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

    public void WakeUp()
    {
        isSleeping = false;
        enemy.animator.SetBool("sleep", false);
        enemy.animator.SetBool("wakeUp", true);
    }
}
