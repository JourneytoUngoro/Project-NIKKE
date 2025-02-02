using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCombat : Combat
{
    [SerializeField] protected LayerMask whatIsTarget;
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
    }

    public bool IsTargetInRangeOf(CombatAbilityWithTransforms attack, bool chargeAttack = false)
    {
        if (enemy.detection.currentTarget == null) return false;

        if (chargeAttack)
        {
            ReboundComponent chargeComponent = attack.combatAbilityData.combatAbilityComponents.FirstOrDefault(combatAbilityComponent => combatAbilityComponent.GetType().Equals(typeof(ReboundComponent))) as ReboundComponent;

            if (chargeComponent != null)
            {
                if (Vector2.Distance(enemy.detection.currentTarget.transform.position, enemy.transform.position) < TrapezoidalRuleIntegral(chargeComponent.onGroundReboundTime, chargeComponent.onGroundReboundVelocity, chargeComponent.onGroundReboundEaseFunction))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            bool targetInRange = false;

            foreach (OverlapCollider overlapCollider in attack.overlapColliders)
            {
                if (overlapCollider.overlapBox)
                {
                    targetInRange = Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, overlapCollider.boxRotation, whatIsDamageable).Contains(enemy.detection.currentTarget.entityCollider);
                }
                else if (overlapCollider.overlapCircle)
                {
                    targetInRange = Physics2D.OverlapCircleAll(overlapCollider.centerTransform.position, overlapCollider.circleRadius, whatIsDamageable).Contains(enemy.detection.currentTarget.entityCollider);
                }

                if (targetInRange) break;
            }

            return targetInRange;
        }
    }

    float TrapezoidalRuleIntegral(float chargeTime, float chargeSpeed, Ease easeFunction)
    {
        float totalArea = 0f;
        float step = chargeTime / 100;

        if (easeFunction != Ease.Unset)
        {
            for (int i = 0; i < 100; i++)
            {
                float value1 = DOVirtual.EasedValue(0, chargeSpeed, i * step, easeFunction);
                float value2 = DOVirtual.EasedValue(0, chargeSpeed, (i + 1) * step, easeFunction);

                totalArea += ((value1 + value2) / 2.0f) * step;
            }
        }
        else
        {
            totalArea = chargeTime * chargeSpeed;
        }

        return totalArea;
    }

    protected override void ChangeToKnockbackState(KnockbackComponent knockbackComponent, bool isGrounded)
    {
        if (knockbackComponent.isKnockbackDifferentWhenAerial)
        {
            if (!isGrounded)
            {
                enemy.knockbackState.knockbackTimer.ChangeDuration(knockbackComponent.knockbackTimeWhenAerial);
            }
            else
            {
                enemy.knockbackState.knockbackTimer.ChangeDuration(knockbackComponent.knockbackTime);
            }
        }
        else
        {
            enemy.knockbackState.knockbackTimer.ChangeDuration(knockbackComponent.knockbackTime);
        }

        enemy.enemyStateMachine.ChangeState(enemy.knockbackState);
    }

    protected override void ChangeToKnockbackState(float knockbackTime)
    {
        enemy.knockbackState.knockbackTimer.ChangeDuration(knockbackTime);
        enemy.enemyStateMachine.ChangeState(enemy.knockbackState);
    }
}
