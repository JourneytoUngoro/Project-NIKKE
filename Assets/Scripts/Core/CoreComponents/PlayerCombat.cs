using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombat : Combat
{
    [SerializeField] private Transform dashAttackTransform;
    [SerializeField] private float dashAttackRadius;

    private List<Collider2D> damagedTargets;

    private Player player;

    public override void Awake()
    {
        player = GetComponent<Player>();

        damagedTargets = new List<Collider2D>();
    }

    public void ClearDamagedTargets()
    {
        damagedTargets.Clear();
    }

    public override void GetDamage(CombatInfo combatInfo)
    {
        if (player.playerStateMachine.currentState == player.blockParryState)
        {
            if (player.blockParryState.isParrying)
            {
                combatInfo.sender.SendMessage("GetPoiseDamage", poiseDamage);
            }
            else
            {
                Debug.Log("Player got damaged by " + combatInfo.damage);
                player.stats.health.DecreaseCurrentValue(combatInfo.damage);
            }
        }
        else
        {
            Debug.Log("Player got damaged by " + combatInfo.damage);
            player.stats.health.DecreaseCurrentValue(combatInfo.damage);
        }
    }

    public override void DoMeleeAttack(float damage, Vector2 center, float radius)
    {
        base.DoMeleeAttack(damage, center, radius);

        Collider2D[] damageTargets = Physics2D.OverlapCircleAll(center, radius, whatIsDamageable);

        foreach (Collider2D damageTarget in damageTargets)
        {
            if (!damagedTargets.Contains(damageTarget))
            {
                damageTarget.SendMessage("GetDamage", damage);
                damagedTargets.Add(damageTarget);
            }
        }
    }

    public override void DoMeleeAttack(float damage, Vector2 center, Vector2 size, float angle)
    {
        base.DoMeleeAttack(damage, center, size, angle);
    }

    public override void GetPostureDamage(float damage)
    {
        base.GetPostureDamage(damage);
    }

    public override void DoRangedAttack()
    {
        base.DoRangedAttack();

        if (Physics2D.OverlapCircleAll(meleeAttackTransform.position, meleeAttackRadius, whatIsDamageable).Length == 0)
        {
            List<Collider2D> detectedObjects = Physics2D.OverlapBoxAll(rangedAttackTransform.position, rangedAttackSize, 0.0f, whatIsDamageable).ToList();
            if (detectedObjects.Count > 0)
            {
                Collider2D target = detectedObjects.OrderBy(x => (entityCenterTransform.position - x.transform.position).magnitude).ToList()[0];
                if (!Physics2D.Raycast(entityCenterTransform.position, target.transform.position - entityCenterTransform.position, Vector2.Distance(entityCenterTransform.position, target.transform.position), whatIsGround))
                {
                    player.normalAttackState.DecreaseAmmo();
                    target.SendMessage("GetDamage", player.PlayerData.rangedAttackDamage);
                }
            }
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (dashAttackTransform != null)
        {
            Gizmos.DrawWireSphere(dashAttackTransform.position, dashAttackRadius);
        }
    }
}
