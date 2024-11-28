using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public Timer attackCoolDownTimer;
    public bool isAttacking { get; private set; }
    public bool canAttack { get; private set; }
    public int currentAttackStroke { get; private set; }

    private float elapsedTime;

    public EnemyAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName)
    {
        canAttack = true;
        attackCoolDownTimer = new Timer(coolDown);
        attackCoolDownTimer.timerAction += () => { canAttack = true; };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        isAnimationActionTriggered = true;
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
        enemy.combat.ClearDamagedTargets();
        attackCoolDownTimer.StartSingleUseTimer();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        elapsedTime = 0.0f;
        canAttack = false;
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
            if (isAbilityDone)
            {
                stateMachine.ChangeState(enemy.targetInAggroRangeState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    protected void GetRecoil(float recoilTime, float recoilSpeed, bool useEaseOut = true)
    {
        elapsedTime += Time.fixedDeltaTime;

        float velocityMultiplierOverTime = useEaseOut ? Mathf.Clamp(1.0f - DOVirtual.EasedValue(0.0f, 1.0f, elapsedTime / recoilTime, Ease.InCubic), 0.0f, 1.0f) : 0.0f;

        if (elapsedTime < recoilTime)
        {
            if (!isDetectingLedge)
            {
                if (isGrounded)
                {
                    if (isOnSlope)
                    {
                        workSpace.Set(enemy.detection.slopePerpNormal.x * -facingDirection, enemy.detection.slopePerpNormal.y * -facingDirection);
                        enemy.movement.SetVelocity(workSpace * velocityMultiplierOverTime * recoilSpeed);
                    }
                    else
                    {
                        enemy.movement.SetVelocityX(velocityMultiplierOverTime * -facingDirection * recoilSpeed);
                    }
                }
                else
                {
                    enemy.movement.SetVelocityX(velocityMultiplierOverTime * -facingDirection * recoilSpeed);
                    enemy.movement.SetVelocityLimitY(0.0f);
                }
            }
            else
            {
                if (isGrounded)
                {
                    enemy.movement.SetVelocityX(0.0f);
                }
                else
                {
                    enemy.movement.SetVelocityX(velocityMultiplierOverTime * facingDirection * recoilSpeed);
                }
            }
        }
        else
        {
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
}
