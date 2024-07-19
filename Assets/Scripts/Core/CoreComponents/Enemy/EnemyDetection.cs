using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDetection : Detection
{
    [SerializeField] private LayerMask whatIsChaseTarget;
    [SerializeField] private LayerMask whatIsAlertTarget;

    #region Check Transform
    [SerializeField] private Transform detectionRangeTransform;
    [SerializeField] private Transform aggroRangeTransform;
    [SerializeField] private Transform meleeAttackTransform;
    [SerializeField] private Transform rangedAttackTransform;
    #endregion

    #region Check Variables
    [SerializeField] private Transform jumpCheckTransform;
    [SerializeField] private float jumpCheckDistance;
    [SerializeField] private float alertRadius;
    [SerializeField] private float rangedAttackRadius;
    [SerializeField] private Vector2 rangedAttackSize;
    [SerializeField] private float meleeAttackRadius;
    [SerializeField] private Vector2 meleeAttackSize;
    [SerializeField] private float detectionRangeRadius;
    [SerializeField] private Vector2 detectionRangeSize;
    [SerializeField] private float aggroRangeRadius;
    [SerializeField] private Vector2 aggroRangeSize;
    #endregion

    #region Other Variables
    public GameObject target { get; private set; }
    #endregion

    public bool isDetectingWall()
    {
        return Physics2D.Raycast(wallCheckTransformTop.position, transform.right, wallCheckDistance, whatIsGround) || Physics2D.Raycast(wallCheckTransformBottom.position, transform.right, wallCheckDistance, whatIsGround);
        // return Physics2D.OverlapBox(wallCheckTransformTop.position, Vector2.one, 0.0f, whatIsGround);
    }

    public bool ShouldJump()
    {
        RaycastHit2D raycast = Physics2D.Raycast(jumpCheckTransform.position, transform.right, jumpCheckDistance, whatIsGround);
        
        if (raycast)
        {
            return Vector2.Angle(raycast.normal, -transform.right) < epsilon;
        }
        else return false;
    }

    public bool InStepbackDistance()
    {
        return Physics2D.Raycast(jumpCheckTransform.position, transform.right, jumpCheckDistance - 1, whatIsGround);
    }

    public bool isPlayerInDetectionRange()
    {
        Collider2D[] colliders = detectionRangeRadius < float.Epsilon ? Physics2D.OverlapBoxAll(detectionRangeTransform.position, detectionRangeSize, 0.0f, whatIsChaseTarget) : Physics2D.OverlapCircleAll(detectionRangeTransform.position, detectionRangeRadius, whatIsChaseTarget);

        Collider2D playerChaseTarget = colliders.Where(collider => collider.gameObject.TryGetComponent<Player>(out var comp)).FirstOrDefault();
        Collider2D otherChaseTarget = colliders.Where(collider => collider.gameObject.TryGetComponent<Enemy>(out var comp) && (1 << collider.gameObject.layer & whatIsChaseTarget) != 0).OrderBy(x => (transform.position - x.transform.position).magnitude).FirstOrDefault();

        if (playerChaseTarget != null)
        {
            target = playerChaseTarget.gameObject;
        }
        else if (otherChaseTarget != null)
        {
            target = otherChaseTarget.gameObject;
        }
        else
        {
            target = null;
        }

        return target != null;
    }

    public bool isPlayerInAggroRange()
    {
        Collider2D[] colliders = aggroRangeRadius < float.Epsilon ? Physics2D.OverlapBoxAll(aggroRangeTransform.position, aggroRangeSize, 0.0f, whatIsChaseTarget) : Physics2D.OverlapCircleAll(aggroRangeTransform.position, aggroRangeRadius, whatIsChaseTarget);

        Collider2D playerChaseTarget = colliders.Where(collider => collider.gameObject.TryGetComponent<Player>(out var comp)).FirstOrDefault();
        Collider2D otherChaseTarget = colliders.Where(collider => collider.gameObject.TryGetComponent<Enemy>(out var comp) && (1 << collider.gameObject.layer & whatIsChaseTarget) != 0).OrderBy(x => (transform.position - x.transform.position).magnitude).FirstOrDefault();

        if (playerChaseTarget != null)
        {
            target = playerChaseTarget.gameObject;
        }
        else if (otherChaseTarget != null)
        {
            target = otherChaseTarget.gameObject;
        }
        else
        {
            target = null;
        }

        return target != null;

        /*if (colliders.Length > 0)
        {
            Collider2D player = colliders.Where(collider => collider.gameObject.TryGetComponent<Player>(out var comp)).ToList()[0];
            Collider2D chaseTarget = colliders.Where(collider => collider.gameObject.TryGetComponent<Enemy>(out var comp) && (1 << collider.gameObject.layer & whatIsChaseTarget) != 0).OrderBy(x => (transform.position - x.transform.position).magnitude).ToList()[0];

            target = player != null ? player.gameObject : chaseTarget.gameObject;
        }
        else
        {
            target = null;
        }
        return target != null;*/
    }

    public bool isPlayerInMeleeAttackRange()
    {
        return Physics2D.OverlapCircle(meleeAttackTransform.position, meleeAttackRadius, whatIsChaseTarget);
    }

    public bool isPlayerInRangedAttackRange()
    {
        return Physics2D.OverlapCircle(transform.position, rangedAttackRadius, whatIsChaseTarget);
    }

    public void DoAlert()
    {
        Collider2D[] alertTargets = Physics2D.OverlapCircleAll(transform.position, alertRadius, whatIsAlertTarget);

        foreach (Collider2D alertTarget in alertTargets)
        {
            alertTarget.SendMessage("GetAlerted");
        }
    }

    public void GetAlerted()
    {
        if (enemy.sleepState.isSleeping)
        {
            enemy.sleepState.WakeUp();
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;

        Gizmos.DrawLine(jumpCheckTransform.position, jumpCheckTransform.position + transform.right * jumpCheckDistance);

        Gizmos.color = Color.blue;
        if (detectionRangeRadius > epsilon)
        {
            Gizmos.DrawWireSphere(detectionRangeTransform.position, detectionRangeRadius);
        }
        else
        {
            Gizmos.DrawWireCube(detectionRangeTransform.position, detectionRangeSize);
        }
        if (aggroRangeRadius > epsilon)
        {
            Gizmos.DrawWireSphere(aggroRangeTransform.position, aggroRangeRadius);
        }
        else
        {
            Gizmos.DrawWireCube(aggroRangeTransform.position, aggroRangeSize);
        }
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.red;
        if (rangedAttackRadius > epsilon)
        {
            Gizmos.DrawWireSphere(rangedAttackTransform.position, rangedAttackRadius);
        }
        else
        {
            Gizmos.DrawWireCube(rangedAttackTransform.position, rangedAttackSize);
        }
        if (meleeAttackRadius > epsilon)
        {
            Gizmos.DrawWireSphere(meleeAttackTransform.position, meleeAttackRadius);
        }
        else
        {
            Gizmos.DrawWireCube(meleeAttackTransform.position, meleeAttackSize);
        }
    }
}
