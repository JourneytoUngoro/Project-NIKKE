using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject
{
    [Header("Enemy Data")]
    public float maxHealth;
    public float maxPostureHealth;
    public float postureRecoveryRate;
    public bool haveMeleeAttack;

    [Header("Idle State")]
    public float moveSpeed = 6.0f;
    public float turnBackMinTime = 3.0f;
    public float turnBackMaxTime = .0f;

    [Header("Enemy Pathfinding")]
    public float nextWaypointDistance = 3.0f;
    public float pathUpdatePeriods = 0.5f;
    public float jumpHeightRequirement = 0.8f;
    public float maxMovementDistance = 10.0f;
    public float jumpSpeed = 10.0f;

    [Header("Teleport State")]
    public float teleportCoolDown = 10.0f;

    [Header("Look for Player State")]
    public int totalTurnAmount = 4;
    public float timeDelayforEachTurn = 1.5f;

    [Header("Dazed State")]
    public float dazedTime = 2.0f;

    [Header("Stunned State")]
    public float stunnedTime = 4.0f;
}
