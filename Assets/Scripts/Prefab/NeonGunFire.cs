using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonGunFire : PooledObject
{
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform damageRangeTransform;
    [SerializeField] private Vector2 damageRange;
    [SerializeField] private EnemyAttackInfo enemyAttackInfo;

    public void DoDamage()
    {
        Collider2D[] damageTargets = Physics2D.OverlapBoxAll(damageRangeTransform.position, damageRange, 0.0f, whatIsDamageable);
        
        foreach (Collider2D damageTarget in damageTargets)
        {
            // damageTarget.gameObject.GetComponentInChildren<Combat>().GetDamage(enemyAttackInfo);
        }
    }

    public void SetEnemyAttackInfo(EnemyAttackInfo enemyAttackInfo)
    {
        this.enemyAttackInfo = enemyAttackInfo;
    }

    private void Awake()
    {
        enemyAttackInfo.attackSubject = transform.gameObject;
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(damageRangeTransform.position, damageRange);
    }
}
