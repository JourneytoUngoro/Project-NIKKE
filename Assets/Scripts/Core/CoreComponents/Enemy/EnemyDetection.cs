using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : Detection
{
    #region Check Transform
    [SerializeField] private Transform wallCheckTransform;
    [SerializeField] private Transform detectionRangeTransform;
    [SerializeField] private Transform aggroRangeTransform;
    [SerializeField] private Transform meleeAttackTransform;
    #endregion

    #region Check Variables
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float rangedAttackRadius;
    [SerializeField] private float meleeAttackRadius;
    [SerializeField] private float detectionRangeRadius;
    [SerializeField] private Vector2 detectionRangeSize;
    [SerializeField] private float aggroRangeRadius;
    [SerializeField] private Vector2 aggroRangeSize;
    #endregion

    #region Other Variables
    [SerializeField] private LayerMask whatIsPlayer;
    
    public GameObject target { get; private set; }
    #endregion

    public bool isDetectingWall()
    {
        return Physics2D.Raycast(wallCheckTransform.position, transform.right, wallCheckDistance, whatIsGround);
    }

    public bool isPlayerInDetectionRange()
    {
        Collider2D collider = detectionRangeRadius < float.Epsilon ? Physics2D.OverlapBox(detectionRangeTransform.position, detectionRangeSize, 0.0f, whatIsPlayer) : Physics2D.OverlapCircle(detectionRangeTransform.position, detectionRangeRadius, whatIsPlayer);
        target = collider != null ? collider.gameObject : null;
        return target != null;
    }

    public bool isPlayerInAggroRange()
    {
        Collider2D collider = aggroRangeRadius < float.Epsilon ? Physics2D.OverlapBox(aggroRangeTransform.position, aggroRangeSize, 0.0f, whatIsPlayer) : Physics2D.OverlapCircle(aggroRangeTransform.position, aggroRangeRadius, whatIsPlayer);
        target = collider != null ? collider.gameObject : null;
        return target != null;
    }

    public bool isPlayerInMeleeAttackRange()
    {
        return Physics2D.OverlapCircle(meleeAttackTransform.position, meleeAttackRadius, whatIsPlayer);
    }

    public bool isPlayerInRangedAttackRange()
    {
        return Physics2D.OverlapCircle(transform.position, rangedAttackRadius, whatIsPlayer);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        if (wallCheckTransform != null)
        {
            Gizmos.DrawRay(wallCheckTransform.position, transform.right * wallCheckDistance);
        }

        Gizmos.color = Color.blue;
        if (detectionRangeRadius > float.Epsilon)
        {
            Gizmos.DrawWireSphere(detectionRangeTransform.position, detectionRangeRadius);
        }
        else
        {
            Gizmos.DrawWireCube(detectionRangeTransform.position, detectionRangeSize);
        }
        if (aggroRangeRadius > float.Epsilon)
        {
            Gizmos.DrawWireSphere(aggroRangeTransform.position, aggroRangeRadius);
        }
        else
        {
            Gizmos.DrawWireCube(aggroRangeTransform.position, aggroRangeSize);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackTransform.position, meleeAttackRadius);
        Gizmos.DrawWireSphere(transform.position, rangedAttackRadius);
    }
}
