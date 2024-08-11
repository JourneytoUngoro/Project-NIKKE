using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonMeleeAttackState : EnemyAttackState
{
    private Neon neon;
    private NeonData neonData;

    public NeonMeleeAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        neon = enemy as Neon;
        neonData = enemyData as NeonData;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.movement.SetVelocityX(0.0f);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        neon.combat.DoMeleeAttack();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(true);

            if (isGrounded)
            {
                neon.movement.SetVelocityX(0.0f);

                if (isOnSlope)
                {
                    neon.movement.SetVelocityY(0.0f);
                }
            }
        }
    }
}
