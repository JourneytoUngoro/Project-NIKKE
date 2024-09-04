using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OverlapCollider
{
    public bool overlapBox;
    public bool overlapCircle;
    
    public Vector2 boxSize;
    public float circleRadius;

    public bool limitAngle;
    [Range(0.0f, 180.0f)] public float clockwiseAngle;
    [Range(0.0f, 180.0f)] public float counterClockwiseAngle;
}
