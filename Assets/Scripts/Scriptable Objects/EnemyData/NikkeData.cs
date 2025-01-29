using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NikkeData : EnemyData
{
    [Header("Move State")]
    public float moveSpeed = 6.0f;
    public float moveTime;
    public Ease moveEaseFunction;
}
