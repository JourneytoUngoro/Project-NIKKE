using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Neon : Enemy
{
    public NeonCombat neonCombat { get; protected set; }
    public NeonMeleeAttackState neonMeleeAttackState { get; protected set; }
    public NeonRangedAttackState neonRangedAttackState { get; protected set; }
    public NeonChargeAttackState neonChargeAttackState { get; protected set; }
    public NeonData neonData { get; protected set; }

    protected override void Start()
    {
        base.Start();
        
        neonCombat = entityCombat as NeonCombat;
        neonData = enemyData as NeonData;
        
        moveState = new NeonMoveState(this, "move");
        targetInAggroRangeState = new NeonTargetInAggroRangeState(this, "move");
        neonMeleeAttackState = new NeonMeleeAttackState(this, "meleeAttack", neonData.meleeAttackCoolDown);
        neonRangedAttackState = new NeonRangedAttackState(this, "rangedAttack", neonData.rangedAttackCoolDown);
        neonChargeAttackState = new NeonChargeAttackState(this, "chargeAttack", neonData.chargeAttackCoolDown);

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
