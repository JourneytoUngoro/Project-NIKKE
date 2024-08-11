using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonTargetInAggroRangeState : EnemyTargetInAggroRangeState
{
    private Neon neon;
    private NeonData neonData;

    private float elapsedTime;

    public NeonTargetInAggroRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        neon = enemy as Neon;
        neonData = enemyData as NeonData;
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        elapsedTime = 0.0f;
        isAnimationFinished = false;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        isAnimationActionTriggered = true;
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAnimationActionTriggered = false;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        neon.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (isAnimationFinished)
            {
                if ((enemy.detection.target.transform.position.x - enemy.rigidBody.position.x) * facingDirection < 0)
                {
                    enemy.movement.Flip();
                }
            }

            RigidBodyController(true);

            if (isAnimationActionTriggered)
            {
                elapsedTime += Time.deltaTime;

                if (!isDetectingLedge)
                {
                    float velocityMultiplierOverTime = Mathf.Clamp(1.0f - DOVirtual.EasedValue(0.0f, 1.0f, elapsedTime / 0.2f, Ease.InQuint), 0.0f, 1.0f);

                    if (isGrounded)
                    {
                        if (isOnSlope)
                        {
                            workSpace.Set(enemy.detection.slopePerpNormal.x * facingDirection, enemy.detection.slopePerpNormal.y * facingDirection);
                            enemy.movement.SetVelocity(workSpace * velocityMultiplierOverTime * neonData.moveSpeed);
                        }
                        else
                        {
                            enemy.movement.SetVelocityX(facingDirection * velocityMultiplierOverTime * neonData.moveSpeed);
                            enemy.movement.SetVelocityLimitY(0.0f);
                        }
                    }
                    else
                    {
                        enemy.movement.SetVelocityX(facingDirection * velocityMultiplierOverTime * neonData.moveSpeed);
                    }
                }
                else
                {
                    neon.movement.SetVelocityX(0.0f);
                }
            }
        }
    }
}
