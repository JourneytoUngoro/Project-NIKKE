using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CheckPositionHorizontal { Back, Mid, Front }
public enum CheckPositionVertical { Top, Bottom, Both }
public enum CheckLogicalOperation { None, AND, OR }

public class Detection : CoreComponent
{
    [field: SerializeField] public LayerMask whatIsGround { get; protected set; }

    #region Check Transform
    [SerializeField] protected OverlapCollider groundCheck;
    [SerializeField] protected Transform wallCheckTransformTop;
    [SerializeField] protected Transform wallCheckTransformBottom;
    [SerializeField] protected Transform ledgeCheckTransformFront;
    [SerializeField] protected Transform ledgeCheckTransformBack;
    #endregion

    #region Check Variables
    [SerializeField] protected float ledgeCheckDistanceFront;
    [SerializeField] protected float ledgeCheckDistanceBack = 0.5f;
    [SerializeField] protected float slopeCheckDistance;
    [SerializeField] protected float wallCheckDistance;
    #endregion

    #region Other Variables
    public Entity currentTarget { get; protected set; }
    public Collider2D currentPlatform { get; private set; }
    public Collider2D lastPlatform { get; private set; }
    public Collider2D detectedPlatform { get; private set; }
    public List<Collider2D> groundedExceptions { get; private set; } = new List<Collider2D>();
    public Vector2 slopePerpNormal { get; protected set; } // Above Vector2 always represents the slope's angle to where player is facing.
    protected float slopeDownAngle; // 해당 Vector2는 시계 반대 방향으로 언덕의 각을 표시한다. 즉, 항상 왼쪽을 바라보고 있다는 말이다.
    #endregion

    public virtual bool isGrounded()
    {
        lastPlatform = currentPlatform != null ? currentPlatform : lastPlatform;

        if (groundCheck.overlapCircle)
        {
            detectedPlatform = Physics2D.OverlapCircle(groundCheck.centerTransform.position, groundCheck.circleRadius, whatIsGround);
            currentPlatform = groundedExceptions.Contains(detectedPlatform) ? null : detectedPlatform;

            if (currentPlatform != null && currentPlatform.CompareTag("OneWayPlatform"))
            {
                if (entity.entityCollider.bounds.min.y > currentPlatform.transform.position.y)
                {
                    Physics2D.IgnoreCollision(currentPlatform, entity.entityCollider, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return currentPlatform;
            }
        }
        else if (groundCheck.overlapBox)
        {
            detectedPlatform = Physics2D.OverlapBox(groundCheck.centerTransform.position, groundCheck.boxSize, 0.0f, whatIsGround);
            currentPlatform = groundedExceptions.Contains(detectedPlatform) ? null : detectedPlatform;

            if (currentPlatform != null && currentPlatform.CompareTag("OneWayPlatform"))
            {
                if (entity.entityCollider.bounds.min.y > currentPlatform.transform.position.y + currentPlatform.bounds.extents.y)
                {
                    Physics2D.IgnoreCollision(currentPlatform, entity.entityCollider, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return currentPlatform;
            }
        }
        else return false;
    }

    /// <summary>
    /// Check the slope in the vertical direction based on the given position. Position is based on the entity's velocity by default, but based on the entity's facing direction if it is not moving.
    /// </summary>
    /// <param name="slopeCheckPosition"></param>
    /// <returns></returns>
    public bool isOnSlopeVertical(CheckPositionHorizontal slopeCheckPosition)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(groundCheck.centerTransform.position, -transform.up, slopeCheckDistance, whatIsGround);
        Vector3 slopePerpNormal = Vector2.Perpendicular(rayHit.normal).normalized * -1;
        float slopeAngle = Vector2.Angle(rayHit.normal, Vector2.up);

        if (Mathf.Abs(slopeAngle) < epsilon)
        {
            slopePerpNormal = transform.right;
        }

        switch (slopeCheckPosition)
        {
            case CheckPositionHorizontal.Front:
                if (groundCheck.overlapBox)
                {
                    if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection < 0)
                    {
                        rayHit = Physics2D.Raycast(groundCheck.centerTransform.position - slopePerpNormal * groundCheck.boxSize.x / 2.0f, -transform.up, slopeCheckDistance, whatIsGround);
                    }
                    else
                    {
                        rayHit = Physics2D.Raycast(groundCheck.centerTransform.position + slopePerpNormal * groundCheck.boxSize.x / 2.0f, -transform.up, slopeCheckDistance, whatIsGround);
                    }
                }
                else if (groundCheck.overlapCircle)
                {
                    if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection < 0)
                    {
                        rayHit = Physics2D.Raycast(groundCheck.centerTransform.position - slopePerpNormal * groundCheck.boxSize.x / 2.0f, -transform.up, slopeCheckDistance, whatIsGround);
                    }
                    else
                    {
                        rayHit = Physics2D.Raycast(groundCheck.centerTransform.position + slopePerpNormal * groundCheck.boxSize.x / 2.0f, -transform.up, slopeCheckDistance, whatIsGround);
                    }
                }
                break;

            case CheckPositionHorizontal.Back:
                if (groundCheck.overlapBox)
                {
                    if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection < 0)
                    {
                        rayHit = Physics2D.Raycast(groundCheck.centerTransform.position + slopePerpNormal * groundCheck.circleRadius, -transform.up, slopeCheckDistance, whatIsGround);
                    }
                    else
                    {
                        rayHit = Physics2D.Raycast(groundCheck.centerTransform.position - slopePerpNormal * groundCheck.circleRadius, -transform.up, slopeCheckDistance, whatIsGround);
                    }
                }
                else if (groundCheck.overlapCircle)
                {
                    if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection < 0)
                    {
                        rayHit = Physics2D.Raycast(groundCheck.centerTransform.position + slopePerpNormal * groundCheck.circleRadius, -transform.up, slopeCheckDistance, whatIsGround);
                    }
                    else
                    {
                        rayHit = Physics2D.Raycast(groundCheck.centerTransform.position - slopePerpNormal * groundCheck.circleRadius, -transform.up, slopeCheckDistance, whatIsGround);
                    }
                }
                break;

            default: break;
        }

        if (rayHit)
        {
            slopeAngle = Vector2.Angle(rayHit.normal, Vector2.up);

            if (Mathf.Abs(slopeAngle) > epsilon)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check the slope based on the entity's current velocity. If the entity's y velocity is above 0, then it will check the slope in horizontal direction toward entity is moving. If else, check the slope in vertical down direction.
    /// </summary>
    /// <returns></returns>
    public bool isOnSlope()
    {
        if (entity.rigidBody.velocity.y > epsilon)
        {
            bool horizontalOnSlope = false;
            Vector2 slopePerpNormalHorizontal = transform.right;
            RaycastHit2D rayHitHorizontal;

            if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection < 0)
            {
                rayHitHorizontal = Physics2D.Raycast(groundCheck.centerTransform.position, -transform.right, slopeCheckDistance, whatIsGround);
            }
            else
            {
                rayHitHorizontal = Physics2D.Raycast(groundCheck.centerTransform.position, transform.right, slopeCheckDistance, whatIsGround);
            }

            if (rayHitHorizontal)
            {
                slopePerpNormalHorizontal = Vector2.Perpendicular(rayHitHorizontal.normal).normalized * -1;

                slopeDownAngle = Vector2.Angle(rayHitHorizontal.normal, Vector2.up) - 90.0f;

                if (Mathf.Abs(slopeDownAngle) > epsilon)
                {
                    horizontalOnSlope = true;
                }
            }

            if (horizontalOnSlope)
            {
                slopePerpNormal = slopePerpNormalHorizontal;
            }
            else
            {
                slopePerpNormal = transform.right;
            }

            return horizontalOnSlope;
        }
        else
        {
            bool verticalOnSlope = false;
            Vector2 slopePerpNormalVertical = transform.up;
            RaycastHit2D rayHitVertical;

            rayHitVertical = Physics2D.Raycast(groundCheck.centerTransform.position, -transform.up, slopeCheckDistance, whatIsGround);

            if (rayHitVertical)
            {
                slopePerpNormalVertical = Vector2.Perpendicular(rayHitVertical.normal).normalized * -1;

                slopeDownAngle = Vector2.Angle(rayHitVertical.normal, Vector2.up);

                if (Mathf.Abs(slopeDownAngle) > epsilon)
                {
                    verticalOnSlope = true;
                }
            }

            if (verticalOnSlope)
            {
                slopePerpNormal = slopePerpNormalVertical;
            }
            // else
            // {
            //     slopePerpNormal = transform.right;
            // }

            return verticalOnSlope;
        }
    }

    public bool isDetectingLedgeFront()
    {
        return !Physics2D.Raycast(ledgeCheckTransformFront.position, -transform.up, ledgeCheckDistanceFront, whatIsGround);
    }

    public bool isDetectingLedgeBack()
    {
        return !Physics2D.Raycast(ledgeCheckTransformBack.position, -transform.up, ledgeCheckDistanceBack, whatIsGround);
    }

    public bool isDetectingWall(CheckPositionHorizontal checkPositionHorizontal, CheckPositionVertical checkPositionVertical, CheckLogicalOperation checkLogicalOperation = CheckLogicalOperation.None)
    {
        if (checkPositionHorizontal == CheckPositionHorizontal.Front)
        {
            if (checkPositionVertical == CheckPositionVertical.Top)
            {
                RaycastHit2D detectedRaycast = Physics2D.Raycast(wallCheckTransformTop.position, transform.right, wallCheckDistance, whatIsGround);
                return detectedRaycast.collider != null ? !detectedRaycast.collider.CompareTag("OneWayPlatform") : false;
            }
            else if (checkPositionVertical == CheckPositionVertical.Bottom)
            {
                RaycastHit2D detectedRaycast = Physics2D.Raycast(wallCheckTransformBottom.position, transform.right, wallCheckDistance, whatIsGround);
                return detectedRaycast.collider != null ? !detectedRaycast.collider.CompareTag("OneWayPlatform") : false;
            }
            else if (checkPositionVertical == CheckPositionVertical.Both)
            {
                if (checkLogicalOperation == CheckLogicalOperation.AND)
                {
                    return isDetectingWall(checkPositionHorizontal, CheckPositionVertical.Top) && isDetectingWall(checkPositionHorizontal, CheckPositionVertical.Bottom);
                }
                else if (checkLogicalOperation == CheckLogicalOperation.OR)
                {
                    return isDetectingWall(checkPositionHorizontal, CheckPositionVertical.Top) || isDetectingWall(checkPositionHorizontal, CheckPositionVertical.Bottom);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else if (checkPositionHorizontal == CheckPositionHorizontal.Back)
        {
            if (checkPositionVertical == CheckPositionVertical.Top)
            {
                RaycastHit2D detectedRaycast = Physics2D.Raycast(wallCheckTransformTop.position - transform.right * wallCheckTransformTop.localPosition.x * 2.0f, -transform.right, wallCheckDistance, whatIsGround);
                return detectedRaycast.collider != null ? !detectedRaycast.collider.CompareTag("OneWayPlatform") : false;
            }
            else if (checkPositionVertical == CheckPositionVertical.Bottom)
            {
                RaycastHit2D detectedRaycast = Physics2D.Raycast(wallCheckTransformBottom.position - transform.right * wallCheckTransformBottom.localPosition.x * 2.0f, -transform.right, wallCheckDistance, whatIsGround);
                return detectedRaycast.collider != null ? !detectedRaycast.collider.CompareTag("OneWayPlatform") : false;
            }
            else if (checkPositionVertical == CheckPositionVertical.Both)
            {
                if (checkLogicalOperation == CheckLogicalOperation.AND)
                {
                    return isDetectingWall(checkPositionHorizontal, CheckPositionVertical.Top) && isDetectingWall(checkPositionHorizontal, CheckPositionVertical.Bottom);
                }
                else if (checkLogicalOperation == CheckLogicalOperation.OR)
                {
                    return isDetectingWall(checkPositionHorizontal, CheckPositionVertical.Top) || isDetectingWall(checkPositionHorizontal, CheckPositionVertical.Bottom);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else if (checkPositionHorizontal == CheckPositionHorizontal.Mid)
        {
            if (Mathf.Abs(entity.rigidBody.velocity.x) > epsilon)
            {
                if (entity.rigidBody.velocity.x > 0)
                {
                    return isDetectingWall(CheckPositionHorizontal.Front, checkPositionVertical, checkLogicalOperation);
                }
                else
                {
                    return isDetectingWall(CheckPositionHorizontal.Back, checkPositionVertical, checkLogicalOperation);
                }
            }
            else
            {
                return isDetectingWall(CheckPositionHorizontal.Front, checkPositionVertical, checkLogicalOperation);
            }
        }
        else
        {
            return false;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (groundCheck.overlapCircle)
        {
            Gizmos.DrawWireSphere(groundCheck.centerTransform.position, groundCheck.circleRadius);
        }
        else if (groundCheck.overlapBox)
        {
            Gizmos.DrawWireCube(groundCheck.centerTransform.position, groundCheck.boxSize);
        }

        if (wallCheckTransformTop != null)
        {
            Gizmos.DrawLine(wallCheckTransformTop.position, wallCheckTransformTop.position + transform.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheckTransformTop.position - transform.right * wallCheckTransformTop.localPosition.x * 2.0f, wallCheckTransformTop.position - transform.right * wallCheckTransformTop.localPosition.x * 2.0f - transform.right * wallCheckDistance);
        }

        if (wallCheckTransformBottom != null)
        {
            Gizmos.DrawLine(wallCheckTransformBottom.position, wallCheckTransformBottom.position + transform.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheckTransformBottom.position - transform.right * wallCheckTransformBottom.localPosition.x * 2.0f, wallCheckTransformBottom.position - transform.right * wallCheckTransformBottom.localPosition.x * 2.0f - transform.right * wallCheckDistance);
        }

        if (Application.isPlaying)
        {
            if (entity.rigidBody.velocity.y > epsilon)
            {
                if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    Gizmos.DrawLine(groundCheck.centerTransform.position, groundCheck.centerTransform.position - transform.right * slopeCheckDistance);
                }
                else
                {
                    Gizmos.DrawLine(groundCheck.centerTransform.position, groundCheck.centerTransform.position + transform.right * slopeCheckDistance);
                }
            }
            else
            {
                Gizmos.DrawLine(groundCheck.centerTransform.position, groundCheck.centerTransform.position - transform.up * slopeCheckDistance);
            }
        }
        else
        {
            Gizmos.DrawLine(groundCheck.centerTransform.position, groundCheck.centerTransform.position - transform.up * slopeCheckDistance);
        }

        if (ledgeCheckTransformFront != null)
        {
            Gizmos.DrawLine(ledgeCheckTransformFront.position, ledgeCheckTransformFront.position - transform.up * ledgeCheckDistanceFront);
        }

        if (ledgeCheckTransformBack != null)
        {
            Gizmos.DrawLine(ledgeCheckTransformBack.position, ledgeCheckTransformBack.position - transform.up * ledgeCheckDistanceBack);
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(groundCheck.centerTransform.position, groundCheck.centerTransform.position + (Vector3)slopePerpNormal * slopeCheckDistance);
    }
}
