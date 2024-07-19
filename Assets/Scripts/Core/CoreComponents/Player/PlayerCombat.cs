using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombat : Combat
{
    [SerializeField] private Transform jumpAttackTransform;
    [SerializeField] private Vector2 jumpAttackSize;
    [SerializeField] private Transform jumpFinishAttackTransform;
    [SerializeField] private float jumpFinishAttackRadius;
    [SerializeField] [Range(0, 180)] private float jumpFinishAttackClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float jumpFinishAttackCounterClockwiseAngle;

    private List<Collider2D> damagedTargets;

    protected override void Awake()
    {
        base.Awake();

        damagedTargets = new List<Collider2D>();
    }

    public void ClearDamagedTargets()
    {
        damagedTargets.Clear();
    }

    public override void GetDamage(AttackInfo combatInfo)
    {
        base.GetDamage(combatInfo);

        EnemyAttackInfo enemyAttackInfo = combatInfo as EnemyAttackInfo;

        if (player.playerStateMachine.currentState == player.shieldParryState)
        {
            if (player.shieldParryState.isParrying)
            {
                player.stats.health.DecreaseCurrentValue(enemyAttackInfo.damageWhenParried);
                player.stats.posture.DecreaseCurrentValue(enemyAttackInfo.postureDamageWhenParried);
                enemyAttackInfo.attackSubject.SendMessage("GetPoiseDamage", enemyAttackInfo);
            }
            else
            {
                Debug.Log("Player got damaged by " + combatInfo.damage);
                player.stats.health.DecreaseCurrentValue(enemyAttackInfo.damageWhenShielded);
                player.stats.posture.DecreaseCurrentValue(enemyAttackInfo.postureDamageWhenParried);
            }
        }
        else
        {
            Debug.Log("Player got damaged by " + combatInfo.damage);
            player.stats.health.DecreaseCurrentValue(enemyAttackInfo.damage);
            player.stats.posture.DecreaseCurrentValue(enemyAttackInfo.postureDamage);
        }
    }

    public override void GetPostureDamage(float damage)
    {
        base.GetPostureDamage(damage);
    }

    public override void DoMeleeAttack()
    {
        base.DoMeleeAttack();

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
                    target.SendMessage("GetDamage", player.playerData.playerRangedAttackInfo.damage);
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

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (jumpAttackTransform != null && jumpFinishAttackTransform != null)
        {
            Gizmos.DrawWireSphere(jumpFinishAttackTransform.position, jumpFinishAttackRadius);
            Gizmos.DrawLine(jumpFinishAttackTransform.position, jumpFinishAttackTransform.position + Quaternion.AngleAxis(-jumpFinishAttackClockwiseAngle, Vector3.forward) * transform.up * jumpFinishAttackRadius);
            Gizmos.DrawLine(jumpFinishAttackTransform.position, jumpFinishAttackTransform.position + Quaternion.AngleAxis(jumpFinishAttackCounterClockwiseAngle, Vector3.forward) * transform.up * jumpFinishAttackRadius);
            Gizmos.DrawWireCube(jumpAttackTransform.position, jumpAttackSize);
        }
    }
}
