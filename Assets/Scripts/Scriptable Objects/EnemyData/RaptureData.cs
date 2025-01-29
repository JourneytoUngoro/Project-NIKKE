using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRaptureData", menuName = "Data/Enemy Data")]
public class RaptureData : EnemyData
{
    public bool canFly;
    public bool canTeleport;

    [Header("Move State")]
    public float moveSpeed = 6.0f;
}
