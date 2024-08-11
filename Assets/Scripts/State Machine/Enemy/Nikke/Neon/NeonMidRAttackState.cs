using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class NeonMidRAttackState : EnemyAttackState
{
    private Neon neon;
    private NeonData neonData;

    public NeonMidRAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        neon = enemy as Neon;
        neonData = enemyData as NeonData;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        neon.combat.FireProjectile("NeonGunFire", neon.projectileGeneratePosition[(int)Neon.ProjectileType.midRAttack].position, null, 0.0f, 0.0f);
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(false);

            if (isAnimationActionTriggered)
            {
                GetRecoil(neonData.midRAttackKRecoilTime, neonData.midRAttackRecoilSpeed);
            }
            else
            {
                if (isGrounded)
                {
                    enemy.movement.SetVelocityX(0.0f);

                    if (isOnSlope)
                    {
                        enemy.movement.SetVelocityY(0.0f);
                    }
                }
            }
        }
    }
}
