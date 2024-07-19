using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    #region State Variables
    public EnemyStateMachine enemyStateMachine { get; private set; }

    public EnemyIdleState idleState { get; private set; }
    public EnemyMoveState moveState { get; private set; }
    public EnemySleepState sleepState { get; private set; }
    public EnemyPlayerInDetectionRangeState playerInDetectionRangeState { get; private set; }
    public EnemyPlayerInAggroRangeState playerInAggroRangeState { get; private set; }
    public EnemyLookForPlayerState lookForPlayerState { get; private set; }
    public EnemyMeleeAttackState meleeAttackState { get; private set; }
    public EnemyRangedAttackState rangedAttackState { get; private set; }
    public EnemyTeleportState teleportState { get; private set; }
    [field: SerializeField] public EnemyData enemyData { get; private set; }
    #endregion

    #region Enemy Components
    public EnemyMovement movement { get; protected set; }
    public EnemyDetection detection { get; protected set; }
    public EnemyCombat combat { get; protected set; }
    public Stats stats { get; protected set; }
    [field: SerializeField] public Seeker seeker { get; private set; }
    #endregion

    #region Other Variables
    protected bool isDead;
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void Start()
    {
        movement = core.GetCoreComponent<EnemyMovement>();
        detection = core.GetCoreComponent<EnemyDetection>();
        combat = core.GetCoreComponent<EnemyCombat>();

        enemyStateMachine = new EnemyStateMachine();

        idleState = new EnemyIdleState(this, "idle");
        sleepState = new EnemySleepState(this, "sleep");
        moveState = new EnemyMoveState(this, "move");
        playerInDetectionRangeState = new EnemyPlayerInDetectionRangeState(this, "alert");
        playerInAggroRangeState = new EnemyPlayerInAggroRangeState(this, "move");
        lookForPlayerState = new EnemyLookForPlayerState(this, "idle");
        meleeAttackState = new EnemyMeleeAttackState(this, "meleeAttack");
        rangedAttackState = new EnemyRangedAttackState(this, "rangedAttack");
        teleportState = new EnemyTeleportState(this, "teleport");

        if (enemyData.canSleep)
        {
            enemyStateMachine.Initialize(sleepState);
        }
        else
        {
            enemyStateMachine.Initialize(idleState);
        }
        // enemyStateMachine.Initialize(idleState);
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
