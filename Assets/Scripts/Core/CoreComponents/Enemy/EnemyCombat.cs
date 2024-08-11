using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : Combat
{
    [field: SerializeField] public Transform midRAttackTransform { get; private set; }
    [SerializeField] private float midRAttackRadius;
    [SerializeField] private Vector2 midRAttackSize;

    public bool isTargetInMeleeAttackRange()
    {
        if (meleeAttackRadius > epsilon)
        {
            return Physics2D.OverlapCircle(meleeAttackTransform.position, meleeAttackRadius, whatIsDamageable);
        }
        else if (meleeAttackSize.x > epsilon && meleeAttackSize.y > epsilon)
        {
            return Physics2D.OverlapBox(meleeAttackTransform.position, meleeAttackSize, 0.0f, whatIsDamageable);
        }
        else return false;
    }

    public bool isTargetInMidRAttackRange()
    {
        if (midRAttackRadius > epsilon)
        {
            return Physics2D.OverlapCircle(midRAttackTransform.position, midRAttackRadius, whatIsDamageable);
        }
        else if (midRAttackSize.x > epsilon && midRAttackSize.y > epsilon)
        {
            return Physics2D.OverlapBox(midRAttackTransform.position, midRAttackSize, 0.0f, whatIsDamageable);
        }
        else return false;
    }

    public bool isTargetInRangedAttackRange()
    {
        if (rangedAttackRadius > epsilon)
        {
            return Physics2D.OverlapCircle(rangedAttackTransform.position, rangedAttackRadius, whatIsDamageable);
        }
        else if (rangedAttackSize.x > epsilon && rangedAttackSize.y > epsilon)
        {
            return Physics2D.OverlapBox(rangedAttackTransform.position, rangedAttackSize, 0.0f, whatIsDamageable);
        }
        else return false;
    }

    public override void DoMeleeAttack()
    {
        Collider2D[] damageTargets = { };

        if (meleeAttackRadius > epsilon)
        {
            damageTargets = Physics2D.OverlapCircleAll(meleeAttackTransform.position, meleeAttackRadius, whatIsDamageable);
        }
        else if (meleeAttackSize.x > epsilon && meleeAttackSize.y > epsilon)
        {
            damageTargets = Physics2D.OverlapBoxAll(meleeAttackTransform.position, meleeAttackSize, 0.0f, whatIsDamageable);
        }

        foreach(Collider2D damageTarget in damageTargets)
        {
            if (!damagedTargets.Contains(damageTarget))
            {
                enemy.enemyData.meleeAttackInfo.attackSubject = enemy.gameObject;
                Debug.Log("Do damage to: " + damageTarget.gameObject.name);
                damageTarget.gameObject.GetComponentInChildren<Combat>().GetDamage(enemy.enemyData.meleeAttackInfo);
                damagedTargets.Add(damageTarget);
            }
        }
    }

    public override void GetDamage(AttackInfo attackInfo)
    {
        base.GetDamage(attackInfo);

        PlayerAttackInfo playerAttackInfo = attackInfo as PlayerAttackInfo;

        enemy.enemyStateMachine.currentState.gotHit = true;
        enemy.stats.health.DecreaseCurrentValue(playerAttackInfo.healthDamage);
        enemy.stats.posture.IncreaseCurrentValue(playerAttackInfo.postureDamage);
    }

    public override void GetPostureDamage(float postureDamage)
    {
        base.GetPostureDamage(postureDamage);

        enemy.enemyStateMachine.currentState.gotHit = true;
    }

    public void FireProjectile(string objectName, Vector2 projectileFirePosition, Vector2? targetPosition, float projectileSpeed, float projectileGravityScale)
    {
        GameObject projectile = Manager.Instance.objectPoolingManager.GetGameObject(objectName);
        projectile.GetComponent<Explosion>().SetAttackSubject(gameObject);
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;

        if (midRAttackRadius > epsilon)
        {
            Gizmos.DrawWireSphere(midRAttackTransform.position, midRAttackRadius);
        }
        else if (midRAttackSize.x > epsilon && midRAttackSize.y > epsilon)
        {
            Gizmos.DrawWireCube(midRAttackTransform.position, midRAttackSize);
        }
    }
}
