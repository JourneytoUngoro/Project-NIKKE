using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Anis : Enemy
{
    public AnisCombat anisCombat { get; protected set; }
    public AnisMeleeAttackState anisMeleeAttackState { get; protected set; }
    public AnisRangedAttackState anisRangedAttackState { get; protected set; }
    public AnisCloseRangedAttackState anisCloseRangedAttackState { get; protected set; }
    public NeonData anisData { get; protected set; }

    protected override void Start()
    {
        base.Start();

        anisCombat = entityCombat as AnisCombat;
        anisData = enemyData as NeonData;

        moveState = new NeonMoveState(this, "move");
        targetInAggroRangeState = new AnisTargetInAggroRangeState(this, "move");
        anisMeleeAttackState = new AnisMeleeAttackState(this, "meleeAttack", anisData.meleeAttackCoolDown);
        anisRangedAttackState = new AnisRangedAttackState(this, "rangedAttack", anisData.rangedAttackCoolDown);
        anisCloseRangedAttackState = new AnisCloseRangedAttackState(this, "chargeAttack", anisData.chargeAttackCoolDown);

        enemyStateMachine.Initialize(moveState);

        attackStates = new List<EnemyAttackState>();
        IEnumerable<PropertyInfo> attackStateProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.IsSubclassOf(typeof(EnemyAttackState)));

        foreach (PropertyInfo property in attackStateProperties)
        {
            EnemyAttackState attackState = property.GetValue(this) as EnemyAttackState;
            attackStates.Add(attackState);
        }
    }
}
