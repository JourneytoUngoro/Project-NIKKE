using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Detection : CoreComponent
{
    [SerializeField] protected LayerMask whatIsGround;

    #region Check Transform
    [SerializeField] protected Transform groundCheckTransform;
    [SerializeField] protected Transform ledgeCheckTransform;
    [SerializeField] protected Transform wallCheckTransformTop;
    [SerializeField] protected Transform wallCheckTransformBottom;
    #endregion

    #region Check Variables
    [SerializeField] protected OverlapCollider groundCheck;
    [SerializeField] protected float ledgeCheckDistance;
    [SerializeField] protected float slopeCheckDistance;
    [SerializeField] protected float wallCheckDistance;

    public Vector2 slopePerpNormal { get; protected set; }
    // above Vector2 always represents the slope's angle to where player is looking at

    protected float slopeDownAngle;
    // 해당 Vector2는 시계 반대 방향으로 언덕의 각을 표시한다. 즉, 항상 왼쪽을 바라보고 있다는 말이다.
    #endregion

    public bool isGrounded()
    {
        if (groundCheck.overlapCircle)
        {
            return Physics2D.OverlapCircle(groundCheckTransform.position, groundCheck.circleRadius, whatIsGround);
        }
        else if (groundCheck.overlapBox)
        {
            return Physics2D.OverlapBox(groundCheckTransform.position, groundCheck.boxSize, 0.0f, whatIsGround);
        }
        else return false;
    }

    public bool isOnSlope()
    {
        Vector2 slopePerpNormalFront = Vector2.right, slopePerpNormalMid = Vector2.right;
        bool frontOnSlope = false, midOnSlope = false;
        RaycastHit2D rayHitMid = Physics2D.Raycast(groundCheckTransform.position, -transform.up, slopeCheckDistance, whatIsGround);
        RaycastHit2D rayHitFront;

        if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection >= 0)
        {
            rayHitFront = groundCheck.overlapBox ? Physics2D.Raycast(groundCheckTransform.position + transform.right * groundCheck.boxSize.x / 2.0f, -transform.up, slopeCheckDistance, whatIsGround) : Physics2D.Raycast(groundCheckTransform.position + transform.right * groundCheck.circleRadius, -transform.up, slopeCheckDistance, whatIsGround);
        }
        else
        {
            rayHitFront = groundCheck.overlapBox ? Physics2D.Raycast(groundCheckTransform.position - transform.right * groundCheck.boxSize.x / 2.0f, -transform.up, slopeCheckDistance, whatIsGround) : Physics2D.Raycast(groundCheckTransform.position - transform.right * groundCheck.circleRadius, -transform.up, slopeCheckDistance, whatIsGround);
        }

        if (rayHitFront)
        {
            slopePerpNormalFront = Vector2.Perpendicular(rayHitFront.normal).normalized * -1;

            slopeDownAngle = Vector2.Angle(rayHitFront.normal, Vector2.up);

            if (Mathf.Abs(slopeDownAngle) > epsilon)
            {
                frontOnSlope = true;
            }
        }

        if (rayHitMid)
        {
            slopePerpNormalMid = Vector2.Perpendicular(rayHitMid.normal).normalized * -1;
            
            slopeDownAngle = Vector2.Angle(rayHitMid.normal, Vector2.up);

            if (Mathf.Abs(slopeDownAngle) > epsilon)
            {
                midOnSlope = true;
            }
        }

        if (frontOnSlope)
        {
            slopePerpNormal = slopePerpNormalFront;
        }
        else if (midOnSlope)
        {
            slopePerpNormal = slopePerpNormalMid;
        }
        else
        {
            slopePerpNormal = Vector2.right;
        }

        return frontOnSlope || midOnSlope;
    }

    public bool isOnSlopeBack()
    {
        RaycastHit2D rayHitBack;

        if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection >= 0)
        {
            rayHitBack = groundCheck.overlapBox ? Physics2D.Raycast(groundCheckTransform.position - transform.right * groundCheck.boxSize.x / 2.0f, -transform.up, slopeCheckDistance, whatIsGround) : Physics2D.Raycast(groundCheckTransform.position - transform.right * groundCheck.circleRadius, -transform.up, slopeCheckDistance, whatIsGround);
        }
        else
        {
            rayHitBack = groundCheck.overlapBox ? Physics2D.Raycast(groundCheckTransform.position + transform.right * groundCheck.boxSize.x / 2.0f, -transform.up, slopeCheckDistance, whatIsGround) : Physics2D.Raycast(groundCheckTransform.position + transform.right * groundCheck.circleRadius, -transform.up, slopeCheckDistance, whatIsGround);
        }

        if (rayHitBack)
        {
            if (Vector2.Angle(rayHitBack.normal, Vector2.up) > epsilon)
            {
                return true;
            }
        }

        return false;
    }

    public bool isDetectingLedge()
    {
        return !Physics2D.Raycast(ledgeCheckTransform.position, -transform.up, ledgeCheckDistance, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (groundCheck.overlapCircle)
        {
            Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheck.circleRadius);
        }
        else if (groundCheck.overlapBox)
        {
            Gizmos.DrawWireCube(groundCheckTransform.position, groundCheck.boxSize);
        }

        if (wallCheckTransformTop != null)
        {
            Gizmos.DrawLine(wallCheckTransformTop.position, wallCheckTransformTop.position + transform.right * wallCheckDistance);
        }
        if (wallCheckTransformBottom != null)
        {
            Gizmos.DrawLine(wallCheckTransformBottom.position, wallCheckTransformBottom.position + transform.right * wallCheckDistance);
        }

        Gizmos.DrawLine(groundCheckTransform.position, groundCheckTransform.position - transform.up * slopeCheckDistance);

        if (groundCheck.overlapCircle)
        {
            if (!Application.isPlaying)
            {
                Gizmos.DrawLine(groundCheckTransform.position + transform.right * groundCheck.circleRadius, groundCheckTransform.position + transform.right * groundCheck.circleRadius - transform.up * slopeCheckDistance);
            }
            else {
                if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection >= 0)
                {
                    Gizmos.DrawLine(groundCheckTransform.position + transform.right * groundCheck.circleRadius, groundCheckTransform.position + transform.right * groundCheck.circleRadius - transform.up * slopeCheckDistance);
                }
                else
                {
                    Gizmos.DrawLine(groundCheckTransform.position - transform.right * groundCheck.circleRadius, groundCheckTransform.position - transform.right * groundCheck.circleRadius - transform.up * slopeCheckDistance);
                }
            }
        }
        else if (groundCheck.overlapBox)
        {
            if (!Application.isPlaying)
            {
                Gizmos.DrawLine(groundCheckTransform.position + transform.right * groundCheck.boxSize.x / 2.0f, groundCheckTransform.position + transform.right * groundCheck.boxSize.x / 2.0f - transform.up * slopeCheckDistance);
            }
            else {
                if (entity.rigidBody.velocity.x * entity.entityMovement.facingDirection >= 0)
                {
                    Gizmos.DrawLine(groundCheckTransform.position + transform.right * groundCheck.boxSize.x / 2.0f, groundCheckTransform.position + transform.right * groundCheck.boxSize.x / 2.0f - transform.up * slopeCheckDistance);
                }
                else
                {
                    Gizmos.DrawLine(groundCheckTransform.position - transform.right * groundCheck.boxSize.x / 2.0f, groundCheckTransform.position - transform.right * groundCheck.boxSize.x / 2.0f - transform.up * slopeCheckDistance);
                }
            }
        }

        if (ledgeCheckTransform != null)
        {
            Gizmos.DrawLine(ledgeCheckTransform.position, ledgeCheckTransform.position - transform.up * ledgeCheckDistance);
        }
    }
}
