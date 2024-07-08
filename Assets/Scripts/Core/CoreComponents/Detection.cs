using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Detection : CoreComponent
{
    #region Check Transform
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Transform ledgeCheckTransform;
    [SerializeField] private Transform wallCheckTransformTop;
    [SerializeField] private Transform wallCheckTransformBottom;
    #endregion

    #region Check Variables
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float ledgeCheckDistance;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] protected float wallCheckDistance;

    private float slopeDownAngle;
    public Vector2 slopePerpNormal { get; private set; }
    // 해당 Vector2는 시계 반대 방향으로 언덕의 각을 표시한다. 즉, 항상 왼쪽을 바라보고 있다는 말이다.
    private bool onSlope;
    #endregion

    #region Other Variables
    protected Vector2 workSpace;
    #endregion

    public bool isGrounded()
    {
        if (groundCheckRadius != 0.0f) return Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, whatIsGround);
        else return Physics2D.OverlapBox(groundCheckTransform.position, groundCheckSize, 0.0f, whatIsGround);
    }

    public bool isDetectingLedge()
    {
        return !Physics2D.Raycast(ledgeCheckTransform.position, -transform.up, ledgeCheckDistance, whatIsGround);
    }

    public bool isDetectingLedge(Vector2 position)
    {
        return !Physics2D.Raycast(position, -transform.up, ledgeCheckDistance, whatIsGround);
    }

    public bool isOnSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckTransform.position, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {
            slopePerpNormal = Vector2.Perpendicular(hit.normal).normalized * -1;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (Mathf.Abs(slopeDownAngle) < 0.001f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }

        // SlopeCheck(groundCheckTransform.position);
        // return onSlope;
    }

    public bool isTouchingWall()
    {
        return Physics2D.Raycast(wallCheckTransformTop.position, transform.right, wallCheckDistance, whatIsGround) && Physics2D.Raycast(wallCheckTransformBottom.position, transform.right, wallCheckDistance, whatIsGround);
        // return Physics2D.OverlapBox(wallCheckTransformTop.position, Vector2.one, 0.0f, whatIsGround);
    }

    private void SlopeCheck(Vector2 checkPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {
            slopePerpNormal = Vector2.Perpendicular(hit.normal).normalized * -1;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            /*if (slopeDownAngle != slopeDownAngleOld)
            {
                onSlope = true;
            }*/
            if (Mathf.Abs(slopeDownAngle) < 0.001f)
            {
                onSlope = false;
            }
            else
            {
                onSlope = true;
            }

            // slopeDownAngleOld = slopeDownAngle;
        }
        else
        {
            onSlope = false;
        }
    }

    public void ChangeGroundCheckSize(Vector2 groundCheckSize)
    {
       this.groundCheckSize = groundCheckSize;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (groundCheckRadius != 0.0f)
        {
            Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
        }
        else
        {
            Gizmos.DrawWireCube(groundCheckTransform.position, groundCheckSize);
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

        if (ledgeCheckTransform != null)
        {
            Gizmos.DrawLine(ledgeCheckTransform.position, ledgeCheckTransform.position - transform.up * ledgeCheckDistance);
        }
    }
}
