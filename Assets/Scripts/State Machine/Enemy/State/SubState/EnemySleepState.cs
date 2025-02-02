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

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInDetectionRange = enemy.detection.isTargetInDetectionRange();
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
                if (isPlayerInDetectionRange || enemy.got[(int)GotConditions.Hit])
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
                        stateMachine.ChangeState(enemy.targetInDetectionRangeState);
                    }
                    else
                    {
                        stateMachine.ChangeState(enemy.lookForTargetState);
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

    public void WakeUp()
    {
        isSleeping = false;
        enemy.animator.SetBool("sleep", false);
        enemy.animator.SetBool("wakeUp", true);
    }
}
