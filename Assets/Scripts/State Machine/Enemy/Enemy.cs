using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    #region State Variables
    public EnemyStateMachine enemyStateMachine { get; private set; }

    public EnemyIdleState idleState { get; protected set; }
    public EnemyMoveState moveState { get; protected set; }
    public EnemySleepState sleepState { get; protected set; }
    public EnemyTargetInDetectionRangeState targetInDetectionRangeState { get; protected set; }
    public EnemyTargetInAggroRangeState targetInAggroRangeState { get; protected set; }
    public EnemyLookForPlayerState lookForTargetState { get; protected set; }
    public EnemyAttackState meleeAttackState { get; protected set; }
    public EnemyAttackState midRAttackState { get; protected set; }
    public EnemyAttackState rangedAttackState { get; protected set; }
    public EnemyTeleportState teleportState { get; protected set; }
    public EnemyStunnedState stunnedState { get; protected set; }
    public EnemyDazedState dazedState { get; protected set; }
    public EnemyKnockbackState knockbackState { get; protected set; }
    [field: SerializeField] public EnemyData enemyData { get; private set; }
    #endregion

    #region Enemy Components
    public EnemyMovement movement { get; protected set; }
    public EnemyDetection detection { get; protected set; }
    public EnemyCombat combat { get; protected set; }
    public EnemyStats stats { get; protected set; }
    [field: SerializeField] public Seeker seeker { get; private set; }
    #endregion

    #region Other Variables
    [field: SerializeField] public Transform[] projectileGeneratePosition;

    protected bool isDead;
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        movement = entityMovement as EnemyMovement;
        detection = entityDetection as EnemyDetection;
        combat = entityCombat as EnemyCombat;
        stats = entityStats as EnemyStats;

        enemyStateMachine = new EnemyStateMachine();

        idleState = new EnemyIdleState(this, "idle");
        sleepState = new EnemySleepState(this, "sleep");
        moveState = new EnemyMoveState(this, "move");
        targetInDetectionRangeState = new EnemyTargetInDetectionRangeState(this, "alert");
        targetInAggroRangeState = new EnemyTargetInAggroRangeState(this, "move");
        lookForTargetState = new EnemyLookForPlayerState(this, "idle");
        teleportState = new EnemyTeleportState(this, "teleport");
        stunnedState = new EnemyStunnedState(this, "stunned");
        dazedState = new EnemyDazedState(this, "idle");
        knockbackState = new EnemyKnockbackState(this, "knockback");

        // State initializing is done in specific enemy script
    }

    protected virtual void Update()
    {
        enemyStateMachine.currentState.LogicUpdate();
    }

    protected virtual void LateUpdate()
    {
        enemyStateMachine.currentState.LateLogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        enemyStateMachine.currentState.PhysicsUpdate();
    }
}
