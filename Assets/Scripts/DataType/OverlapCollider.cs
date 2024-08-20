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
}
