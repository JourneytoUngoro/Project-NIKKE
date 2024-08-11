using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunnedState : EnemyState
{
    private Timer stunnedTimer;

    private bool canTransit;

    protected float elapsedTime;

    protected bool isTargetInDetectionRange;

    public EnemyStunnedState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        stunnedTimer = new Timer(enemyData.stunnedTime);
        stunnedTimer.timerAction += () => { canTransit = true; };
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInDetectionRange = enemy.detection.isTargetInDetectionRange();
    }

    public override void Enter()
    {
        base.Enter();

        elapsedTime = 0.0f;
        stunnedTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        canTransit = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            stunnedTimer.Tick();

            if (canTransit)
            {
                if (isTargetInDetectionRange)
                {
                    stateMachine.ChangeState(enemy.targetInAggroRangeState);
                }
                else
                {
                    stateMachine.ChangeState(enemy.lookForTargetState);
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

            elapsedTime += Time.deltaTime;

            float velocityMultiplierOverTime = Mathf.Clamp(1.0f - DOVirtual.EasedValue(0.0f, 1.0f, elapsedTime / 0.2f, Ease.InCubic), 0.0f, 1.0f);

            if (isGrounded)
            {
                if (isOnSlope)
                {
                    workSpace.Set(enemy.detection.slopePerpNormal.x * -facingDirection, enemy.detection.slopePerpNormal.y * -facingDirection);
                    enemy.movement.SetVelocity(workSpace * velocityMultiplierOverTime * enemyData.stunnedKnockbackSpeed);
                }
                else
                {
                    enemy.movement.SetVelocityX(velocityMultiplierOverTime * -facingDirection * enemyData.stunnedKnockbackSpeed);
                }
            }
            else
            {
                enemy.movement.SetVelocityX(velocityMultiplierOverTime * -facingDirection * enemyData.stunnedKnockbackSpeed);
            }
        }
    }
}
