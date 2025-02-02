using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;

public class NeonChargeAttackState : EnemyAttackState
{
    private Neon neon;
    private Timer chargeAttackTimer;

    public NeonChargeAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        neon = enemy as Neon;
        float chargeTime = (neon.neonCombat.chargeAttack[0].combatAbilityData.combatAbilityComponents.FirstOrDefault(combatAbilityComponent => combatAbilityComponent.GetType().Equals(typeof(ReboundComponent))) as ReboundComponent).onGroundReboundTime;
        chargeAttackTimer = new Timer(chargeTime);
        chargeAttackTimer.timerAction += () => { neon.animator.SetBool("chargeAttackFinish", true); };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        neon.combat.DoAttack(neon.neonCombat.chargeAttack[0]);
        chargeAttackTimer.StartSingleUseTimer();
    }

    public override void Enter()
    {
        base.Enter();

        neon.movement.SetVelocityX(0.0f);
    }

    public override void Exit()
    {
        base.Exit();

        neon.animator.SetBool("chargeAttackFinish", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        afterImageTimer.Tick();
        chargeAttackTimer.Tick();

        if (isAnimationActionTriggered)
        {
            neon.combat.DoAttack(neon.neonCombat.chargeAttack[1]);
        }

        if (isAbilityDone)
        {
            if (isTargetInAggroRange)
            {
                neon.enemyStateMachine.ChangeState(neon.targetInAggroRangeState);
            }
            else
            {
                neon.enemyStateMachine.ChangeState(neon.lookForTargetState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (isDetectingLedgeFront)
                {
                    isAbilityDone = true;
                }

                if (isOnSlope)
                {
                    enemy.rigidBody.gravityScale = 0.0f;

                    if (enemy.detection.slopePerpNormal.y * facingDirection > 0)
                    {
                        enemy.movement.SetVelocityMultiplier(Vector2.one * 0.8f);
                    }
                    else
                    {
                        enemy.movement.SetVelocityMultiplier(Vector2.one * 1.4f);
                    }
                }
                else
                {
                    enemy.movement.SetVelocityMultiplier(Vector2.one);
                    enemy.rigidBody.gravityScale = 9.5f;
                    enemy.movement.SetVelocityLimitY(0.0f);
                }
            }
            else
            {
                enemy.movement.SetVelocityMultiplier(Vector2.one);
                enemy.rigidBody.gravityScale = 9.5f;
            }
        }
    }
}
