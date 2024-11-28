using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

/* Incomplete */
/* Needs to be optimized and improved */

public abstract class Combat : CoreComponent
{
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsDamageable;
    [SerializeField] protected LayerMask parryLayer;
    [SerializeField] protected LayerMask shieldLayer;

    [field: SerializeField] public CombatAbilityWithTransforms shieldParry { get; protected set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> meleeAttacks { get; protected set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> rangedAttacks { get; protected set; }

    protected List<Collider2D> damagedTargets;

    protected override void Awake()
    {
        base.Awake();

        damagedTargets = new List<Collider2D>();

        var combatAbilityFields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.FieldType == typeof(List<CombatAbilityWithTransforms>));

        foreach (var field in combatAbilityFields)
        {
            var combatAbilityList = field.GetValue(this) as List<CombatAbilityWithTransforms>;

            foreach (CombatAbilityWithTransforms combatAbilityWithTransform in combatAbilityList)
            {
                combatAbilityWithTransform.combatAbilityData.entity = entity;

                foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransform.combatAbilityData.combatAbilityComponents)
                {
                    combatAbilityComponent.entity = entity;
                }
            }
        }
    }

    public void GetDamage(DamageComponent damageComponent)
    {
        GetHealthDamage(damageComponent);
        GetPostureDamage(damageComponent);
    }

    public abstract void GetHealthDamage(DamageComponent damageComponent);

    public abstract void GetPostureDamage(DamageComponent damageComponent);

    /// <summary>
    /// Knockback time of 0 means that the knockback will be done when the entity hits the ground.
    /// </summary>
    public virtual void GetKnockback(KnockbackComponent knockbackComponent)
    {
        
    }

    public virtual bool DoAttack(CombatAbilityWithTransforms combatAbilityWithTransforms)
    {
        bool foundTarget = false;

        List<Collider2D> damageTargets = new List<Collider2D>();

        foreach (OverlapCollider overlapCollider in combatAbilityWithTransforms.overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                damageTargets.Union(Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, 0.0f, whatIsDamageable)).ToList();
            }
            else if (overlapCollider.overlapCircle)
            {
                damageTargets.Union(Physics2D.OverlapCircleAll(overlapCollider.centerTransform.position, overlapCollider.circleRadius, whatIsDamageable)).ToList();
            }
        }

        damageTargets.Remove(entity.entityCollider);

        foundTarget = damageTargets.Count() > 0;

        foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransforms.combatAbilityData.combatAbilityComponents)
        {
            switch (combatAbilityComponent)
            {
                case ReboundComponent reboundComponent:
                    reboundComponent.ApplyCombatAbility();
                    break;
                case DodgeComponent dodgeComponent:
                    dodgeComponent.ApplyCombatAbility();
                    break;
                case ShieldParryComponent shieldParryComponent:
                    shieldParryComponent.ApplyCombatAbility(combatAbilityWithTransforms.overlapColliders);
                    break;
                case ProjectileComponent projectileComponent:
                    projectileComponent.ApplyCombatAbility(damageTargets, combatAbilityWithTransforms.overlapColliders);
                    break;
            }
        }

        foreach (Collider2D damageTarget in damageTargets)
        {
            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransforms.combatAbilityData.combatAbilityComponents)
            {
                switch (combatAbilityComponent)
                {
                    case DamageComponent damageComponent:
                        damageComponent.ApplyCombatAbility(damageTarget);
                        break;
                    case KnockbackComponent knockbackComponent:
                        knockbackComponent.ApplyCombatAbility(damageTarget);
                        break;
                    case StatusEffectComponent statusEffectComponent:
                        statusEffectComponent.ApplyCombatAbility(damageTarget);
                        break;
                }
            }
        }

        return foundTarget;
    }

    public bool CheckWithinAngle(Vector2 baseVector, Vector2 targetVector, float counterClockwiseAngle, float clockwiseAngle)
    {
        float angleBetweenVectors = -Vector2.SignedAngle(baseVector, targetVector);
        return -counterClockwiseAngle <= angleBetweenVectors && angleBetweenVectors <= clockwiseAngle;
    }

    public void ClearDamagedTargets()
    {
        damagedTargets.Clear();
    }

    public Vector2? CalculateProjectileAngle(Vector2 projectileFirePosition, Vector2 targetPosition, float projectileSpeed, float projectileGravityScale)
    {
        float distance = Vector2.Distance(projectileFirePosition, targetPosition);
        float gravityAccelaration = Mathf.Abs(Physics2D.gravity.y * projectileGravityScale);
        float xDifference = targetPosition.x - projectileFirePosition.x;
        float yDifference = targetPosition.y - projectileFirePosition.y;
        float speedSquare = Mathf.Pow(projectileSpeed, 2);
        float rootUnderValue = Mathf.Pow(speedSquare, 2) - gravityAccelaration * (gravityAccelaration * Mathf.Pow(xDifference, 2) + 2 * yDifference * speedSquare);
        if (rootUnderValue >= 0.0f)
        {
            float lowAngle = Mathf.Atan2(speedSquare - Mathf.Sqrt(rootUnderValue), gravityAccelaration * xDifference);
            float highAngle = Mathf.Atan2(speedSquare + Mathf.Sqrt(rootUnderValue), gravityAccelaration * xDifference);
            Vector2 highAngleVector = new Vector2(Mathf.Cos(highAngle), Mathf.Sin(highAngle));
            Vector2 lowAngleVector = new Vector2(Mathf.Cos(lowAngle), Mathf.Sin(lowAngle));

            if (CheckProjectileRoute(projectileFirePosition, targetPosition, projectileSpeed * lowAngleVector))
            {
                return lowAngleVector;
            }
            else
            {
                if (CheckProjectileRoute(projectileFirePosition, targetPosition, projectileSpeed * highAngleVector))
                {
                    return highAngleVector;
                }
                else
                {
                    return null;
                }
            }
        }
        else return null;
    }

    private bool CheckProjectileRoute(Vector2 projectileFirePosition, Vector2 targetPosition, Vector2 projectileVelocity)
    {
        float distance = Vector2.Distance(projectileFirePosition, targetPosition);
        float xDifference = targetPosition.x - projectileFirePosition.x;
        float expectedTravelDuration = xDifference / projectileVelocity.x;
        float timeStep = expectedTravelDuration / 20.0f;

        Vector2 prevPosition;
        Vector2 currentPosition = projectileFirePosition;
        for (int i = 0; i < 20; i++)
        {
            float timeElapsed = timeStep * i;
            Vector2 movementVector = new Vector2(projectileVelocity.x * timeElapsed, projectileVelocity.y * timeElapsed + 0.5f * Physics2D.gravity.y * Mathf.Pow(timeElapsed, 2));
            prevPosition = currentPosition;
            currentPosition = movementVector + projectileFirePosition;

            if (Physics2D.Raycast(prevPosition, currentPosition - prevPosition, Vector2.Distance(currentPosition, prevPosition), whatIsGround))
            {
                if (Vector2.Distance(currentPosition, targetPosition) < distance * 0.2f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (CombatAbilityWithTransforms combatAbilityWithTransform in meleeAttacks)
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
        foreach (CombatAbilityWithTransforms combatAbilityWithTransform in rangedAttacks)
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