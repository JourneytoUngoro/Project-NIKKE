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
    [SerializeField] private Transform midRAttackTransform;
    [SerializeField] private Transform rangedAttackTransform;
    #endregion

    #region Check Variables
    [SerializeField] private Transform jumpCheckTransform;
    [SerializeField] private float jumpCheckDistance;
    [SerializeField] private float alertRadius;
    [SerializeField] private float meleeAttackRadius;
    [SerializeField] private Vector2 meleeAttackSize;
    [SerializeField] private float midRAttackRadius;
    [SerializeField] private Vector2 midRAttackSize;
    [SerializeField] private float rangedAttackRadius;
    [SerializeField] private Vector2 rangedAttackSize;
    [SerializeField] private float detectionRangeRadius;
    [SerializeField] private Vector2 detectionRangeSize;
    [SerializeField] private float aggroRangeRadius;
    [SerializeField] private Vector2 aggroRangeSize;
    #endregion

    #region Other Variables
    public GameObject target { get; private set; }
    private Enemy enemy;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
    }

    public bool isDetectingWall()
    {
        RaycastHit2D rayHitTop = Physics2D.Raycast(wallCheckTransformTop.position, transform.right, wallCheckDistance, whatIsGround);
        RaycastHit2D rayHitBottom = Physics2D.Raycast(wallCheckTransformBottom.position, transform.right, wallCheckDistance, whatIsGround);

        if (rayHitTop)
        {
            return true;
        }
        else if (rayHitBottom)
        {
            if (Vector2.Angle(rayHitBottom.normal, -transform.right) < epsilon)
            {
                return true;
            }
            else return false;
        }

        return false;
    }

    public bool ShouldJump()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(jumpCheckTransform.position, transform.right, jumpCheckDistance, whatIsGround);
        
        if (rayHit)
        {
            return Vector2.Angle(rayHit.normal, -transform.right) < epsilon;
        }
        else return false;
    }

    public bool InStepbackDistance()
    {
        return Physics2D.Raycast(jumpCheckTransform.position, transform.right, jumpCheckDistance - 1, whatIsGround);
    }

    public bool isTargetInDetectionRange()
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
    }
}
