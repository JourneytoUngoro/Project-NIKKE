using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonTargetInAggroRangeState : EnemyTargetInAggroRangeState
{
    private enum MovementOption { Halt, Approach, Retreat };

    private Neon neon;

    private bool isTargetInMeleeAttackRange;
    private bool isTargetInRangedAttackRange;
    private bool isTargetInChargeAttackRange;

    private bool inDistance;
    private bool firstInDistance;
    private MovementOption movementOption;
    private Timer movementOptionTimer;

    public NeonTargetInAggroRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        neon = enemy as Neon;
        movementOptionTimer = new Timer(UtilityFunctions.RandomFloat(neon.neonData.minMovementOptionMaintainTime, neon.neonData.maxMovementOptionMaintainTime));
        movementOptionTimer.timerAction += ChangeMovementOption;
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        if (inDistance)
        {
            if (movementOption == MovementOption.Approach)
            {
                neon.movement.SetVelocityXChangeOverTime(neon.neonData.moveSpeed * neon.movement.facingDirection, neon.neonData.moveTime, neon.neonData.moveEaseFunction, true, isDetectingLedgeFront);
            }
            else if (movementOption == MovementOption.Retreat)
            {
                neon.movement.SetVelocityXChangeOverTime(neon.neonData.moveSpeed * -neon.movement.facingDirection, neon.neonData.moveTime, neon.neonData.moveEaseFunction, true, isDetectingLedgeFront);
            }
        }
        else
        {
            neon.movement.SetVelocityXChangeOverTime(neon.neonData.moveSpeed * neon.movement.facingDirection, neon.neonData.moveTime, neon.neonData.moveEaseFunction, true, isDetectingLedgeFront);
        }
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        if ((enemy.detection.currentTarget.transform.position.x - enemy.rigidBody.position.x) * facingDirection < 0)
        {
            enemy.movement.Flip();
        }

        if ((firstInDistance && Vector2.Distance(enemy.detection.currentTarget.transform.position, enemy.transform.position) < enemy.enemyData.adequateDistance) || inDistance)
        {
            firstInDistance = false;
            neon.animator.SetBool("move", false);
            neon.animator.SetBool("idle", true);
            movementOptionTimer.StartMultiUseTimer();
        }
        else
        {
            movementOptionTimer.StopTimer();
        }
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInMeleeAttackRange = neon.neonCombat.IsTargetInRangeOf(neon.neonCombat.meleeAttack[0]);
        isTargetInRangedAttackRange = neon.neonCombat.IsTargetInRangeOf(neon.neonCombat.rangedAttack[0]);
        isTargetInChargeAttackRange = neon.neonCombat.IsTargetInRangeOf(neon.neonCombat.chargeAttack[0], true);
        Debug.Log("Current Target: " + neon.detection.currentTarget);
        Debug.Log("isTargetInMeleeAttackRange: " + isTargetInMeleeAttackRange);
        Debug.Log("canMeleeAttack: " + neon.neonMeleeAttackState.canAttack);
    }

    public override void Enter()
    {
        base.Enter();

        neon.neonCombat.DoAttack(neon.neonCombat.shieldArea);
        neon.animator.SetBool("shield", true);
        neon.neonCombat.damagedTargets.Clear();
        firstInDistance = true;
        neon.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        neon.combat.ReleaseShieldParryPrefabs(neon.neonCombat.shieldArea);
        neon.animator.ResetTrigger("shielded");
        neon.animator.SetBool("shield", false);
        neon.animator.SetBool("idle", false);
        neon.movement.StopVelocityXChangeOverTime();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (isTargetInMeleeAttackRange && neon.neonMeleeAttackState.canAttack)
                {
                    stateMachine.ChangeState(neon.neonMeleeAttackState);
                }
                else if (isTargetInChargeAttackRange && neon.neonChargeAttackState.canAttack)
                {
                    stateMachine.ChangeState(neon.neonChargeAttackState);
                }
                else if (isTargetInRangedAttackRange && neon.neonRangedAttackState.canAttack)
                {
                    stateMachine.ChangeState(neon.neonRangedAttackState);
                }
                else if (neon.detection.currentTarget != null && neon.detection.currentPlatform != neon.detection.currentTarget.entityDetection.currentPlatform)
                {
                    stateMachine.ChangeState(neon.idleState);
                }

                movementOptionTimer.Tick();

                if (inDistance)
                {
                    if ((enemy.detection.currentTarget.transform.position.x - neon.transform.position.x) * facingDirection < 0)
                    {
                        enemy.movement.Flip();
                    }
                }
                
                if (!firstInDistance && neon.detection.currentTarget.rigidBody.velocity.x * (neon.detection.currentTarget.transform.position.x - neon.transform.position.x) > 0 && Vector2.Distance(enemy.detection.currentTarget.transform.position, enemy.transform.position) >= enemy.enemyData.adequateDistance)
                {
                    inDistance = false;
                    firstInDistance = true;
                    neon.animator.SetBool("idle", false);
                    neon.animator.SetBool("move", true);
                    movementOptionTimer.StopTimer();
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
        }
    }

    private void ChangeMovementOption()
    {
        if (Vector2.Distance(enemy.detection.currentTarget.transform.position, enemy.transform.position) < enemy.enemyData.adequateDistance)
        {
            inDistance = true;

            movementOption = (MovementOption)UtilityFunctions.RandomOption(new float[] { neon.neonData.haltPossibility, neon.neonData.approachPossibility, neon.neonData.retreatPossibility });

            switch (movementOption)
            {
                case MovementOption.Halt:
                    neon.animator.SetBool("move", false);
                    neon.animator.SetBool("idle", true);
                    break;

                case MovementOption.Approach:
                    neon.animator.SetBool("move", true);
                    neon.animator.SetBool("idle", false);
                    break;

                case MovementOption.Retreat:
                    neon.animator.SetBool("move", true);
                    neon.animator.SetBool("idle", false);
                    break;

                default:
                    Debug.LogWarning("Unexpected Movement Option: " + movementOption);
                    break;
            }

            movementOptionTimer.ChangeDuration(UtilityFunctions.RandomFloat(neon.neonData.minMovementOptionMaintainTime, neon.neonData.maxMovementOptionMaintainTime));
        }
        else
        {
            inDistance = false;
            firstInDistance = true;
            neon.animator.SetBool("move", true);
            neon.animator.SetBool("idle", false);
            movementOptionTimer.StopTimer();
        }
    }
}
