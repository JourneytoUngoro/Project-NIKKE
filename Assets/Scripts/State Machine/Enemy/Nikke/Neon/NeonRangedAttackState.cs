using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonRangedAttackState : EnemyAttackState
{
    private Neon neon;
    private NeonData neonData;

    private float elapsedTime;
    private int actionTriggerCalled;

    public NeonRangedAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        neon = enemy as Neon;
        neonData = enemyData as NeonData;
    }

    public override void AnimationActionTrigger(int index)
    {
        switch (actionTriggerCalled)
        {
            case 0:
                base.AnimationActionTrigger(index); break;
            case 1:
                neon.animator.SetBool("rangedAttackFinish", true); isAnimationActionTriggered = false; break;
            default:
                break;
        }
        actionTriggerCalled += 1;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        elapsedTime = 0.0f;
        actionTriggerCalled = 0;
    }

    public override void Exit()
    {
        base.Exit();

        neon.animator.SetBool("rangedAttackFinish", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        afterImageTimer.Tick();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(true);

            neon.movement.SetVelocityMultiplier(Vector2.one);

            if (isAnimationActionTriggered)
            {
                elapsedTime += Time.deltaTime;
                
                float velocityMultiplierOverTime = Mathf.Clamp(1.0f - DOVirtual.EasedValue(0.0f, 1.0f, elapsedTime / neonData.rangedAttackChargeTime, Ease.InSine), 0.0f, 1.0f);

                if (!isDetectingLedge)
                {
                    if (isGrounded)
                    {
                        if (isOnSlope)
                        {
                            workSpace.Set(neon.detection.slopePerpNormal.x * facingDirection, neon.detection.slopePerpNormal.y * facingDirection);
                            neon.movement.SetVelocity(workSpace * velocityMultiplierOverTime * neonData.rangedAttackChargeSpeed);
                        }
                        else
                        {
                            neon.movement.SetVelocityX(velocityMultiplierOverTime * facingDirection * neonData.rangedAttackChargeSpeed);
                        }
                    }
                    else
                    {
                        neon.movement.SetVelocityX(velocityMultiplierOverTime * facingDirection * neonData.rangedAttackChargeSpeed);
                        neon.movement.SetVelocityLimitY(0.0f);
                    }
                }
                else
                {
                    if (isGrounded)
                    {
                        neon.movement.SetVelocityX(0.0f);
                    }
                    else
                    {
                        neon.movement.SetVelocityX(velocityMultiplierOverTime * facingDirection * neonData.rangedAttackChargeSpeed);
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
}
