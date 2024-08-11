using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Neon : Enemy
{
    public enum ProjectileType { midRAttack }

    protected override void Start()
    {
        base.Start();
        
        moveState = new NeonMoveState(this, "move");
        meleeAttackState = new NeonMeleeAttackState(this, "meleeAttack", enemyData.meleeAttackCoolDown);
        midRAttackState = new NeonMidRAttackState(this, "midRAttack", enemyData.midRAttackCoolDown);
        rangedAttackState = new NeonRangedAttackState(this, "rangedAttack", enemyData.rangedAttackCoolDown);
        targetInAggroRangeState = new NeonTargetInAggroRangeState(this, "move");

        enemyStateMachine.Initialize(moveState);
    }
}
