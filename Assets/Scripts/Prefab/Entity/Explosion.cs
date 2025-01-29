using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : PooledObject
{
    [SerializeField] private Transform knockbackSourceTransform;
    [SerializeField] private LayerMask whatIsDamageable;
    [field: SerializeField] public List<CombatAbilityWithTransforms> explosionAreas { get; private set; }

    private List<Collider2D> damagedTargets = new List<Collider2D>();

    public Entity sourceEntity { get; protected set; }
    public Projectile sourceProjectile { get; protected set; }

    protected float epsilon = 0.001f;

    private void Awake()
    {
        foreach (CombatAbilityWithTransforms combatAbilityWithTransforms in explosionAreas)
        {
            CombatAbility combatAbility = combatAbilityWithTransforms.combatAbilityData;

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbility.combatAbilityComponents)
            {
                combatAbilityComponent.pertainedCombatAbility = combatAbility;

                if (combatAbilityComponent.GetType().Equals(typeof(KnockbackComponent)))
                {
                    KnockbackComponent knockbackComponent = combatAbilityComponent as KnockbackComponent;
                    knockbackComponent.knockbackSourceTransform = knockbackSourceTransform ? knockbackSourceTransform : transform;
                }
            }
        }
    }

    public override void ReleaseObject()
    {
        base.ReleaseObject();

        sourceEntity = null;
        sourceProjectile = null;
        damagedTargets.Clear();
    }

    public void SetExplosion(Entity sourceEntity)
    {
        this.sourceEntity = sourceEntity;
    }

    public void SetExplosion(Projectile sourceProjectile)
    {
        this.sourceProjectile = sourceProjectile;
    }

    public void Explode(int index)
    {
        Collider2D[] damageTargets = new Collider2D[0];

        CombatAbilityWithTransforms combatAbilityWithTransforms = explosionAreas[index];

        foreach (OverlapCollider overlapCollider in combatAbilityWithTransforms.overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                damageTargets = damageTargets.Union(Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, overlapCollider.boxRotation, whatIsDamageable)).ToArray();
            }
            else if (overlapCollider.overlapCircle)
            {
                damageTargets = damageTargets.Union(Physics2D.OverlapCircleAll(overlapCollider.centerTransform.position, overlapCollider.circleRadius, whatIsDamageable)).ToArray();
            }
        }

        foreach (Collider2D damageTarget in damageTargets)
        {
            if (damagedTargets.Contains(damageTarget)) continue;

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransforms.combatAbilityData.combatAbilityComponents)
            {
                switch (combatAbilityComponent)
                {
                    case DamageComponent damageComponent:
                        damageComponent.ApplyCombatAbility(damageTarget, combatAbilityWithTransforms.overlapColliders);
                        break;
                    case KnockbackComponent knockbackComponent:
                        knockbackComponent.ApplyCombatAbility(damageTarget, combatAbilityWithTransforms.overlapColliders);
                        break;
                    case StatusEffectComponent statusEffectComponent:
                        statusEffectComponent.ApplyCombatAbility(damageTarget, combatAbilityWithTransforms.overlapColliders);
                        break;
                }
            }

            damagedTargets.Add(damageTarget);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (CombatAbilityWithTransforms combatAbilityWithTransforms in explosionAreas)
        {
            if (combatAbilityWithTransforms.visualize)
            {
                foreach (OverlapCollider overlapCollider in combatAbilityWithTransforms.overlapColliders)
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
    }
}
