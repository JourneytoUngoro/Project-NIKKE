using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

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

    protected List<Collider2D> damagedTargets;

    protected override void Awake()
    {
        base.Awake();

        damagedTargets = new List<Collider2D>();
    }

    public virtual void GetDamage(AttackInfo attackInfo)
    {

    }

    public virtual void GetHealthDamage(float healthDamage)
    {
        if (player != null)
        {
            player.stats.health.DecreaseCurrentValue(healthDamage);
        }
        else if (enemy != null)
        {
            enemy.stats.health.DecreaseCurrentValue(healthDamage);
        }
    }

    public virtual void GetPostureDamage(float postureDamage)
    {
        if (player != null)
        {
            player.stats.posture.IncreaseCurrentValue(postureDamage);
        }
        else if (enemy != null)
        {
            enemy.stats.posture.IncreaseCurrentValue(postureDamage);
        }
    }

    /// <summary>
    /// Knockback time of 0 means that the knockback will be done when the entity hits the ground.
    /// </summary>
    public virtual void GetKnockback(Vector2 knockbackVelocity, float knockbackTime, bool constantVelocity, Ease? easeFunction, bool transitToKnockbackState)
    {
        if (player != null)
        {
            if (transitToKnockbackState)
            {
                player.knockbackState.SetKnockback(knockbackTime);//, knockbackVelocity);
                player.playerStateMachine.ChangeState(player.knockbackState);
            }

            player.shieldParryState.GotHit();

            if (constantVelocity)
            {
                player.movement.SetVelocity(knockbackVelocity);
            }
            else
            {
                player.movement.SetVelocityXChangeOverTime(knockbackVelocity.x, knockbackTime, easeFunction.Value, true);
                player.movement.SetVelocityY(knockbackVelocity.y);
            }
        }
        else if (enemy != null)
        {
            if (transitToKnockbackState)
            {
                enemy.enemyStateMachine.ChangeState(enemy.knockbackState);
            }
            
        }
    }

    public virtual void DoMeleeAttack()
    {
        
    }

    public virtual void DoRangedAttack()
    {
        
    }

    public bool CheckWithinAngle(Vector2 baseVector, Vector2 targetVector, float counterClockwiseAngle, float clockwiseAngle)
    {
        float angleBetweenVectors = -Vector2.SignedAngle(baseVector, targetVector);
        Debug.Log($"Angle between {baseVector} and {targetVector} is: {angleBetweenVectors}");
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
    }
}