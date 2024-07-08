using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region State Variables
    public EnemyStateMachine enemyStateMachine { get; private set; }

    public EnemyIdleState idleState { get; private set; }
    public EnemyPlayerInDetectionRangeState playerInDetectionRangeState { get; private set; }
    public EnemyPlayerInAggroRangeState playerInAggroRangeState { get; private set; }
    public EnemyLookForPlayerState lookForPlayerState { get; private set; }
    public EnemyMeleeAttackState meleeAttackState { get; private set; }
    public EnemyRangedAttackState rangedAttackState { get; private set; }
    public EnemyTeleportState teleportState { get; private set; }
    [field: SerializeField] public EnemyData enemyData { get; private set; }
    #endregion

    #region Enemy Components
    public Animator animator { get; private set; }
    public Rigidbody2D rigidBody { get; private set; }
    public Core enemyCore { get; private set; }
    public Movement movement { get; private set; }
    public EnemyDetection detection { get; private set; }
    public EnemyCombat combat { get; private set; }
    
    [field: SerializeField] public Seeker seeker { get; private set; }
    #endregion

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        enemyCore = GetComponentInChildren<Core>();
    }

    protected virtual void Start()
    {
        movement = enemyCore.GetCoreComponent<Movement>();
        detection = enemyCore.GetCoreComponent<EnemyDetection>();
        combat = enemyCore.GetCoreComponent<EnemyCombat>();

        enemyStateMachine = new EnemyStateMachine();

        idleState = new EnemyIdleState(this, "idle");
        playerInDetectionRangeState = new EnemyPlayerInDetectionRangeState(this, "playerDetected");
        playerInAggroRangeState = new EnemyPlayerInAggroRangeState(this, "inAgroRange");
        lookForPlayerState = new EnemyLookForPlayerState(this, "lookForPlayer");
        meleeAttackState = new EnemyMeleeAttackState(this, "meleeAttack");
        rangedAttackState = new EnemyRangedAttackState(this, "rangedAttack");
        teleportState = new EnemyTeleportState(this, "teleport");

        enemyStateMachine.Initialize(idleState);
    }

    protected virtual void Update()
    {
        enemyStateMachine.currentState.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        enemyStateMachine.currentState.PhysicsUpdate();
    }
}
