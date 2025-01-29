using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class AnisTargetInAggroRangeState : EnemyTargetInAggroRangeState
{
    private Anis anis;

    private bool isTargetInMeleeAttack0Range;
    private bool isTargetInMeleeAttack1Range;
    private bool isTargetInMeleeAttack2Range;
    private bool isTargetInMeleeAttack3Range;
    private bool isTargetInRangedAttackRange;
    private bool isTargetInCloseRangedAttackRange;

    public AnisTargetInAggroRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        anis = enemy as Anis;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInMeleeAttack0Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack0[0]) || anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack0[1]);
        isTargetInMeleeAttack1Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack1[0]);
        isTargetInMeleeAttack2Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack2[0]);
        isTargetInMeleeAttack3Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack3[0]) || anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack3[1]);
        isTargetInRangedAttackRange = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.rangedAttack[0]);
        isTargetInCloseRangedAttackRange = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.closeRangedAttack[0]);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (isTargetInMeleeAttack0Range && anis.anisMeleeAttackState.canAttack)
                {
                    stateMachine.ChangeState(anis.anisMeleeAttackState);
                }
                else if (isTargetInCloseRangedAttackRange && anis.anisCloseRangedAttackState.canAttack)
                {
                    stateMachine.ChangeState(anis.anisCloseRangedAttackState);
                }
                else if (isTargetInRangedAttackRange && anis.anisRangedAttackState.canAttack)
                {
                    stateMachine.ChangeState(anis.anisRangedAttackState);
                }
            }
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController();

            if ((enemy.detection.currentTarget.transform.position.x - enemy.rigidBody.position.x) * facingDirection < 0)
            {
                enemy.movement.Flip();
            }

            anis.movement.SetVelocityX(anis.anisData.moveSpeed, true);

            anis.movement.SetVelocityX(-anis.anisData.moveSpeed, true);
        }
    }
}
