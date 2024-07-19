using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : Detection
{
    #region Check Transform
    [SerializeField] private Transform ceilingCheckTransform;
    [SerializeField] private Transform wallCheckTransformTopBehind;
    [SerializeField] private Transform wallCheckTransformBottomBehind;
    #endregion

    #region Check Variables
    [SerializeField] private Vector2 ceilingCheckSize;
    [SerializeField] private Vector2 colliderCheckOffset;
    #endregion

    #region Other Variables

    #endregion

    public bool isTouchingWall()
    {
        return Physics2D.Raycast(wallCheckTransformTop.position, transform.right, wallCheckDistance, whatIsGround) && Physics2D.Raycast(wallCheckTransformBottom.position, transform.right, wallCheckDistance, whatIsGround);
        // return Physics2D.OverlapBox(wallCheckTransformTop.position, Vector2.one, 0.0f, whatIsGround);
    }

    public bool isTouchingWallBehind()
    {
        return Physics2D.Raycast(wallCheckTransformTopBehind.position, -transform.right, wallCheckDistance, whatIsGround) && Physics2D.Raycast(wallCheckTransformBottomBehind.position, -transform.right, wallCheckDistance, whatIsGround);
    }

    public bool isTouchingCeiling()
    {
        return Physics2D.OverlapBox(ceilingCheckTransform.position, ceilingCheckSize, 0.0f, whatIsGround);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (ceilingCheckTransform != null)
        {
            Gizmos.DrawWireCube(ceilingCheckTransform.position, ceilingCheckSize);
        }
    }
}
