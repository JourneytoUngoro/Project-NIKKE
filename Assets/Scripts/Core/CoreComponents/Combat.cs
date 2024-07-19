using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* Incomplete */
/* Needs to be optimized and improved */

public class Combat : CoreComponent
{
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsDamageable;
    [SerializeField] protected Transform meleeAttackTransform;
    [SerializeField] protected float meleeAttackRadius;
    [SerializeField] protected Transform rangedAttackTransform;
    [SerializeField] protected Vector2 rangedAttackSize;

    [SerializeField] protected GameObject projectile;

    public virtual void GetDamage(AttackInfo combatInfo)
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

    public virtual void DoMeleeAttack()
    {
        
    }

    public virtual void DoRangedAttack()
    {
        
    }

    public bool CheckWithinAngle(Vector2 baseVector, Vector2 targetVector, float counterClockwiseAngle, float clockwiseAngle)
    {
        float angleBetweenVectors = -Vector2.SignedAngle(baseVector, targetVector);
        return counterClockwiseAngle <= angleBetweenVectors && angleBetweenVectors <= clockwiseAngle;
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