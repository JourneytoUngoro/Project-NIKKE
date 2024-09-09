using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PlayerCombat : Combat
{
    [SerializeField] private Transform jumpAttackTransform;
    [SerializeField] private Vector2 jumpAttackSize;
    [SerializeField] private Transform jumpFinishAttackTransform;
    [SerializeField] private float jumpFinishAttackRadius;
    [SerializeField] [Range(0, 180)] private float jumpFinishAttackClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float jumpFinishAttackCounterClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float shieldParryAreaClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float shieldParryAreaCounterClockwiseAngle;
    [SerializeField] protected List<CombatAbilityWithTransform> shieldParry;

    protected override void Awake()
    {
        base.Awake();

        /*foreach (MemberInfo memberInfo in this.GetType().GetMembers())
        {
            Debug.Log(memberInfo.MemberType + ": " + memberInfo.Name);
        }
        Debug.Log("------------------------------------");*/
        foreach (FieldInfo memberInfo in typeof(Combat).GetFields())//.Where(field => field.FieldType == typeof(Transform)))
        {
            Debug.Log(memberInfo.MemberType + ": " + memberInfo.Name);
        }
    }

    /*public override void GetDamage(AttackInfo combatInfo)
    {
        base.GetDamage(combatInfo);

        EnemyAttackInfo enemyAttackInfo = combatInfo as EnemyAttackInfo;
        Debug.Log("Damage by " + enemyAttackInfo.attackSubject.name);
        if (player.gameObject.layer == LayerMask.NameToLayer("PlayerParry"))
        {
            if (inShieldParryAngle(enemyAttackInfo.attackSubject.gameObject.transform.position))
            {
                Debug.Log("parry succeed");
                player.stats.health.DecreaseCurrentValue(enemyAttackInfo.damageWhenParried);
                player.stats.posture.IncreaseCurrentValue(enemyAttackInfo.postureDamageWhenParried, false);
                enemyAttackInfo.attackSubject.GetComponentInChildren<Combat>().GetPostureDamage(enemyAttackInfo.counterPostureDamageWhenParried);
                enemyAttackInfo.attackSubject.GetComponentInChildren<Combat>().GetKnockback(enemyAttackInfo.counterKnockbackVelocityWhenParried, enemyAttackInfo.counterKnockbackTimeWhenParried, false, Ease.InCubic, true);
            }
            else
            {
                Debug.Log("failed to parry");
            }
        }
        else
        {
            Debug.Log("Player got damaged by " + combatInfo.healthDamage);
            player.stats.health.DecreaseCurrentValue(enemyAttackInfo.healthDamage);
            player.stats.posture.IncreaseCurrentValue(enemyAttackInfo.postureDamage);
            
            if (enemyAttackInfo.attackSubject.transform.position.x < transform.position.x)
            {
                workSpace.Set(enemyAttackInfo.knockbackVelocityWhenHit.x, enemyAttackInfo.knockbackVelocityWhenHit.y);
            }
            else
            {
                workSpace.Set(-enemyAttackInfo.knockbackVelocityWhenHit.x, enemyAttackInfo.knockbackVelocityWhenHit.y);
            }
            GetKnockback(workSpace, enemyAttackInfo.knockbackTimeWhenHit, true, null, true);
        }
        *//*if (player.playerStateMachine.currentState == player.shieldParryState)
        {
            if (player.shieldParryState.isParrying)
            {
                if (inShieldParryAngle(enemyAttackInfo.attackSubject.gameObject.transform.position))
                {
                    player.stats.health.DecreaseCurrentValue(enemyAttackInfo.damageWhenParried);
                    player.stats.posture.IncreaseCurrentValue(enemyAttackInfo.postureDamageWhenParried, false);
                    enemyAttackInfo.attackSubject.GetComponentInChildren<Combat>().GetPostureDamage(enemyAttackInfo.counterPostureDamageWhenParried, true);
                    enemyAttackInfo.attackSubject.GetComponentInChildren<Movement>().SetVelocity(enemyAttackInfo.counterKnockbackVelocityWhenParried);
                }
            }
            else
            {
                Debug.Log("Player got damaged by " + combatInfo.damage);
                player.stats.health.DecreaseCurrentValue(enemyAttackInfo.damageWhenShielded);
                player.stats.posture.IncreaseCurrentValue(enemyAttackInfo.postureDamageWhenParried);
            }
        }
        else
        {
            Debug.Log("Player got damaged by " + combatInfo.damage);
            player.stats.health.DecreaseCurrentValue(enemyAttackInfo.damage);
            player.stats.posture.IncreaseCurrentValue(enemyAttackInfo.postureDamage);
        }*//*
    }*/

    public override void GetHealthDamage(DamageComponent damageComponent)
    {
        if (player.playerStateMachine.currentState.Equals(player.shieldParryState))
        {
            if (player.shieldParryState.isParrying)
            {

            }
        }
    }

    public override void GetPostureDamage(DamageComponent damageComponent)
    {
        
    }

    public void DoMeleeAttack()
    {
        Collider2D[] damageTargets = Physics2D.OverlapCircleAll(meleeAttackTransform.position, meleeAttackRadius, whatIsDamageable);

        foreach (Collider2D damageTarget in damageTargets)
        {
            damageTarget.SendMessage("GetDamage", player.playerData.playerMeleeAttackInfo);
        }
    }

    public override void DoRangedAttack()
    {
        base.DoRangedAttack();

        if (Physics2D.OverlapCircleAll(meleeAttackTransform.position, meleeAttackRadius, whatIsDamageable).Length == 0)
        {
            List<Collider2D> detectedObjects = Physics2D.OverlapBoxAll(rangedAttackTransform.position, rangedAttackSize, 0.0f, whatIsDamageable).ToList();
            if (detectedObjects.Count > 0)
            {
                Collider2D target = detectedObjects.OrderBy(x => (transform.position - x.transform.position).magnitude).ToList()[0];
                if (!Physics2D.Raycast(transform.position, target.transform.position - transform.position, Vector2.Distance(transform.position, target.transform.position), whatIsGround))
                {
                    player.meleeAttackState.DecreaseAmmo();
                    target.SendMessage("GetDamage", player.playerData.playerRangedAttackInfo.healthDamage);
                    target.SendMessage("GetPostureDamage", player.playerData.playerRangedAttackInfo.postureDamage);
                }
            }
        }
    }

    public void DoJumpAttack()
    {
        Collider2D[] damageTargets = Physics2D.OverlapBoxAll(jumpAttackTransform.position, jumpAttackSize, whatIsDamageable);

        foreach (Collider2D damageTarget in damageTargets)
        {
            if (!damagedTargets.Contains(damageTarget))
            {
                damageTarget.SendMessage("GetDamage", player.playerData.playerJumpAttackInfo);
                damagedTargets.Add(damageTarget);
            }
        }
    }

    public void DoJumpFinishAttack()
    {
        Collider2D[] damageTargets = Physics2D.OverlapCircleAll(jumpFinishAttackTransform.position, jumpFinishAttackRadius, whatIsDamageable);

        foreach (Collider2D damageTarget in damageTargets)
        {
            Vector2 targetVector = damageTarget.transform.position - jumpFinishAttackTransform.position;
            if (CheckWithinAngle(transform.up, targetVector, jumpFinishAttackCounterClockwiseAngle, jumpFinishAttackClockwiseAngle))
            {
                damageTarget.SendMessage("GetDamage", player.playerData.playerJumpAttackInfo);
            }
        }
    }

    public void DoUltimateAttack()
    {

    }

    public bool inShieldParryAngle(Vector2 targetPosition)
    {
        return CheckWithinAngle(transform.right, targetPosition - (Vector2)transform.position, shieldParryAreaClockwiseAngle, shieldParryAreaCounterClockwiseAngle);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (jumpAttackTransform != null && jumpFinishAttackTransform != null)
        {
            Gizmos.DrawWireSphere(jumpFinishAttackTransform.position, jumpFinishAttackRadius);
            Gizmos.DrawLine(jumpFinishAttackTransform.position, jumpFinishAttackTransform.position + Quaternion.AngleAxis(-jumpFinishAttackClockwiseAngle, Vector3.forward) * transform.up * jumpFinishAttackRadius);
            Gizmos.DrawLine(jumpFinishAttackTransform.position, jumpFinishAttackTransform.position + Quaternion.AngleAxis(jumpFinishAttackCounterClockwiseAngle, Vector3.forward) * transform.up * jumpFinishAttackRadius);
            Gizmos.DrawWireCube(jumpAttackTransform.position, jumpAttackSize);
        }

        foreach(CombatAbilityWithTransform combatAbility in shieldParry)
        {
            if (combatAbility.overlapCollider.overlapBox)
            {
                Gizmos.DrawWireCube(combatAbility.centerTransform.position, combatAbility.overlapCollider.boxSize);

                if (combatAbility.overlapCollider.limitAngle)
                { 
                    Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position + combatAbility.centerTransform.right * combatAbility.overlapCollider.boxSize.x / 2.0f);

                    if (combatAbility.overlapCollider.clockwiseAngle < Mathf.Atan2(combatAbility.overlapCollider.boxSize.y, combatAbility.overlapCollider.boxSize.x) * Mathf.Rad2Deg)
                    {
                        Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position + Quaternion.AngleAxis(-combatAbility.overlapCollider.clockwiseAngle,    combatAbility.centerTransform.forward) * combatAbility.centerTransform.right * combatAbility.overlapCollider.boxSize.x / Mathf.Cos(combatAbility.overlapCollider.clockwiseAngle *  Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (combatAbility.overlapCollider.clockwiseAngle > 180.0f - Mathf.Atan2(combatAbility.overlapCollider.boxSize.y, combatAbility.overlapCollider.boxSize.x) * Mathf.Rad2Deg)
                    {
                        Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position - Quaternion.AngleAxis(-combatAbility.overlapCollider.clockwiseAngle,    combatAbility.centerTransform.forward) * combatAbility.centerTransform.right * combatAbility.overlapCollider.boxSize.x / Mathf.Cos(combatAbility.overlapCollider.clockwiseAngle *  Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position + Quaternion.AngleAxis(-combatAbility.overlapCollider.clockwiseAngle,    combatAbility.centerTransform.forward) * combatAbility.centerTransform.right * combatAbility.overlapCollider.boxSize.y / Mathf.Cos((90.0f -    combatAbility.overlapCollider.clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }

                    if (combatAbility.overlapCollider.counterClockwiseAngle < Mathf.Atan2(combatAbility.overlapCollider.boxSize.y, combatAbility.overlapCollider.boxSize.x) * Mathf.Rad2Deg)
                    {
                        Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position + Quaternion.AngleAxis(combatAbility.overlapCollider.counterClockwiseAngle,  combatAbility.centerTransform.forward) * combatAbility.centerTransform.right * combatAbility.overlapCollider.boxSize.x / Mathf.Cos(combatAbility.overlapCollider.counterClockwiseAngle    * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (combatAbility.overlapCollider.counterClockwiseAngle > 180.0f - Mathf.Atan2(combatAbility.overlapCollider.boxSize.y, combatAbility.overlapCollider.boxSize.x) * Mathf.Rad2Deg)
                    {
                        Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position - Quaternion.AngleAxis(combatAbility.overlapCollider.counterClockwiseAngle,  combatAbility.centerTransform.forward) * combatAbility.centerTransform.right * combatAbility.overlapCollider.boxSize.x / Mathf.Cos(combatAbility.overlapCollider.counterClockwiseAngle    * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position + Quaternion.AngleAxis(combatAbility.overlapCollider.counterClockwiseAngle,  combatAbility.centerTransform.forward) * combatAbility.centerTransform.right * combatAbility.overlapCollider.boxSize.y / Mathf.Cos((90.0f -  combatAbility.overlapCollider.counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
            }
            else if (combatAbility.overlapCollider.overlapCircle)
            {
                Gizmos.DrawWireSphere(combatAbility.centerTransform.position, combatAbility.overlapCollider.circleRadius);

                if (combatAbility.overlapCollider.limitAngle)
                {
                    Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position + combatAbility.centerTransform.right * combatAbility.overlapCollider.circleRadius);
                    Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position + Quaternion.AngleAxis(-combatAbility.overlapCollider.clockwiseAngle, combatAbility.centerTransform.forward) * combatAbility.centerTransform.right * combatAbility.overlapCollider.circleRadius);
                    Gizmos.DrawLine(combatAbility.centerTransform.position, combatAbility.centerTransform.position + Quaternion.AngleAxis(combatAbility.overlapCollider.counterClockwiseAngle, combatAbility.centerTransform.forward) * combatAbility.centerTransform.right * combatAbility.overlapCollider.circleRadius);
                }
            }
        }

        Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-shieldParryAreaClockwiseAngle, Vector3.forward) * transform.right * 8.0f);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(shieldParryAreaCounterClockwiseAngle, Vector3.forward) * transform.right * 8.0f);//player.GetComponent<Collider2D>().bounds.size.y / 2.0f);
    }
}
