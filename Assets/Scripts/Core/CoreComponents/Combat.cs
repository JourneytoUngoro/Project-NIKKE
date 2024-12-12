using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using DG.Tweening;

/* Incomplete */
/* Needs to be optimized and improved */

public abstract class Combat : CoreComponent
{
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsDamageable;

    [field: SerializeField] public List<CombatAbilityWithTransforms> meleeAttacks { get; protected set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> rangedAttacks { get; protected set; }
    [field: SerializeField] public CombatAbilityWithTransforms parryArea { get; protected set; }
    [field: SerializeField] public Transform parryAreaTransform { get; protected set; }

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
                combatAbilityWithTransform.combatAbilityData.sourceEntity = entity;

                foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransform.combatAbilityData.combatAbilityComponents)
                {
                    combatAbilityComponent.entity = entity;
                }
            }
        }
    }

    public virtual void GetDamage(DamageComponent damageComponent, OverlapCollider[] overlapColliders)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;
        bool isParrying = IsParrying(sourceEntity, overlapColliders);
        bool isShielding = IsShielding(sourceEntity, overlapColliders);

        GetHealthDamage(damageComponent, isParrying, isShielding);
        GetPostureDamage(damageComponent, isParrying, isShielding);
    }

    public virtual void GetHealthDamage(DamageComponent damageComponent, bool isParrying, bool isShielding)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;

        float healthDamage = damageComponent.baseHealthDamage + sourceEntity.entityLevel * damageComponent.healthDamageIncreaseByLevel;

        if (damageComponent.canBeParried)
        {
            if (isParrying)
            {
                entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageParryRate));
                sourceEntity.entityStats.health.DecreaseCurrentValue(healthDamage * damageComponent.healthCounterDamageRate);
                return;
            }
            else
            {
                entity.entityStats.health.DecreaseCurrentValue(healthDamage);
                return;
            }
        }

        if (damageComponent.canBeShielded)
        {
            if (isParrying || isShielding)
            {
                entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageShieldRate));
                return;
            }
            else
            {
                entity.entityStats.health.DecreaseCurrentValue(healthDamage);
                return;
            }
        }

        entity.entityStats.health.DecreaseCurrentValue(healthDamage);
    }

    public virtual void GetPostureDamage(DamageComponent damageComponent, bool isParrying, bool isShielding)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;

        float postureDamage = damageComponent.basePostureDamage + sourceEntity.entityLevel * damageComponent.postureDamageIncreaseByLevel;

        if (damageComponent.canBeParried)
        {
            if (isParrying)
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage * (1.0f - damageComponent.postureDamageParryRate), false);
                sourceEntity.entityStats.posture.IncreaseCurrentValue(postureDamage * damageComponent.postureCounterDamageRate, !sourceEntity.GetType().Equals(typeof(Player)));
                return;
            }
            else
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
                return;
            }
        }

        if (damageComponent.canBeShielded)
        {
            if (isParrying || isShielding)
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage * (1.0f - damageComponent.postureDamageShieldRate));
                return;
            }
            else
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
                return;
            }
        }

        entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
    }

    /// <summary>
    /// Knockback time of 0 means that the knockback will be done when the entity hits the ground.
    /// </summary>
    public virtual void GetKnockback(KnockbackComponent knockbackComponent)
    {
        if (knockbackComponent.easeFunction == Ease.Unset)
        {
            entity.entityMovement.SetVelocity(knockbackComponent.knockbackDirection.normalized * knockbackComponent.knockbackSpeed);
        }
        else
        {
            entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
            entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
        }
    }

    private bool IsParrying(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool isParrying = false;

        foreach (OverlapCollider overlapCollider in overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                ShieldParryPrefab parryDetection = Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, overlapCollider.boxRotation, LayerMask.NameToLayer("ParryLayer")).Where(collider2D => collider2D.GetComponent<Entity>().Equals(entity)).Select(collider2D => collider2D.GetComponent<ShieldParryPrefab>()).FirstOrDefault();
                if (parryDetection != null && parryDetection.overlapCollider.overlapCircle)
                {
                    isParrying = CheckWithinAngle(new Vector2(Mathf.Cos(parryDetection.overlapCollider.boxRotation + parryDetection.overlapCollider.centerRotation), Mathf.Sin(parryDetection.overlapCollider.boxRotation + parryDetection.overlapCollider.centerRotation)), sourceEntity.transform.position - entity.transform.position, parryDetection.overlapCollider.counterClockwiseAngle, parryDetection.overlapCollider.clockwiseAngle);
                }
            }
            else if (overlapCollider.overlapCircle)
            {
                ShieldParryPrefab parryDetection = Physics2D.OverlapCircleAll(overlapCollider.centerTransform.position, overlapCollider.circleRadius, LayerMask.NameToLayer("ParryLayer")).Where(collider2D => collider2D.GetComponent<Entity>().Equals(entity)).Select(collider2D => collider2D.GetComponent<ShieldParryPrefab>()).FirstOrDefault();
                if (parryDetection != null && parryDetection.overlapCollider.overlapCircle)
                {
                    isParrying = CheckWithinAngle(new Vector2(Mathf.Cos(parryDetection.overlapCollider.centerRotation), Mathf.Sin(parryDetection.overlapCollider.centerRotation)), sourceEntity.transform.position - entity.transform.position, parryDetection.overlapCollider.counterClockwiseAngle, parryDetection.overlapCollider.clockwiseAngle);
                }
            }

            if (isParrying) return true;
        }

        return isParrying;
    }

    private bool IsShielding(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool isShielding = false;

        foreach (OverlapCollider overlapCollider in overlapColliders)
        {
            if (overlapCollider.overlapBox)
            {
                ShieldParryPrefab shieldDetection = Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, overlapCollider.boxRotation, LayerMask.NameToLayer("ShieldLayer")).Where(collider2D => collider2D.GetComponent<Entity>().Equals(entity)).Select(collider2D => collider2D.GetComponent<ShieldParryPrefab>()).FirstOrDefault();
                if (shieldDetection != null && shieldDetection.overlapCollider.overlapCircle)
                {
                    isShielding = CheckWithinAngle(new Vector2(Mathf.Cos(shieldDetection.overlapCollider.boxRotation + shieldDetection.overlapCollider.centerRotation), Mathf.Sin(shieldDetection.overlapCollider.boxRotation + shieldDetection.overlapCollider.centerRotation)), sourceEntity.transform.position - entity.transform.position, shieldDetection.overlapCollider.counterClockwiseAngle, shieldDetection.overlapCollider.clockwiseAngle);
                }
            }
            else if (overlapCollider.overlapCircle)
            {
                ShieldParryPrefab shieldDetection = Physics2D.OverlapCircleAll(overlapCollider.centerTransform.position, overlapCollider.circleRadius, LayerMask.NameToLayer("ShieldLayer")).Where(collider2D => collider2D.GetComponent<Entity>().Equals(entity)).Select(collider2D => collider2D.GetComponent<ShieldParryPrefab>()).FirstOrDefault();
                if (shieldDetection != null && shieldDetection.overlapCollider.overlapCircle)
                {
                    isShielding = CheckWithinAngle(new Vector2(Mathf.Cos(shieldDetection.overlapCollider.centerRotation), Mathf.Sin(shieldDetection.overlapCollider.centerRotation)), sourceEntity.transform.position - entity.transform.position, shieldDetection.overlapCollider.counterClockwiseAngle, shieldDetection.overlapCollider.clockwiseAngle);
                }
            }

            if (isShielding) return true;
        }

        return isShielding;
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
                    Transform[] projectileFireTransforms = combatAbilityWithTransforms.overlapColliders.Where(overlapCollider => !overlapCollider.overlapBox && !overlapCollider.overlapCircle).Select(overlapCollider => overlapCollider.centerTransform).ToArray();

                    if (projectileComponent.rotateTransform)
                    {

                    }
                    else
                    {
                        projectileComponent.ApplyCombatAbility(damageTargets, projectileFireTransforms, null);
                    }
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
        Gizmos.color = Color.blue;

        IEnumerable<PropertyInfo> combatAbilityWithTransformsProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.Equals(typeof(CombatAbilityWithTransforms)));

        foreach (PropertyInfo property in combatAbilityWithTransformsProperties)
        {
            CombatAbilityWithTransforms combatAbilityWithTransforms = property.GetValue(this) as CombatAbilityWithTransforms;

            if (!combatAbilityWithTransforms.visualize) continue;

            OverlapCollider[] overlapColliders = combatAbilityWithTransforms.overlapColliders;

            foreach (OverlapCollider overlapCollider in overlapColliders)
            {
                if (overlapCollider.overlapBox)
                {
                    float angle = Mathf.Atan2(overlapCollider.boxSize.y, overlapCollider.boxSize.x) * Mathf.Rad2Deg;

                    #region Draw Box
                    Vector3 topRightPosition = overlapCollider.centerTransform.position + overlapCollider.boxSize.magnitude * new Vector3(Mathf.Cos((angle + overlapCollider.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((angle + overlapCollider.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
                    Vector3 topLeftPosition = overlapCollider.centerTransform.position + overlapCollider.boxSize.magnitude * new Vector3(Mathf.Cos((180.0f - angle + overlapCollider.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((180.0f - angle + overlapCollider.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
                    Vector3 bottomLeftPosition = overlapCollider.centerTransform.position + overlapCollider.boxSize.magnitude * new Vector3(Mathf.Cos((angle - 180.0f + overlapCollider.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((angle - 180.0f + overlapCollider.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
                    Vector3 bottomRightPosition = overlapCollider.centerTransform.position + overlapCollider.boxSize.magnitude * new Vector3(Mathf.Cos((-angle + overlapCollider.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((-angle + overlapCollider.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
                    Vector3 topMidPosition = (topRightPosition + topLeftPosition) / 2.0f;
                    Vector3 bottomMidPosition = (bottomRightPosition + bottomLeftPosition) / 2.0f;
                    Vector3 leftMidPosition = (topLeftPosition + bottomLeftPosition) / 2.0f;
                    Vector3 rightMidPosition = (topRightPosition + bottomRightPosition) / 2.0f;

                    Gizmos.DrawLine(topLeftPosition, topRightPosition);
                    Gizmos.DrawLine(topRightPosition, bottomRightPosition);
                    Gizmos.DrawLine(bottomRightPosition, bottomLeftPosition);
                    Gizmos.DrawLine(bottomLeftPosition, topLeftPosition);
                    Gizmos.DrawLine(topMidPosition, bottomMidPosition);
                    Gizmos.DrawLine(leftMidPosition, rightMidPosition);
                    #endregion

                    if (overlapCollider.limitAngle)
                    {
                        #region Draw Center Line
                        if (overlapCollider.centerRotation > 0.0f)
                        {
                            if (overlapCollider.centerRotation < angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(overlapCollider.centerRotation * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (overlapCollider.centerRotation > 180.0f - angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(overlapCollider.centerRotation * Mathf.Deg2Rad) / 2.0f);
                            }
                            else
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f - overlapCollider.centerRotation) * Mathf.Deg2Rad) / 2.0f);
                            }
                        }
                        else
                        {
                            if (overlapCollider.centerRotation > -angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(overlapCollider.centerRotation * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (overlapCollider.centerRotation < angle - 180.0f)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(overlapCollider.centerRotation * Mathf.Deg2Rad) / 2.0f);
                            }
                            else
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f + overlapCollider.centerRotation) * Mathf.Deg2Rad) / 2.0f);
                            }
                        }
                        #endregion

                        #region Draw Clockwise Line
                        float clockwiseAngle = overlapCollider.centerRotation - overlapCollider.clockwiseAngle;
                        if (clockwiseAngle > 0.0f)
                        {
                            if (clockwiseAngle < angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (clockwiseAngle > 180.0f - angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f - clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                            }
                        }
                        else
                        {
                            if (clockwiseAngle > -angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (clockwiseAngle > angle - 180.0F)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f + clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (clockwiseAngle >= -180.0f)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (clockwiseAngle > -180.0f - angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (clockwiseAngle > angle - 360.0f)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f + clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (clockwiseAngle >= -360.0f)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                        }
                        #endregion

                        #region Draw Counterclockwise Line
                        float counterClockwiseAngle = overlapCollider.centerRotation + overlapCollider.counterClockwiseAngle;
                        if (counterClockwiseAngle > 0.0f)
                        {
                            if (counterClockwiseAngle < angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (counterClockwiseAngle < 180.0f - angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f - counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (counterClockwiseAngle <= 180.0f)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (counterClockwiseAngle < 180.0f + angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (counterClockwiseAngle < 360.0f - angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f - counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (counterClockwiseAngle <= 360.0f)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                        }
                        else
                        {
                            if (counterClockwiseAngle > -angle)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else if (counterClockwiseAngle < angle - 180.0f)
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                            }
                            else
                            {
                                Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f + counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                            }
                        }
                        #endregion
                    }
                }
                else if (overlapCollider.overlapCircle)
                {
                    Gizmos.DrawWireSphere(overlapCollider.centerTransform.position, overlapCollider.circleRadius);

                    if (overlapCollider.limitAngle)
                    {
                        Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.circleRadius);

                        Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + overlapCollider.centerTransform.right * overlapCollider.circleRadius);
                        Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(-overlapCollider.clockwiseAngle + overlapCollider.centerRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.circleRadius);
                        Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.counterClockwiseAngle + overlapCollider.centerRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.circleRadius);
                    }
                }
            }
        }

        Gizmos.color = Color.red;

        IEnumerable<PropertyInfo> combatAbilityWithTransformsListProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.Equals(typeof(List<CombatAbilityWithTransforms>)));

        foreach (PropertyInfo property in  combatAbilityWithTransformsListProperties)
        {
            List<CombatAbilityWithTransforms> combatAbilityWithTransformsList = property.GetValue(this) as List<CombatAbilityWithTransforms>;
            
            foreach (CombatAbilityWithTransforms combatAbilityWithTransforms in combatAbilityWithTransformsList)
            {
                if (!combatAbilityWithTransforms.visualize) continue;

                OverlapCollider[] overlapColliders = combatAbilityWithTransforms.overlapColliders;

                foreach (OverlapCollider overlapCollider in overlapColliders)
                {
                    if (overlapCollider.overlapBox)
                    {
                        float angle = Mathf.Atan2(overlapCollider.boxSize.y, overlapCollider.boxSize.x) * Mathf.Rad2Deg;

                        #region Draw Box
                        Vector3 topRightPosition = overlapCollider.centerTransform.position + overlapCollider.boxSize.magnitude * new Vector3(Mathf.Cos((angle + overlapCollider.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((angle + overlapCollider.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
                        Vector3 topLeftPosition = overlapCollider.centerTransform.position + overlapCollider.boxSize.magnitude * new Vector3(Mathf.Cos((180.0f - angle + overlapCollider.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((180.0f - angle + overlapCollider.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
                        Vector3 bottomLeftPosition = overlapCollider.centerTransform.position + overlapCollider.boxSize.magnitude * new Vector3(Mathf.Cos((angle - 180.0f + overlapCollider.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((angle - 180.0f + overlapCollider.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
                        Vector3 bottomRightPosition = overlapCollider.centerTransform.position + overlapCollider.boxSize.magnitude * new Vector3(Mathf.Cos((-angle + overlapCollider.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((-angle + overlapCollider.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
                        Vector3 topMidPosition = (topRightPosition + topLeftPosition) / 2.0f;
                        Vector3 bottomMidPosition = (bottomRightPosition + bottomLeftPosition) / 2.0f;
                        Vector3 leftMidPosition = (topLeftPosition + bottomLeftPosition) / 2.0f;
                        Vector3 rightMidPosition = (topRightPosition + bottomRightPosition) / 2.0f;
                        
                        Gizmos.DrawLine(topLeftPosition, topRightPosition);
                        Gizmos.DrawLine(topRightPosition, bottomRightPosition);
                        Gizmos.DrawLine(bottomRightPosition, bottomLeftPosition);
                        Gizmos.DrawLine(bottomLeftPosition, topLeftPosition);
                        Gizmos.DrawLine(topMidPosition, bottomMidPosition);
                        Gizmos.DrawLine(leftMidPosition, rightMidPosition);
                        #endregion

                        if (overlapCollider.limitAngle)
                        {
                            #region Draw Center Line
                            if (overlapCollider.centerRotation > 0.0f)
                            {
                                if (overlapCollider.centerRotation < angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(overlapCollider.centerRotation * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (overlapCollider.centerRotation > 180.0f - angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(overlapCollider.centerRotation * Mathf.Deg2Rad) / 2.0f);
                                }
                                else
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f - overlapCollider.centerRotation) * Mathf.Deg2Rad) / 2.0f);
                                }
                            }
                            else
                            {
                                if (overlapCollider.centerRotation > -angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(overlapCollider.centerRotation * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (overlapCollider.centerRotation < angle - 180.0f)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(overlapCollider.centerRotation * Mathf.Deg2Rad) / 2.0f);
                                }
                                else
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f + overlapCollider.centerRotation) * Mathf.Deg2Rad) / 2.0f);
                                }
                            }
                            #endregion

                            #region Draw Clockwise Line
                            float clockwiseAngle = overlapCollider.centerRotation - overlapCollider.clockwiseAngle;
                            if (clockwiseAngle > 0.0f)
                            {
                                if (clockwiseAngle < angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (clockwiseAngle > 180.0f - angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f - clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                                }
                            }
                            else
                            {
                                if (clockwiseAngle > -angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (clockwiseAngle > angle - 180.0F)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f + clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (clockwiseAngle >= -180.0f)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (clockwiseAngle > -180.0f - angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (clockwiseAngle > angle - 360.0f)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f + clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (clockwiseAngle >= -360.0f)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                            }
                            #endregion

                            #region Draw Counterclockwise Line
                            float counterClockwiseAngle = overlapCollider.centerRotation + overlapCollider.counterClockwiseAngle;
                            if (counterClockwiseAngle > 0.0f)
                            {
                                if (counterClockwiseAngle < angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (counterClockwiseAngle < 180.0f - angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f - counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (counterClockwiseAngle <= 180.0f)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (counterClockwiseAngle < 180.0f + angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (counterClockwiseAngle < 360.0f - angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f - counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (counterClockwiseAngle <= 360.0f)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                            }
                            else
                            {
                                if (counterClockwiseAngle > -angle)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else if (counterClockwiseAngle < angle - 180.0f)
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                                }
                                else
                                {
                                    Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + overlapCollider.boxRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.boxSize.y / Mathf.Cos((90.0f + counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                                }
                            }
                            #endregion
                        }
                    }
                    else if (overlapCollider.overlapCircle)
                    {
                        Gizmos.DrawWireSphere(overlapCollider.centerTransform.position, overlapCollider.circleRadius);

                        if (overlapCollider.limitAngle)
                        {
                            Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.centerRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.circleRadius);

                            Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + overlapCollider.centerTransform.right * overlapCollider.circleRadius);
                            Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(-overlapCollider.clockwiseAngle + overlapCollider.centerRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.circleRadius);
                            Gizmos.DrawLine(overlapCollider.centerTransform.position, overlapCollider.centerTransform.position + Quaternion.AngleAxis(overlapCollider.counterClockwiseAngle + overlapCollider.centerRotation, overlapCollider.centerTransform.forward) * overlapCollider.centerTransform.right * overlapCollider.circleRadius);
                        }
                    }
                }
            }
        }
    }
}