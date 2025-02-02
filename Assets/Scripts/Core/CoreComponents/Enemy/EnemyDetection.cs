using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDetection : Detection
{
    [field: SerializeField] public LayerMask whatIsChaseTarget { get; private set; }
    [field: SerializeField] public LayerMask whatIsAlertTarget { get; private set; }

    #region Check Transform
    [SerializeField] protected OverlapCollider detectionRange;
    [SerializeField] protected OverlapCollider aggroRange;
    #endregion

    #region Check Variables
    [SerializeField] private Transform jumpCheckTransform;
    [SerializeField] private float jumpCheckDistance;
    [SerializeField] private float alertRadius;
    #endregion

    #region Other Variables
    public Vector2 currentTargetLastVelocity { get; private set; }
    public Vector2 currentTargetLastPosition { get; private set; }
    private Enemy enemy;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
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

    // TODO: 시야 장애물 정보
    public bool isTargetInDetectionRange()
    {
        Collider2D[] detectionRangeColliders = new Collider2D[0];

        if (detectionRange.overlapBox)
        {
            detectionRangeColliders = Physics2D.OverlapBoxAll(detectionRange.centerTransform.position, detectionRange.boxSize, detectionRange.boxRotation, whatIsChaseTarget);
        }
        else if (detectionRange.overlapCircle)
        {
            detectionRangeColliders = Physics2D.OverlapCircleAll(detectionRange.centerTransform.position, detectionRange.circleRadius, whatIsChaseTarget);
        }

        Entity[] detectionRangeEntities = detectionRangeColliders.Select(collider => collider.GetComponent<Entity>()).ToArray();

        if (detectionRange.limitAngle)
        {
            int multiplier = enemy.transform.rotation.y == 0 ? 1 : -1;
            Vector2 baseVector = enemy.transform.right.Rotate(multiplier * detectionRange.centerRotation * Mathf.Deg2Rad);
            detectionRangeEntities = detectionRangeEntities.Where(collider => enemy.combat.CheckWithinAngle(baseVector, collider.transform.position - enemy.transform.position, detectionRange.counterClockwiseAngle, detectionRange.clockwiseAngle)).ToArray();
        }

        if (currentTarget == null)
        {
            currentTarget = detectionRangeEntities.OrderBy(entity => Vector3.SqrMagnitude(entity.transform.position - enemy.transform.position)).FirstOrDefault();

            return currentTarget != null;
        }
        else
        {
            return detectionRangeEntities.Contains(currentTarget);
        }
    }

    // TODO: 시야 장애물 정보
    public bool isTargetInAggroRange(bool exclusive)
    {
        if (currentTarget != null)
        {
            Collider2D[] aggroRangeColliders = new Collider2D[0];

            if (aggroRange.overlapBox)
            {
                aggroRangeColliders = Physics2D.OverlapBoxAll(aggroRange.centerTransform.position, aggroRange.boxSize, aggroRange.boxRotation, whatIsChaseTarget);
            }
            else if (aggroRange.overlapCircle)
            {
                aggroRangeColliders = Physics2D.OverlapCircleAll(aggroRange.centerTransform.position, aggroRange.circleRadius, whatIsChaseTarget);
            }

            Entity[] aggroRangeEntities = aggroRangeColliders.Select(collider => collider.GetComponent<Entity>()).ToArray();

            if (aggroRange.limitAngle)
            {
                int multiplier = enemy.transform.rotation.y == 0 ? 1 : -1;
                Vector2 baseVector = enemy.transform.right.Rotate(multiplier * aggroRange.centerRotation * Mathf.Deg2Rad);
                Debug.Log("BaseVector: " + baseVector);
                aggroRangeEntities = aggroRangeEntities.Where(collider => enemy.combat.CheckWithinAngle(baseVector, collider.transform.position - enemy.transform.position, aggroRange.counterClockwiseAngle, aggroRange.clockwiseAngle)).ToArray();
            }

            if (exclusive)
            {
                bool result = !isTargetInDetectionRange() && aggroRangeEntities.Contains(currentTarget);

                if (!result)
                {
                    currentTargetLastVelocity = currentTarget.rigidBody.velocity;
                    currentTargetLastPosition = currentTarget.transform.position;
                    currentTarget = null;
                }
                return result;
            }
            else
            {
                bool result = aggroRangeEntities.Contains(currentTarget);

                if (!result)
                {
                    currentTargetLastVelocity = currentTarget.rigidBody.velocity;
                    currentTargetLastPosition = currentTarget.transform.position;
                    currentTarget = null;
                }
                return result;
            }
        }
        else
        {
            return false;
        }
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

        

        Gizmos.color = Color.yellow;

        if (detectionRange.overlapBox)
        {
            float angle = Mathf.Atan2(detectionRange.boxSize.y, detectionRange.boxSize.x) * Mathf.Rad2Deg;

            #region Draw Box
            Vector3 topRightPosition = detectionRange.centerTransform.position + detectionRange.boxSize.magnitude * new Vector3(Mathf.Cos((angle + detectionRange.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((angle + detectionRange.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
            Vector3 topLeftPosition = detectionRange.centerTransform.position + detectionRange.boxSize.magnitude * new Vector3(Mathf.Cos((180.0f - angle + detectionRange.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((180.0f - angle + detectionRange.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
            Vector3 bottomLeftPosition = detectionRange.centerTransform.position + detectionRange.boxSize.magnitude * new Vector3(Mathf.Cos((angle - 180.0f + detectionRange.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((angle - 180.0f + detectionRange.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
            Vector3 bottomRightPosition = detectionRange.centerTransform.position + detectionRange.boxSize.magnitude * new Vector3(Mathf.Cos((-angle + detectionRange.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((-angle + detectionRange.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
            Vector3 topMidPosition = (topRightPosition + topLeftPosition) / 2.0f;
            Vector3 bottomMidPosition = (bottomRightPosition + bottomLeftPosition) / 2.0f;
            Vector3 leftMidPosition = (topLeftPosition + bottomLeftPosition) / 2.0f;
            Vector3 rightMidPosition = (topRightPosition + bottomRightPosition) / 2.0f;

            Gizmos.DrawLine(topLeftPosition, topRightPosition);
            Gizmos.DrawLine(topRightPosition, bottomRightPosition);
            Gizmos.DrawLine(bottomRightPosition, bottomLeftPosition);
            Gizmos.DrawLine(bottomLeftPosition, topLeftPosition);
            Gizmos.DrawLine(topMidPosition, bottomMidPosition);
            Gizmos.DrawLine(leftMidPosition, rightMidPosition);
            #endregion

            if (detectionRange.limitAngle)
            {
                #region Draw Center Line
                if (detectionRange.centerRotation > 0.0f)
                {
                    if (detectionRange.centerRotation < angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(detectionRange.centerRotation + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(detectionRange.centerRotation * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (detectionRange.centerRotation > 180.0f - angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(detectionRange.centerRotation + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(detectionRange.centerRotation * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(detectionRange.centerRotation + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.y / Mathf.Cos((90.0f - detectionRange.centerRotation) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                else
                {
                    if (detectionRange.centerRotation > -angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(detectionRange.centerRotation + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(detectionRange.centerRotation * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (detectionRange.centerRotation < angle - 180.0f)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(detectionRange.centerRotation + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(detectionRange.centerRotation * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(detectionRange.centerRotation + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.y / Mathf.Cos((90.0f + detectionRange.centerRotation) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                #endregion

                #region Draw Clockwise Line
                float clockwiseAngle = detectionRange.centerRotation - detectionRange.clockwiseAngle;
                if (clockwiseAngle > 0.0f)
                {
                    if (clockwiseAngle < angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle > 180.0f - angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.y / Mathf.Cos((90.0f - clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                else
                {
                    if (clockwiseAngle > -angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle > angle - 180.0F)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.y / Mathf.Cos((90.0f + clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle >= -180.0f)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle > -180.0f - angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle > angle - 360.0f)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.y / Mathf.Cos((90.0f + clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle >= -360.0f)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                #endregion

                #region Draw Counterclockwise Line
                float counterClockwiseAngle = detectionRange.centerRotation + detectionRange.counterClockwiseAngle;
                if (counterClockwiseAngle > 0.0f)
                {
                    if (counterClockwiseAngle < angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle < 180.0f - angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.y / Mathf.Cos((90.0f - counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle <= 180.0f)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle < 180.0f + angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle < 360.0f - angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.y / Mathf.Cos((90.0f - counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle <= 360.0f)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                else
                {
                    if (counterClockwiseAngle > -angle)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle < angle - 180.0f)
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + detectionRange.boxRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.boxSize.y / Mathf.Cos((90.0f + counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                #endregion
            }
        }
        else if (detectionRange.overlapCircle)
        {
            Gizmos.DrawWireSphere(detectionRange.centerTransform.position, detectionRange.circleRadius);

            if (detectionRange.limitAngle)
            {
                Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(detectionRange.centerRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.circleRadius);

                Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + detectionRange.centerTransform.right * detectionRange.circleRadius);
                Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(-detectionRange.clockwiseAngle + detectionRange.centerRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.circleRadius);
                Gizmos.DrawLine(detectionRange.centerTransform.position, detectionRange.centerTransform.position + Quaternion.AngleAxis(detectionRange.counterClockwiseAngle + detectionRange.centerRotation, detectionRange.centerTransform.forward) * detectionRange.centerTransform.right * detectionRange.circleRadius);
            }
        }

        if (aggroRange.overlapBox)
        {
            float angle = Mathf.Atan2(aggroRange.boxSize.y, aggroRange.boxSize.x) * Mathf.Rad2Deg;

            #region Draw Box
            Vector3 topRightPosition = aggroRange.centerTransform.position + aggroRange.boxSize.magnitude * new Vector3(Mathf.Cos((angle + aggroRange.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((angle + aggroRange.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
            Vector3 topLeftPosition = aggroRange.centerTransform.position + aggroRange.boxSize.magnitude * new Vector3(Mathf.Cos((180.0f - angle + aggroRange.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((180.0f - angle + aggroRange.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
            Vector3 bottomLeftPosition = aggroRange.centerTransform.position + aggroRange.boxSize.magnitude * new Vector3(Mathf.Cos((angle - 180.0f + aggroRange.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((angle - 180.0f + aggroRange.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
            Vector3 bottomRightPosition = aggroRange.centerTransform.position + aggroRange.boxSize.magnitude * new Vector3(Mathf.Cos((-angle + aggroRange.boxRotation) * Mathf.Deg2Rad), Mathf.Sin((-angle + aggroRange.boxRotation) * Mathf.Deg2Rad)) / 2.0f;
            Vector3 topMidPosition = (topRightPosition + topLeftPosition) / 2.0f;
            Vector3 bottomMidPosition = (bottomRightPosition + bottomLeftPosition) / 2.0f;
            Vector3 leftMidPosition = (topLeftPosition + bottomLeftPosition) / 2.0f;
            Vector3 rightMidPosition = (topRightPosition + bottomRightPosition) / 2.0f;

            Gizmos.DrawLine(topLeftPosition, topRightPosition);
            Gizmos.DrawLine(topRightPosition, bottomRightPosition);
            Gizmos.DrawLine(bottomRightPosition, bottomLeftPosition);
            Gizmos.DrawLine(bottomLeftPosition, topLeftPosition);
            Gizmos.DrawLine(topMidPosition, bottomMidPosition);
            Gizmos.DrawLine(leftMidPosition, rightMidPosition);
            #endregion

            if (aggroRange.limitAngle)
            {
                #region Draw Center Line
                if (aggroRange.centerRotation > 0.0f)
                {
                    if (aggroRange.centerRotation < angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(aggroRange.centerRotation + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(aggroRange.centerRotation * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (aggroRange.centerRotation > 180.0f - angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(aggroRange.centerRotation + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(aggroRange.centerRotation * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(aggroRange.centerRotation + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.y / Mathf.Cos((90.0f - aggroRange.centerRotation) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                else
                {
                    if (aggroRange.centerRotation > -angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(aggroRange.centerRotation + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(aggroRange.centerRotation * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (aggroRange.centerRotation < angle - 180.0f)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(aggroRange.centerRotation + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(aggroRange.centerRotation * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(aggroRange.centerRotation + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.y / Mathf.Cos((90.0f + aggroRange.centerRotation) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                #endregion

                #region Draw Clockwise Line
                float clockwiseAngle = aggroRange.centerRotation - aggroRange.clockwiseAngle;
                if (clockwiseAngle > 0.0f)
                {
                    if (clockwiseAngle < angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle > 180.0f - angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.y / Mathf.Cos((90.0f - clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                else
                {
                    if (clockwiseAngle > -angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle > angle - 180.0F)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.y / Mathf.Cos((90.0f + clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle >= -180.0f)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle > -180.0f - angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle > angle - 360.0f)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.y / Mathf.Cos((90.0f + clockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (clockwiseAngle >= -360.0f)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(clockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(clockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                #endregion

                #region Draw Counterclockwise Line
                float counterClockwiseAngle = aggroRange.centerRotation + aggroRange.counterClockwiseAngle;
                if (counterClockwiseAngle > 0.0f)
                {
                    if (counterClockwiseAngle < angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle < 180.0f - angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.y / Mathf.Cos((90.0f - counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle <= 180.0f)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle < 180.0f + angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle < 360.0f - angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.y / Mathf.Cos((90.0f - counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle <= 360.0f)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                else
                {
                    if (counterClockwiseAngle > -angle)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else if (counterClockwiseAngle < angle - 180.0f)
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position - Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.x / Mathf.Cos(counterClockwiseAngle * Mathf.Deg2Rad) / 2.0f);
                    }
                    else
                    {
                        Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(counterClockwiseAngle + aggroRange.boxRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.boxSize.y / Mathf.Cos((90.0f + counterClockwiseAngle) * Mathf.Deg2Rad) / 2.0f);
                    }
                }
                #endregion
            }
        }
        else if (aggroRange.overlapCircle)
        {
            Gizmos.DrawWireSphere(aggroRange.centerTransform.position, aggroRange.circleRadius);

            if (aggroRange.limitAngle)
            {
                Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(aggroRange.centerRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.circleRadius);

                Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + aggroRange.centerTransform.right * aggroRange.circleRadius);
                Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(-aggroRange.clockwiseAngle + aggroRange.centerRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.circleRadius);
                Gizmos.DrawLine(aggroRange.centerTransform.position, aggroRange.centerTransform.position + Quaternion.AngleAxis(aggroRange.counterClockwiseAngle + aggroRange.centerRotation, aggroRange.centerTransform.forward) * aggroRange.centerTransform.right * aggroRange.circleRadius);
            }
        }

        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}
