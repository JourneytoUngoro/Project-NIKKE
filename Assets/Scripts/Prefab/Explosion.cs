using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : PooledObject
{
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] protected CombatAbilityWithTransforms combatAbilityWithTransform;

    private List<Collider2D> damagedTargets;

    protected float epsilon = 0.001f;

    protected void OnEnable()
    {
        damagedTargets.Clear();
    }

    public override void ReleaseObject()
    {
        base.ReleaseObject();
        damagedTargets.Clear();
    }

    public void Explode()
    {
        Collider2D[] damageTargets = new Collider2D[0];

        foreach (OverlapCollider overlapCollider in combatAbilityWithTransform.overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                damageTargets.Union(Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, 0.0f, whatIsDamageable)).ToArray();
            }
            else if (overlapCollider.overlapCircle)
            {
                damageTargets.Union(Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, 0.0f, whatIsDamageable)).ToArray();
            }
        }

        foreach (Collider2D damageTarget in damageTargets)
        {
            if (damagedTargets.Contains(damageTarget)) continue;

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransform.combatAbilityData.combatAbilityComponents)
            {
                combatAbilityComponent.ApplyCombatAbility(damageTarget);
                damagedTargets.Add(damageTarget);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (OverlapCollider overlapCollider in combatAbilityWithTransform.overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                if (overlapCollider.centerTransform != null)
                {
                    Gizmos.DrawWireCube(overlapCollider.centerTransform.position, overlapCollider.boxSize);
                }
                else
                {
                    Gizmos.DrawWireCube(transform.position, overlapCollider.boxSize);
                }
            }
            else if (overlapCollider.overlapCircle)
            {
                if (overlapCollider.centerTransform != null)
                {
                    Gizmos.DrawWireSphere(overlapCollider.centerTransform.position, overlapCollider.circleRadius);
                }
                else
                {
                    Gizmos.DrawWireSphere(transform.position, overlapCollider.circleRadius);
                }
            }
        }
    }
}
