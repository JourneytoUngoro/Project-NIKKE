using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonRangedAttackState : EnemyAttackState
{
    private Neon neon;

    public NeonRangedAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        neon = enemy as Neon;
    }

    public override void AnimationActionTrigger(int index)
    {
        neon.combat.DoAttack(neon.neonCombat.rangedAttack[0]);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(false);
        }
    }
}
