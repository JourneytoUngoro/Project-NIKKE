using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class AnisRangedAttackState : EnemyAttackState
{
    private Anis anis;
    private Timer rangedAttackHaltTimer;
    private Timer afterShockTimer;
    private LineRenderer trajectoryLine;
    private ProjectileComponent projectileComponent;
    private Projectile projectile;
    private EnemyState transitState;

    private int? rangedAttackType;
    private bool initialFlag;
    private bool attackConfirmed;
    private bool isTargetInMeleeAttack0Range;
    private bool isTargetInMeleeAttack1Range;
    private bool isTargetInMeleeAttack2Range;
    private bool isTargetInMeleeAttack3Range;
    private bool isTargetInRangedAttackRange;

    public AnisRangedAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        anis = enemy as Anis;
        trajectoryLine = anis.GetComponentInChildren<LineRenderer>(true);
        projectileComponent = anis.anisCombat.rangedAttack[0].combatAbilityData.combatAbilityComponents.GetCombatComponent<ProjectileComponent>();
        projectile = projectileComponent.projectilePrefab.GetComponent<Projectile>();

        rangedAttackHaltTimer = new Timer(anis.anisData.rangedAttackHaltTime);
        rangedAttackHaltTimer.timerAction += () =>
        {
            attackConfirmed = true;
            anis.animator.SetTrigger("rangedAttackFire");
        };

        afterShockTimer = new Timer(anis.anisData.rangedAttackAfterShockTime);
        afterShockTimer.timerAction += () =>
        {
            if (CheckShouldTransit())
            {
                anis.animator.SetBool("rangedAttackOut", true);
            }
            else
            {
                initialFlag = true;
                anis.animator.SetInteger("rangedAttackType", rangedAttackType.Value);
            }
        };
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        if (initialFlag)
        {
            anis.anisCombat.damagedTargets.Clear();
            attackConfirmed = false;
            rangedAttackHaltTimer.StartSingleUseTimer();
            initialFlag = false;
        }
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        anis.combat.DoAttack(anis.anisCombat.rangedAttack[index]);
        afterShockTimer.StartSingleUseTimer();
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        stateMachine.ChangeState(transitState);
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInMeleeAttack0Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack0[0]) || anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack0[1]);
        isTargetInMeleeAttack1Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack1[0]);
        isTargetInMeleeAttack2Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack2[0]);
        isTargetInMeleeAttack3Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack3[0]) || anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack3[1]);
        isTargetInRangedAttackRange = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.rangedAttack[0]);
    }

    public override void Enter()
    {
        base.Enter();

        initialFlag = true;
        transitState = anis.targetInAggroRangeState;
    }

    public override void Exit()
    {
        base.Exit();

        anis.animator.ResetTrigger("rangedAttackFire");
        anis.animator.SetBool("rangedAttackOut", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isTargetInRangedAttackRange)
            {
                if (isTargetInMeleeAttack0Range)
                {
                    anis.animator.SetBool("rangedAttackOut", true);
                    transitState = anis.anisMeleeAttackState;
                }
                else
                {
                    if (!attackConfirmed)
                    {
                        if (CheckShouldTransit())
                        {
                            anis.animator.SetBool("rangedAttackOut", true);
                            transitState = anis.targetInAggroRangeState;
                        }
                        else
                        {
                            anis.animator.SetInteger("rangedAttackType", rangedAttackType.Value);
                        }
                    }
                }
            }
            else
            {
                anis.animator.SetBool("rangedAttackOut", true);

                if (anis.detection.currentTargetLastVelocity.x * anis.movement.facingDirection < 0)
                {
                    transitState = anis.lookForTargetState;
                }
                else
                {
                    transitState = anis.targetInAggroRangeState;
                }
            }

            rangedAttackHaltTimer.Tick();
            afterShockTimer.Tick();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(true);
        }
    }

    private bool CheckShouldTransit()
    {
        if (anis.detection.currentTarget.isDead)
        {
            return true;
        }
        else
        {
            Vector2? direction = projectile.CalculateProjectileVelocity(anis.transform.position, anis.detection.currentTarget.transform.position, true);

            if (direction.HasValue)
            {
                rangedAttackType = RangedAttackType(Vector2.Angle(anis.transform.right, direction.Value));

                if (rangedAttackType.Value != -1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                rangedAttackType = null;
                return true;
            }
        }
    }

    private int RangedAttackType(float angle)
    {
        if (-75.0f <= angle && angle < -45.0f)
        {
            return 0;
        }
        else if (-45.0f <= angle && angle < -15.0f)
        {
            return 1;
        }
        else if (-15.0f <= angle && angle <= 15.0f)
        {
            return 2;
        }
        else if (15.0f < angle && angle <= 45.0f)
        {
            return 3;
        }
        else if (45.0f < angle  && angle < 75.0f)
        {
            return 4;
        }
        else
        {
            return -1;
        }
    }

    public bool CanAttack()
    {
        if (!canAttack) return false;
        Vector2? direction = projectile.CalculateProjectileVelocity(anis.transform.position, anis.detection.currentTarget.transform.position, true);
        rangedAttackType = direction.HasValue ? RangedAttackType(Vector2.SignedAngle(anis.transform.right, direction.Value)) : null;
        return rangedAttackType.HasValue ? rangedAttackType != -1 : false;
    }
}
