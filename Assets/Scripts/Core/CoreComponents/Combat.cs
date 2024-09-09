using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Reflection;

/* Incomplete */
/* Needs to be optimized and improved */

public class Combat : CoreComponent
{
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsDamageable;
    
    [field: SerializeField] public Transform meleeAttackTransform { get; protected set; }
    [SerializeField] protected float meleeAttackRadius;
    [SerializeField] protected Vector2 meleeAttackSize;
    
    [field: SerializeField] public Transform rangedAttackTransform { get; protected set; }
    [SerializeField] protected float rangedAttackRadius;
    [SerializeField] protected Vector2 rangedAttackSize;

    [SerializeField] protected List<CombatAbilityWithTransform> meleeAttack;
    [SerializeField] protected List<CombatAbilityWithTransform> rangedAttack;

    protected List<Collider2D> damagedTargets;

    protected override void Awake()
    {
        base.Awake();

        damagedTargets = new List<Collider2D>();
    }

    public virtual void GetDamage(DamageComponent damageComponent)
    {
        GetHealthDamage(damageComponent);
        GetPostureDamage(damageComponent);
    }

    public virtual void GetHealthDamage(DamageComponent damageComponent)
    {
        // entity.entityStats.health.DecreaseCurrentValue(healthDamage);
        if (player != null)
        {
            if (player.playerStateMachine.currentState.Equals(player.shieldParryState))
            {
                if (damageComponent.canBeParried)
                {
                    if (player.shieldParryState.isParrying)
                    {
                        player.stats.health.DecreaseCurrentValue((damageComponent.baseHealthDamage + damageComponent.healthDamageIncreaseByLevel * (entity as Enemy).level) *   damageComponent.healthDamageParryRate);
                        damageComponent.entity.entityStats.health.DecreaseCurrentValue((damageComponent.baseHealthDamage + damageComponent.healthDamageIncreaseByLevel * (entity as Enemy).level) *     damageComponent.healthCounterDamageRate);
                    }
                }
                else if (damageComponent.canBeShielded)
                {
                    player.stats.health.DecreaseCurrentValue((damageComponent.baseHealthDamage + damageComponent.healthDamageIncreaseByLevel * (entity as Enemy).level) *   damageComponent.healthDamageShieldRate);
                }
                else
                {
                    player.stats.health.DecreaseCurrentValue((damageComponent.baseHealthDamage + damageComponent.healthDamageIncreaseByLevel * (entity as Enemy).level));
                }
            }
            else
            {
                player.stats.health.DecreaseCurrentValue(damageComponent.baseHealthDamage + damageComponent.healthDamageIncreaseByLevel * (entity as Enemy).level);
            }
        }
        else if (enemy != null)
        {
            if (entity.GetType().Equals(typeof(Player)))
            {
                enemy.stats.health.DecreaseCurrentValue(damageComponent.baseHealthDamage + damageComponent.healthDamageIncreaseByLevel * Manager.Instance.dataManager.gameData.attackLevel);
            }
            else if (entity.GetType().Equals(typeof(Enemy)))
            {
                enemy.stats.health.DecreaseCurrentValue(damageComponent.baseHealthDamage + damageComponent.healthDamageIncreaseByLevel * (entity as Enemy).level);
            }
        }
    }

    public virtual void GetPostureDamage(DamageComponent damageComponent)
    {
        // entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
        if (player != null)
        {

        }
        else if (enemy != null)
        {

        }
    }

    /// <summary>
    /// Knockback time of 0 means that the knockback will be done when the entity hits the ground.
    /// </summary>
    public virtual void GetKnockback(KnockbackComponent knockbackComponent)
    {
        if (player != null)
        {
            
        }
        else if (enemy != null)
        {
            if (!enemy.stance)
            {
                enemy.enemyStateMachine.ChangeState(enemy.knockbackState);
            }
        }
    }

    public virtual void DoMeleeAttack(int index = 0)
    {
        Collider2D[] damageTargets = new Collider2D[0];

        if (meleeAttack[index].overlapCollider.overlapBox)
        {
            damageTargets = Physics2D.OverlapBoxAll(meleeAttack[index].centerTransform.position, meleeAttack[index].overlapCollider.boxSize, 0.0f, whatIsDamageable);
        }
        else if (meleeAttack[index].overlapCollider.overlapCircle)
        {
            damageTargets = Physics2D.OverlapCircleAll(meleeAttack[index].centerTransform.position, meleeAttack[index].overlapCollider.circleRadius, whatIsDamageable);
        }

        foreach (Collider2D damageTarget in damageTargets)
        {
            foreach (CombatAbilityComponent combatAbilityComponent in meleeAttack[index].combatAbilityData.combatAbilityComponents)
            {
                combatAbilityComponent.ApplyCombatAbility(damageTarget);
            }
        }
    }

    public virtual void DoRangedAttack()
    {
        
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

        if (rangedAttackRadius > epsilon)
        {
            Gizmos.DrawWireSphere(rangedAttackTransform.position, rangedAttackRadius);
        }
        else if (rangedAttackSize.x > epsilon && rangedAttackSize.y > epsilon)
        {
            Gizmos.DrawWireCube(rangedAttackTransform.position, rangedAttackSize);
        }
        if (meleeAttackRadius > epsilon)
        {
            Gizmos.DrawWireSphere(meleeAttackTransform.position, meleeAttackRadius);
        }
        else if (meleeAttackSize.x > epsilon && meleeAttackSize.y > epsilon)
        {
            Gizmos.DrawWireCube(meleeAttackTransform.position, meleeAttackSize);
        }

        foreach (CombatAbilityWithTransform combatAbilityWithTransform in meleeAttack)
        {
            if (combatAbilityWithTransform.overlapCollider.overlapBox)
            {
                Gizmos.DrawWireCube(combatAbilityWithTransform.centerTransform.position, combatAbilityWithTransform.overlapCollider.boxSize);
            }
            else if (combatAbilityWithTransform.overlapCollider.overlapCircle)
            {
                Gizmos.DrawWireSphere(combatAbilityWithTransform.centerTransform.position, combatAbilityWithTransform.overlapCollider.circleRadius);
            }
        }
    }
}