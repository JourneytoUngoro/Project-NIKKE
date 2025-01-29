using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using static Unity.Burst.Intrinsics.Arm;

/* Incomplete */
/* Needs to be optimized and improved */

public abstract class Combat : CoreComponent
{
    [field: SerializeField] public LayerMask whatIsDamageable { get; protected set; }
    [SerializeField] private LayerMask shieldLayer;
    [SerializeField] private LayerMask parryLayer;

    public List<Collider2D> damagedTargets { get; protected set; }

    protected override void Awake()
    {
        base.Awake();

        damagedTargets = new List<Collider2D>();

        IEnumerable<PropertyInfo> combatAbilityProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.Equals(typeof(List<CombatAbilityWithTransforms>)));

        foreach (PropertyInfo property in combatAbilityProperties)
        {
            List<CombatAbilityWithTransforms> combatAbilityList = property.GetValue(this) as List<CombatAbilityWithTransforms>;

            foreach (CombatAbilityWithTransforms combatAbilityWithTransforms in combatAbilityList)
            {
                combatAbilityWithTransforms.combatAbilityData.sourceEntity = entity;

                foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransforms.combatAbilityData.combatAbilityComponents)
                {
                    combatAbilityComponent.entity = entity;
                    combatAbilityComponent.pertainedCombatAbility = combatAbilityWithTransforms.combatAbilityData;

                    if (combatAbilityComponent.GetType().Equals(typeof(ProjectileComponent)))
                    {
                        ProjectileComponent projectileComponent = combatAbilityComponent as ProjectileComponent;

                        Projectile projectile = projectileComponent.projectilePrefab.GetComponent<Projectile>();
                        Explosion explosion = projectileComponent.projectilePrefab.GetComponent<Explosion>();

                        if (projectile != null)
                        {
                            projectile.combatAbility.sourceEntity = entity;
                        }
                        
                        if (explosion != null)
                        {
                            foreach (CombatAbilityWithTransforms explosionArea in explosion.explosionAreas)
                            {
                                explosionArea.combatAbilityData.sourceEntity = entity;
                            }
                        }
                    }
                    else if (combatAbilityComponent.GetType().Equals(typeof(KnockbackComponent)))
                    {
                        KnockbackComponent knockbackComponent = combatAbilityComponent as KnockbackComponent;
                        knockbackComponent.knockbackSourceTransform = entity.transform;
                    }
                }
            }
        }

        combatAbilityProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.Equals(typeof(CombatAbilityWithTransforms)));

        foreach (PropertyInfo property in combatAbilityProperties)
        {
            CombatAbilityWithTransforms combatAbilityWithTransforms = property.GetValue(this) as CombatAbilityWithTransforms;

            combatAbilityWithTransforms.combatAbilityData.sourceEntity = entity;

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransforms.combatAbilityData.combatAbilityComponents)
            {
                combatAbilityComponent.entity = entity;
                combatAbilityComponent.pertainedCombatAbility = combatAbilityWithTransforms.combatAbilityData;
            }
        }
    }

    public virtual void GetDamage(DamageComponent damageComponent, OverlapCollider[] overlapColliders)
    {
        Debug.Log("Get Damage Function Called");
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;
        bool isParrying = damageComponent.canBeParried ? IsParrying(sourceEntity, overlapColliders) : false;
        bool isShielding = damageComponent.canBeShielded ? IsShielding(sourceEntity, overlapColliders) : false;

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
                entity.animator.SetInteger("parryType", UtilityFunctions.RandomInteger(0, 3));
                entity.animator.SetTrigger("parried");
                entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageParryRate));
                sourceEntity.entityStats.health.DecreaseCurrentValue(healthDamage * damageComponent.healthCounterDamageRate);
            }
            else
            {
                if (damageComponent.canBeShielded)
                {
                    if (isParrying || isShielding)
                    {
                        entity.animator.SetTrigger("shielded");
                        entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageShieldRate));
                    }
                    else
                    {
                        entity.animator.SetTrigger("gotHit");
                        entity.entityStats.health.DecreaseCurrentValue(healthDamage);
                    }
                }
                else
                {
                    entity.animator.SetTrigger("gotHit");
                    entity.entityStats.health.DecreaseCurrentValue(healthDamage);
                }
            }
        }
        else if (damageComponent.canBeShielded)
        {
            if (isParrying || isShielding)
            {
                entity.animator.SetTrigger("shielded");
                entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageShieldRate));
            }
            else
            {
                entity.animator.SetTrigger("gotHit");
                entity.entityStats.health.DecreaseCurrentValue(healthDamage);
            }
        }
        else
        {
            entity.animator.SetTrigger("gotHit");
            entity.entityStats.health.DecreaseCurrentValue(healthDamage);
        }
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
            }
            else
            {
                if (damageComponent.canBeShielded)
                {
                    if (isParrying || isShielding)
                    {
                        entity.entityStats.posture.IncreaseCurrentValue(postureDamage * (1.0f - damageComponent.postureDamageShieldRate));
                    }
                    else
                    {
                        entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
                    }
                }
                else
                {
                    entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
                }
            }
        }
        else if (damageComponent.canBeShielded)
        {
            if (isParrying || isShielding)
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage * (1.0f - damageComponent.postureDamageShieldRate));
            }
            else
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
            }
        }
        else
        {
            entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
        }
    }

    /// <summary>
    /// Knockback time of 0 means that the knockback will be done when the entity hits the ground.
    /// </summary>
    public virtual void GetKnockback(KnockbackComponent knockbackComponent, OverlapCollider[] overlapColliders)
    {
        Debug.Log("Get Knockback Function Called");
        // TODO
        // 1. 투사체일때 sourceEntity 문제를 어떻게 해결할 것인가? -> source transform을 활용한다
        // 2. Ease.Unset일때 시간을 어떻게 할 것인가? -> 벽에 부딛힐시 코루틴 탈출
        Entity sourceEntity = knockbackComponent.pertainedCombatAbility.sourceEntity;
        bool isParrying = knockbackComponent.canBeParried ? IsParrying(sourceEntity, overlapColliders) : false;
        bool isShielding = knockbackComponent.canBeShielded ? IsShielding(sourceEntity, overlapColliders) : false;
        bool isGrounded = entity.entityDetection.isGrounded();

        int direction = 0;

        switch (knockbackComponent.directionBase)
        {
            case DirectionBase.Transform:
                direction = entity.transform.position.x - knockbackComponent.knockbackSourceTransform.position.x < 0 ? -1 : 1; break;
            case DirectionBase.Rotation:
                direction = Mathf.Abs(knockbackComponent.knockbackSourceTransform.rotation.y) < epsilon ? 1 : -1; break;
            case DirectionBase.Absolute:
                direction = 1; break;
            default:
                break;
        }

        if (knockbackComponent.canBeParried)
        {
            if (isParrying)
            {
                entity.animator.SetInteger("parryType", UtilityFunctions.RandomInteger(0, 3));
                entity.animator.SetTrigger("parried");
                
                if (knockbackComponent.isParriedKnockbackDifferentInAir)
                {
                    if (!isGrounded)
                    {
                        entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenParriedInAir.normalized.x * direction * knockbackComponent.knockbackSpeedWhenParriedInAir, knockbackComponent.knockbackTimeWhenParriedInAir, knockbackComponent.easeFunctionWhenParriedInAir, true);
                        entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenParriedInAir.normalized.y * knockbackComponent.knockbackSpeedWhenParriedInAir);
                        
                        if (!knockbackComponent.pertainedCombatAbility.stance)
                        {
                            sourceEntity.animator.SetTrigger("wasParried");
                            sourceEntity.entityCombat.ChangeToKnockbackState(knockbackComponent.knockbackTimeWhenParriedInAir);

                            sourceEntity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.counterKnockbackDirectionWhenParriedInAir.normalized.x * direction * knockbackComponent.counterKnockbackSpeedWhenParriedInAir, knockbackComponent.counterKnockbackTimeWhenParriedInAir, knockbackComponent.counterEaseFunctionWhenParriedInAir, true);
                            sourceEntity.entityMovement.SetVelocityY(knockbackComponent.counterKnockbackDirectionWhenParriedInAir.normalized.y * knockbackComponent.counterKnockbackSpeedWhenParriedInAir);
                        }
                    }
                    else
                    {
                        entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenParried.normalized.x * direction * knockbackComponent.knockbackSpeedWhenParried, knockbackComponent.knockbackTimeWhenParried, knockbackComponent.easeFunctionWhenParried, true);
                        entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenParried.normalized.y * knockbackComponent.knockbackSpeedWhenParried);

                        if (!knockbackComponent.pertainedCombatAbility.stance)
                        {
                            sourceEntity.animator.SetTrigger("wasParried");
                            sourceEntity.entityCombat.ChangeToKnockbackState(knockbackComponent.knockbackTimeWhenParried);

                            sourceEntity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.counterKnockbackDirectionWhenParried.normalized.x * direction * knockbackComponent.counterKnockbackSpeedWhenParried, knockbackComponent.counterKnockbackTimeWhenParried, knockbackComponent.counterEaseFunctionWhenParried, true);
                            sourceEntity.entityMovement.SetVelocityY(knockbackComponent.counterKnockbackDirectionWhenParried.normalized.y * knockbackComponent.counterKnockbackSpeedWhenParried);
                        }
                    }
                }
                else
                {
                    entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenParried.normalized.x * direction * knockbackComponent.knockbackSpeedWhenParried, knockbackComponent.knockbackTimeWhenParried, knockbackComponent.easeFunctionWhenParried, true);
                    entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenParried.normalized.y * knockbackComponent.knockbackSpeedWhenParried);

                    if (!knockbackComponent.pertainedCombatAbility.stance)
                    {
                        sourceEntity.animator.SetTrigger("wasParried");
                        sourceEntity.entityCombat.ChangeToKnockbackState(knockbackComponent.knockbackTimeWhenParried);

                        sourceEntity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.counterKnockbackDirectionWhenParried.normalized.x * direction * knockbackComponent.counterKnockbackSpeedWhenParried, knockbackComponent.counterKnockbackTimeWhenParried, knockbackComponent.counterEaseFunctionWhenParried, true);
                        sourceEntity.entityMovement.SetVelocityY(knockbackComponent.counterKnockbackDirectionWhenParried.normalized.y * knockbackComponent.counterKnockbackSpeedWhenParried);
                    }
                }
            }
            else
            {
                if (knockbackComponent.canBeShielded)
                {
                    if (isParrying || isShielding)
                    {
                        entity.animator.SetTrigger("shielded");

                        if (knockbackComponent.isShieldedKnockbackDifferentInAir)
                        {
                            if (!isGrounded)
                            {
                                entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenShieldedInAir.normalized.x * direction * knockbackComponent.knockbackSpeedWhenShieldedInAir, knockbackComponent.knockbackTimeWhenShieldedInAir, knockbackComponent.easeFunctionWhenShieldedInAir, true);
                                entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenShieldedInAir.normalized.y * knockbackComponent.knockbackSpeedWhenShieldedInAir);
                            }
                            else
                            {
                                entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenShielded.normalized.x * direction * knockbackComponent.knockbackSpeedWhenShielded, knockbackComponent.knockbackTimeWhenShielded, knockbackComponent.easeFunctionWhenShielded, true);
                                entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenShielded.normalized.y * knockbackComponent.knockbackSpeedWhenShielded);
                            }
                        }
                        else
                        {
                            entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenShielded.normalized.x * direction * knockbackComponent.knockbackSpeedWhenShielded, knockbackComponent.knockbackTimeWhenShielded, knockbackComponent.easeFunctionWhenShielded, true);
                            entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenShielded.normalized.y * knockbackComponent.knockbackSpeedWhenShielded);
                        }
                    }
                    else
                    {
                        entity.animator.SetTrigger("gotHit");
                        ChangeToKnockbackState(knockbackComponent, isGrounded);

                        if (knockbackComponent.isKnockbackDifferentInAir)
                        {
                            if (!isGrounded)
                            {
                                entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionInAir.normalized.x * direction * knockbackComponent.knockbackSpeedInAir, knockbackComponent.knockbackTimeInAir, knockbackComponent.easeFunctionInAir, true);
                                entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionInAir.normalized.y * knockbackComponent.knockbackSpeedInAir);
                            }
                            else
                            {
                                entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * direction * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
                                entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
                            }
                        }
                        else
                        {
                            entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * direction * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
                            entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
                        }
                    }
                }
                else
                {
                    entity.animator.SetTrigger("gotHit");
                    ChangeToKnockbackState(knockbackComponent, isGrounded);

                    if (knockbackComponent.isKnockbackDifferentInAir)
                    {
                        if (!isGrounded)
                        {
                            entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionInAir.normalized.x * direction * knockbackComponent.knockbackSpeedInAir, knockbackComponent.knockbackTimeInAir, knockbackComponent.easeFunctionInAir, true);
                            entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionInAir.normalized.y * knockbackComponent.knockbackSpeedInAir);
                        }
                        else
                        {
                            entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * direction * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
                            entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
                        }
                    }
                    else
                    {
                        entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * direction * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
                        entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
                    }
                }
            }
        }
        else if (knockbackComponent.canBeShielded)
        {
            if (isParrying || isShielding)
            {
                entity.animator.SetTrigger("shielded");

                if (knockbackComponent.isShieldedKnockbackDifferentInAir)
                {
                    if (!isGrounded)
                    {
                        entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenShieldedInAir.normalized.x * direction * knockbackComponent.knockbackSpeedWhenShieldedInAir, knockbackComponent.knockbackTimeWhenShieldedInAir, knockbackComponent.easeFunctionWhenShieldedInAir, true);
                        entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenShieldedInAir.normalized.y * knockbackComponent.knockbackSpeedWhenShieldedInAir);
                    }
                    else
                    {
                        entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenShielded.normalized.x * direction * knockbackComponent.knockbackSpeedWhenShielded, knockbackComponent.knockbackTimeWhenShielded, knockbackComponent.easeFunctionWhenShielded, true);
                        entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenShielded.normalized.y * knockbackComponent.knockbackSpeedWhenShielded);
                    }
                }
                else
                {
                    entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionWhenShielded.normalized.x * direction * knockbackComponent.knockbackSpeedWhenShielded, knockbackComponent.knockbackTimeWhenShielded, knockbackComponent.easeFunctionWhenShielded, true);
                    entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionWhenShielded.normalized.y * knockbackComponent.knockbackSpeedWhenShielded);
                }
            }
            else
            {
                entity.animator.SetTrigger("gotHit");
                ChangeToKnockbackState(knockbackComponent, isGrounded);

                if (knockbackComponent.isKnockbackDifferentInAir)
                {
                    if (!isGrounded)
                    {
                        entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionInAir.normalized.x * direction * knockbackComponent.knockbackSpeedInAir, knockbackComponent.knockbackTimeInAir, knockbackComponent.easeFunctionInAir, true);
                        entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionInAir.normalized.y * knockbackComponent.knockbackSpeedInAir);
                    }
                    else
                    {
                        entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * direction * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
                        entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
                    }
                }
                else
                {
                    entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * direction * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
                    entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
                }
            }
        }
        else
        {
            entity.animator.SetTrigger("gotHit");
            ChangeToKnockbackState(knockbackComponent, isGrounded);

            if (knockbackComponent.isKnockbackDifferentInAir)
            {
                if (!isGrounded)
                {
                    entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirectionInAir.normalized.x * direction * knockbackComponent.knockbackSpeedInAir, knockbackComponent.knockbackTimeInAir, knockbackComponent.easeFunctionInAir, true);
                    entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirectionInAir.normalized.y * knockbackComponent.knockbackSpeedInAir);
                }
                else
                {
                    entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * direction * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
                    entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
                }
            }
            else
            {
                entity.entityMovement.SetVelocityXChangeOverTime(knockbackComponent.knockbackDirection.normalized.x * direction * knockbackComponent.knockbackSpeed, knockbackComponent.knockbackTime, knockbackComponent.easeFunction, true);
                entity.entityMovement.SetVelocityY(knockbackComponent.knockbackDirection.normalized.y * knockbackComponent.knockbackSpeed);
            }
        }
    }

    protected abstract void ChangeToKnockbackState(float knockbackTime);
    protected abstract void ChangeToKnockbackState(KnockbackComponent knockbackComponent, bool isGrounded);

    public void GetRebound(Vector2 reboundDirection, float reboundVelocity, float reboundTime, Ease reboundFunction)
    {
        entity.entityMovement.SetVelocityXChangeOverTime(reboundDirection.normalized.x * reboundVelocity * entity.entityMovement.facingDirection, reboundTime, reboundFunction, true);
        entity.entityMovement.SetVelocityY(reboundDirection.normalized.y * reboundVelocity);
    }

    protected bool IsParrying(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool isParrying = false;

        if (sourceEntity != null)
        {
            foreach (OverlapCollider overlapCollider in overlapColliders)
            {
                if (overlapCollider.overlapBox)
                {
                    ShieldParry parryDetection = Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, overlapCollider.boxRotation, parryLayer).Select(collider2D => collider2D.GetComponent<ShieldParry>()).Where(shieldParry => shieldParry.GetComponentInParent<Entity>().Equals(entity)).FirstOrDefault();

                    if (parryDetection != null)
                    {
                        isParrying = parryDetection.overlapCollider.limitAngle ? CheckWithinAngle(new Vector2(Mathf.Cos(parryDetection.overlapCollider.boxRotation + parryDetection.overlapCollider.centerRotation), Mathf.Sin(parryDetection.overlapCollider.boxRotation + parryDetection.overlapCollider.centerRotation)) * entity.entityMovement.facingDirection, sourceEntity.transform.position - entity.transform.position, parryDetection.overlapCollider.counterClockwiseAngle, parryDetection.overlapCollider.clockwiseAngle) : true;
                    }
                }
                else if (overlapCollider.overlapCircle)
                {
                    ShieldParry parryDetection = Physics2D.OverlapCircleAll(overlapCollider.centerTransform.position, overlapCollider.circleRadius, parryLayer).Select(collider2D => collider2D.GetComponent<ShieldParry>()).Where(shieldParry => shieldParry.GetComponentInParent<Entity>().Equals(entity)).FirstOrDefault();

                    if (parryDetection != null)
                    {
                        isParrying = parryDetection.overlapCollider.limitAngle ? CheckWithinAngle(new Vector2(Mathf.Cos(parryDetection.overlapCollider.centerRotation), Mathf.Sin(parryDetection.overlapCollider.centerRotation)) * entity.entityMovement.facingDirection, sourceEntity.transform.position - entity.transform.position, parryDetection.overlapCollider.counterClockwiseAngle, parryDetection.overlapCollider.clockwiseAngle) : true;
                    }
                }

                if (isParrying) break;
            }
        }

        return isParrying;
    }

    protected bool IsShielding(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool isShielding = false;

        if (sourceEntity != null)
        {
            foreach (OverlapCollider overlapCollider in overlapColliders)
            {
                if (overlapCollider.overlapBox)
                {
                    ShieldParry shieldDetection = Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, overlapCollider.boxRotation, shieldLayer).Select(collider2D => collider2D.GetComponent<ShieldParry>()).Where(shieldParry => shieldParry.GetComponentInParent<Entity>().Equals(entity)).FirstOrDefault();

                    if (shieldDetection != null)
                    {
                        isShielding = shieldDetection.overlapCollider.limitAngle ? CheckWithinAngle(new Vector2(Mathf.Cos(shieldDetection.overlapCollider.boxRotation + shieldDetection.overlapCollider.centerRotation), Mathf.Sin(shieldDetection.overlapCollider.boxRotation + shieldDetection.overlapCollider.centerRotation)) * entity.entityMovement.facingDirection, sourceEntity.transform.position - entity.transform.position, shieldDetection.overlapCollider.counterClockwiseAngle, shieldDetection.overlapCollider.clockwiseAngle) : true;
                    }
                }
                else if (overlapCollider.overlapCircle)
                {
                    ShieldParry shieldDetection = Physics2D.OverlapCircleAll(overlapCollider.centerTransform.position, overlapCollider.circleRadius, shieldLayer).Select(collider2D => collider2D.GetComponent<ShieldParry>()).Where(shieldParry => shieldParry.GetComponentInParent<Entity>().Equals(entity)).FirstOrDefault();

                    if (shieldDetection != null)
                    {
                        isShielding = shieldDetection.overlapCollider.limitAngle ? CheckWithinAngle(new Vector2(Mathf.Cos(shieldDetection.overlapCollider.centerRotation), Mathf.Sin(shieldDetection.overlapCollider.centerRotation)) * entity.entityMovement.facingDirection, sourceEntity.transform.position - entity.transform.position, shieldDetection.overlapCollider.counterClockwiseAngle, shieldDetection.overlapCollider.clockwiseAngle) : true;
                    }
                }

                if (isShielding) break;
            }
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
                damageTargets = damageTargets.Union(Physics2D.OverlapBoxAll(overlapCollider.centerTransform.position, overlapCollider.boxSize, overlapCollider.boxRotation, whatIsDamageable)).ToList();
            }
            else if (overlapCollider.overlapCircle)
            {
                damageTargets = damageTargets.Union(Physics2D.OverlapCircleAll(overlapCollider.centerTransform.position, overlapCollider.circleRadius, whatIsDamageable)).ToList();
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
                    projectileComponent.ApplyCombatAbility(damageTargets, projectileFireTransforms, null);
                    break;
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

        return foundTarget;
    }

    public bool CheckWithinAngle(Vector2 baseVector, Vector2 targetVector, float counterClockwiseAngle, float clockwiseAngle)
    {
        float angleBetweenVectors = -Vector2.SignedAngle(baseVector, targetVector);
        return -counterClockwiseAngle <= angleBetweenVectors && angleBetweenVectors <= clockwiseAngle;
    }

    public void ClearDamagedTargets()
    {
        Debug.Log($"Clear damagedTargets");
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

            if (Physics2D.Raycast(prevPosition, currentPosition - prevPosition, Vector2.Distance(currentPosition, prevPosition), entity.entityDetection.whatIsGround))
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

    public void ReleaseShieldParryPrefabs(CombatAbility pertainedCombatAbility)
    {
        foreach (ShieldParry shieldParryPrefab in entity.entityCombat.GetComponentsInChildren<ShieldParry>())
        {
            if (pertainedCombatAbility != null)
            {
                if (shieldParryPrefab.pertainedCombatAbility.Equals(pertainedCombatAbility))
                {
                    shieldParryPrefab.ReleaseObject();
                }
            }
            else
            {
                shieldParryPrefab.ReleaseObject();
            }
        }
    }

    public void ReleaseShieldParryPrefabs(CombatAbilityWithTransforms pertainedCombatAbilityWithTransforms)
    {
        foreach (ShieldParry shieldParryPrefab in entity.entityCombat.GetComponentsInChildren<ShieldParry>())
        {
            if (pertainedCombatAbilityWithTransforms != null && pertainedCombatAbilityWithTransforms.combatAbilityData != null)
            {
                if (shieldParryPrefab.pertainedCombatAbility.Equals(pertainedCombatAbilityWithTransforms.combatAbilityData))
                {
                    shieldParryPrefab.ReleaseObject();
                }
            }
            else
            {
                shieldParryPrefab.ReleaseObject();
            }
        }
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