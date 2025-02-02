using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class AnisCloseRangedAttackState : EnemyAttackState
{
    private Anis anis;
    private LineRenderer trajectoryLine;
    private ProjectileComponent projectileComponent;
    private Projectile projectile;

    public AnisCloseRangedAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        anis = enemy as Anis;
        trajectoryLine = anis.GetComponentInChildren<LineRenderer>(true);
        projectileComponent = anis.anisCombat.rangedAttack[0].combatAbilityData.combatAbilityComponents.GetCombatComponent<ProjectileComponent>();
        projectile = projectileComponent.projectilePrefab.GetComponent<Projectile>();
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        anis.combat.DoAttack(anis.anisCombat.closeRangedAttack[0]);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(true);

            /*if (isGrounded)
            {
                anis.movement.SetVelocityX(0.0f);

                if (isOnSlope)
                {
                    anis.movement.SetVelocityY(0.0f);
                }
            }*/
        }
    }

    public bool CanAttack()
    {
        if (!canAttack) return false;

        Vector2? direction = projectile.CalculateProjectileVelocity(anis.anisCombat.closeRangedAttack[0].overlapColliders[1].centerTransform.position, anis.detection.currentTarget.transform.position, true);

        if (direction.HasValue)
        {
            return Mathf.Abs(Vector2.SignedAngle(anis.transform.right, direction.Value)) <= 15.0f;
        }
        else
        {
            return false;
        }
    }
}
