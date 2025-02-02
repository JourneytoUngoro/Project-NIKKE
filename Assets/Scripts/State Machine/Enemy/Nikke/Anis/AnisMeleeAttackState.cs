using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnisMeleeAttackState : EnemyAttackState
{
    private Anis anis;

    public AnisMeleeAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        anis = enemy as Anis;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        anis.combat.ClearDamagedTargets();
        anis.combat.DoAttack(anis.anisCombat.meleeAttack0[index]);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(true);

            if (isGrounded)
            {
                anis.movement.SetVelocityX(0.0f);

                if (isOnSlope)
                {
                    anis.movement.SetVelocityY(0.0f);
                }
            }
        }
    }
}
