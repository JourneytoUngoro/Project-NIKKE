using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : Combat
{
    [SerializeField] private float blockMultiplier;

    public void MeleeAttack()
    {
        /*Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(meleeAttackTransform.position, meleeAttackRadius, whatIsDamageable);
        
        if (Array.Exists(detectedObjects, x => x.CompareTag("BlockParry")))
        {
            foreach (var detectedObject in detectedObjects)
            {
                if (detectedObject.CompareTag("Player"))
                {
                    EnemyAttackInfo combatInfo = new EnemyAttackInfo(gameObject, damage * blockMultiplier, poiseDamage);
                    detectedObject.SendMessage("GetDamage", combatInfo);
                    break;
                }
            }
        }
        else
        {
            foreach (var detectedObject in detectedObjects)
            {
                if (detectedObject.CompareTag("Player"))
                {
                    EnemyAttackInfo combatInfo = new EnemyAttackInfo(gameObject, damage, poiseDamage);
                    detectedObject.SendMessage("GetDamage", combatInfo);
                    break;
                }
            }
        }*/
    }

    public override void GetDamage(AttackInfo attackInfo)
    {
        base.GetDamage(attackInfo);

        PlayerAttackInfo playerAttackInfo = attackInfo as PlayerAttackInfo;

        enemy.enemyStateMachine.currentState.gotHit = true;
        enemy.stats.health.DecreaseCurrentValue(playerAttackInfo.damage);
        enemy.stats.posture.DecreaseCurrentValue(playerAttackInfo.postureDamage);
    }

    public void RangedAttack()
    {
        if (projectile == null) return;

        Collider2D detectedObject = Physics2D.OverlapCircle(transform.position, 20.0f, whatIsDamageable);
        GameObject temp = Instantiate(projectile, transform.position, Quaternion.identity);
        /*Vector2? projectileAngle = CalculateAngle(transform.position, detectedObject.transform.position, 10.0f, 1);
        if (projectileAngle != null)
        {
            temp.GetComponent<Rigidbody2D>().velocity = (Vector2)projectileAngle * 10.0f;
        }
        else
        {
            Destroy(temp);
        }*/
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
}
