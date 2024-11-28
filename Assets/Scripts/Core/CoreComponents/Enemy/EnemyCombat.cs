using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : Combat
{
    public int currentMeleeAttackStroke { get; private set; }
    public int currentRangedAttackStroke { get; private set; }
    public int currentMidRangedAttackStroke { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> midRangedAttacks { get; protected set; }

    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
    }

    public bool isTargetInMeleeAttackRange()
    {
        bool targetInMeleeAttackRange = false;

        foreach (OverlapCollider overlapCollider in meleeAttacks[enemy.meleeAttackState.currentAttackStroke].overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                targetInMeleeAttackRange |= Physics2D.OverlapBox(overlapCollider.centerTransform.position, overlapCollider.boxSize, 0.0f, whatIsDamageable);
            }
            else if (overlapCollider.overlapCircle)
            {
                targetInMeleeAttackRange |= Physics2D.OverlapCircle(overlapCollider.centerTransform.position, overlapCollider.circleRadius, whatIsDamageable);
            }
        }

        return targetInMeleeAttackRange;
    }

    public bool isTargetInMidRAttackRange()
    {
        bool targetInMidRangeAttackRange = false;

        foreach (OverlapCollider overlapCollider in midRangedAttacks[enemy.meleeAttackState.currentAttackStroke].overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                targetInMidRangeAttackRange |= Physics2D.OverlapBox(overlapCollider.centerTransform.position, overlapCollider.boxSize, 0.0f, whatIsDamageable);
            }
            else if (overlapCollider.overlapCircle)
            {
                targetInMidRangeAttackRange |= Physics2D.OverlapCircle(overlapCollider.centerTransform.position, overlapCollider.circleRadius, whatIsDamageable);
            }
        }

        return targetInMidRangeAttackRange;
    }

    public bool isTargetInRangedAttackRange()
    {
        bool targetInRangedAttackRange = false;

        foreach (OverlapCollider overlapCollider in rangedAttacks[enemy.meleeAttackState.currentAttackStroke].overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                targetInRangedAttackRange |= Physics2D.OverlapBox(overlapCollider.centerTransform.position, overlapCollider.boxSize, 0.0f, whatIsDamageable);
            }
            else if (overlapCollider.overlapCircle)
            {
                targetInRangedAttackRange |= Physics2D.OverlapCircle(overlapCollider.centerTransform.position, overlapCollider.circleRadius, whatIsDamageable);
            }
        }

        return targetInRangedAttackRange;
    }

    public void FireProjectile(string objectName, Vector2 projectileFirePosition, Vector2? targetPosition, float projectileSpeed, float projectileGravityScale)
    {
        GameObject projectile = Manager.Instance.objectPoolingManager.GetGameObject(objectName);
        // projectile.GetComponent<Explosion>().SetAttackSubject(gameObject);
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();

        if (projectile == null)
        {
            Debug.LogError($"Can't find projectile name: {objectName}");
            return;
        }
        
        if (targetPosition.HasValue)
        {
            Vector2? projectileAngle = CalculateProjectileAngle(projectileFirePosition, targetPosition.Value, projectileSpeed, projectileGravityScale);

            if (projectileAngle.HasValue)
            {
                projectileRigidbody.velocity = projectileAngle.Value * projectileSpeed;
            }
            else
            {
                Manager.Instance.objectPoolingManager.ReleaseGameObject(projectile);
            }
        }
        else
        {
            projectile.transform.position = projectileFirePosition;
            projectile.transform.rotation = enemy.movement.facingDirection == 1 ? Quaternion.Euler(0.0f, 0.0f, 0.0f) : Quaternion.Euler(0.0f, -180.0f, 0.0f);
            if (projectileRigidbody != null)
            {
                projectileRigidbody.velocity = projectile.transform.right * projectileSpeed;
                projectileRigidbody.gravityScale = projectileGravityScale;
            }
        }
    }

    public override void GetHealthDamage(DamageComponent damageComponent)
    {

    }

    public override void GetPostureDamage(DamageComponent damageComponent)
    {
        throw new NotImplementedException();
    }

    public override void GetKnockback(KnockbackComponent knockbackComponent)
    {
        throw new NotImplementedException();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;

        foreach (CombatAbilityWithTransforms combatAbilityWithTransform in midRangedAttacks)
        {
            foreach (OverlapCollider overlapCollider in combatAbilityWithTransform.overlapColliders)
            {
                if (overlapCollider.overlapCircle)
                {
                    Gizmos.DrawWireSphere(overlapCollider.centerTransform.position, overlapCollider.circleRadius);
                }
                else if (overlapCollider.overlapBox)
                {
                    Gizmos.DrawWireCube(overlapCollider.centerTransform.position, overlapCollider.boxSize);
                }
            }
        }
    }
}
