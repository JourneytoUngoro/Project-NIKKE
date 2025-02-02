using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyData : ScriptableObject
{
    [Header("Enemy Data")]
    public float baseHealth;
    public float healthIncreasePerLevel;
    public float basePostureHealth;
    public float postureIncreasePerLevel;
    public float postureRecoveryRate;
    public bool canSleep;
    public bool canAlert;
    public bool canFallDownLedge;
    public bool canSmartPathFind;

    [Header("Idle State")]
    public float minWaitTime = 3.0f;
    public float maxWaitTime = 5.0f;

    [Header("Target In Aggro Range State")]
    public float adequateDistance = 5.0f;
    public float haltPossibility;
    public float approachPossibility;
    public float retreatPossibility;
    public float shieldCoonDownTime = 0.5f;
    public float minMovementOptionMaintainTime = 3.0f;
    public float maxMovementOptionMaintainTime = 5.0f;

    [Header("Look for Player State")]
    public int totalTurnAmount = 4;
    public float timeDelayforEachTurn = 1.5f;

    [Header("Dazed State")]
    public float dazedTime = 2.0f;

    [Header("Stunned State")]
    public float stunnedTime = 4.0f;
    public float stunnedKnockbackSpeed = 5.0f;

    [Header("Enemy Pathfinding")]
    public float nextWaypointDistance = 3.0f;
    public float pathUpdatePeriods = 0.5f;
    public float jumpHeightRequirement = 0.8f;
    public float maxMovementDistance = 10.0f;
    public float jumpSpeed = 10.0f;

    [Header("Canvas Settings")]
    public float canvasDisableTime = 5.0f;
}