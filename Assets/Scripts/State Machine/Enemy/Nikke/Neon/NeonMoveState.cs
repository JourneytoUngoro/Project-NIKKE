using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonMoveState : EnemyMoveState
{
    private float elapsedTime;

    private Neon neon;
    private NeonData neonData;

    public NeonMoveState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
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
        if (isAnimationFinished)
        {
            base.LogicUpdate();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
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
                            enemy.movement.SetVelocity(workSpace * velocityMultiplierOverTime * enemyData.moveSpeed);
                        }
                        else
                        {
                            enemy.movement.SetVelocityX(facingDirection * velocityMultiplierOverTime * enemyData.moveSpeed);
                            enemy.movement.SetVelocityLimitY(0.0f);
                        }
                    }
                    else
                    {
                        enemy.movement.SetVelocityX(facingDirection * velocityMultiplierOverTime * enemyData.moveSpeed);
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
