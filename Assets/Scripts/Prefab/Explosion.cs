using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : AggressiveObject
{
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform damageRangeTransform;
    [SerializeField] private float damageRadius;
    [SerializeField] private Vector2 damageSize;

    private List<Collider2D> damagedTargets;

    protected float epsilon = 0.001f;

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    public void DoDamage()
    {
        Collider2D[] damageTargets = { };

        if (damageRadius > epsilon)
        {
            damageTargets = Physics2D.OverlapCircleAll(damageRangeTransform.position, damageRadius, whatIsDamageable);
        }
        else if (damageSize.x > epsilon && damageSize.y > epsilon)
        {
            damageTargets = Physics2D.OverlapBoxAll(damageRangeTransform.position, damageSize, 0.0f, whatIsDamageable);
        }
          
        foreach (Collider2D damageTarget in damageTargets)
        {
            if(!damagedTargets.Contains(damageTarget))
            {
                // damageTarget.gameObject.GetComponentInChildren<Combat>().GetDamage(enemyAttackInfo);

            }
        }
    }

    private void Awake()
    {
        damagedTargets = new List<Collider2D>();
    }

    private void OnEnable()
    {
        enemyAttackInfo.attackSubject = gameObject;
    }

    private void OnDisable()
    {
        damagedTargets.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(damageRangeTransform.position, damageSize);
        Gizmos.DrawWireSphere(damageRangeTransform.position, damageRadius);
    }
}
