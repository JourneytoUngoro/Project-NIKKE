using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* Incomplete */
/* Needs to be optimized and improved */

public class Combat : CoreComponent
{
    [SerializeField] protected LayerMask whatIsDamageable;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform entityCenterTransform;
    [SerializeField] protected Transform meleeAttackTransform;
    [SerializeField] protected float meleeAttackRadius;
    [SerializeField] protected Transform rangedAttackTransform;
    [SerializeField] protected Vector2 rangedAttackSize;

    [SerializeField] protected GameObject projectile;

    [SerializeField] protected float damage;
    [SerializeField] protected float poiseDamage;

    public virtual void Awake()
    {
        
    }

    public virtual void GetDamage(CombatInfo combatInfo)
    {
        if (gameObject.layer != LayerMask.NameToLayer("Invincible"))
        {
            Debug.Log(gameObject.name + " got damaged by " + combatInfo.damage);
        }
    }

    public virtual void GetPostureDamage(float damage)
    {
        if (gameObject.layer != LayerMask.NameToLayer("Invincible"))
        {
            Debug.Log(gameObject.name + " got poise damaged by " + damage);
        }
    }

    public virtual void DoMeleeAttack(float damage, Vector2 center, float radius)
    {
        
    }

    public virtual void DoMeleeAttack(float damage, Vector2 center, Vector2 size, float angle)
    {
        
    }

    public virtual void DoRangedAttack()
    {
        
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (meleeAttackTransform != null)
        {
            Gizmos.DrawWireSphere(meleeAttackTransform.position, meleeAttackRadius);
        }
        if (rangedAttackTransform != null)
        {
            Gizmos.DrawWireCube(rangedAttackTransform.position, rangedAttackSize);
        }
    }
}