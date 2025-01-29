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
    #endregion

    #region Other Variables
    
    #endregion

    public bool isTouchingCeiling()
    {
        Collider2D detectedCollider = Physics2D.OverlapBox(ceilingCheckTransform.position, ceilingCheckSize, 0.0f, whatIsGround);
        return detectedCollider != null ? !detectedCollider.CompareTag("OneWayPlatform") : false;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;

        if (ceilingCheckTransform != null)
        {
            Gizmos.DrawWireCube(ceilingCheckTransform.position, ceilingCheckSize);
        }
    }
}
