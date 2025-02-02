using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnisTargetInAggroRangeState : EnemyTargetInAggroRangeState
{
    private enum MovementOption { Halt, Approach, Retreat };

    private Anis anis;

    private bool initialFlag;
    private bool isTargetInMinDistanceRange;
    private bool isTargetInAdequateDistanceRange;
    private bool keepPreviousMoving;
    private bool isTargetInMeleeAttack0Range;
    private bool isTargetInMeleeAttack1Range;
    private bool isTargetInMeleeAttack2Range;
    private bool isTargetInMeleeAttack3Range;
    private bool isTargetInRangedAttackRange;
    private bool isTargetInCloseRangedAttackRange;
    private MovementOption movementOption;
    private Timer movementOptionTimer;

    public AnisTargetInAggroRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        anis = enemy as Anis;
        movementOptionTimer = new Timer(UtilityFunctions.RandomFloat(anis.anisData.minMovementOptionMaintainTime, anis.anisData.maxMovementOptionMaintainTime));
        movementOptionTimer.timerAction += ChangeMovementOption;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInMinDistanceRange = IsTargetInRange(anis.anisData.minDistance);
        isTargetInAdequateDistanceRange = IsTargetInRange(anis.anisData.adequateDistance);
        isTargetInMeleeAttack0Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack0[0]) || anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack0[1]);
        isTargetInMeleeAttack1Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack1[0]);
        isTargetInMeleeAttack2Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack2[0]);
        isTargetInMeleeAttack3Range = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack3[0]) || anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.meleeAttack3[1]);
        isTargetInRangedAttackRange = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.rangedAttack[0]);
        isTargetInCloseRangedAttackRange = anis.anisCombat.IsTargetInRangeOf(anis.anisCombat.closeRangedAttack[0]);
    }

    public override void Enter()
    {
        base.Enter();

        initialFlag = true;
    }

    public override void Exit()
    {
        base.Exit();

        initialFlag = true;
        anis.animator.SetBool("move", false);
        anis.animator.SetBool("idle", false);
        anis.animator.SetBool("backStep", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (isDetectingLedgeBack)
                {
                    if (isTargetInMeleeAttack0Range && anis.anisMeleeAttackState.canAttack)
                    {
                        stateMachine.ChangeState(anis.anisMeleeAttackState);
                    }
                    else if (isTargetInCloseRangedAttackRange && anis.anisCloseRangedAttackState.CanAttack())
                    {
                        stateMachine.ChangeState(anis.anisCloseRangedAttackState);
                    }
                    else if (isTargetInRangedAttackRange && anis.anisRangedAttackState.CanAttack())
                    {
                        stateMachine.ChangeState(anis.anisRangedAttackState);
                    }
                }
                else
                {
                    if (isTargetInMinDistanceRange)
                    {
                        if (isTargetInMeleeAttack0Range)
                        {
                            if (anis.anisMeleeAttackState.canAttack)
                            {
                                stateMachine.ChangeState(anis.anisMeleeAttackState);
                            }
                            else
                            {
                                if (isTargetInCloseRangedAttackRange && anis.anisCloseRangedAttackState.CanAttack())
                                {
                                    stateMachine.ChangeState(anis.anisCloseRangedAttackState);
                                }
                                else if (isTargetInRangedAttackRange && anis.anisRangedAttackState.CanAttack())
                                {
                                    stateMachine.ChangeState(anis.anisRangedAttackState);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isTargetInMeleeAttack0Range && anis.anisMeleeAttackState.canAttack)
                        {
                            stateMachine.ChangeState(anis.anisMeleeAttackState);
                        }
                        else if (isTargetInCloseRangedAttackRange && anis.anisCloseRangedAttackState.CanAttack())
                        {
                            stateMachine.ChangeState(anis.anisCloseRangedAttackState);
                        }
                        else if (isTargetInRangedAttackRange && anis.anisRangedAttackState.CanAttack())
                        {
                            stateMachine.ChangeState(anis.anisRangedAttackState);
                        }
                    }
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if ((enemy.detection.currentTarget.transform.position.x - enemy.rigidBody.position.x) * facingDirection < 0)
            {
                enemy.movement.Flip();
            }

            if (isTargetInMinDistanceRange)
            {
                initialFlag = true;

                if (isDetectingLedgeBack)
                {
                    anis.animator.SetBool("move", false);
                    anis.animator.SetBool("idle", true);
                    anis.movement.SetVelocityX(0.0f);
                    movementOption = MovementOption.Halt;
                    RigidBodyController();
                }
                else
                {
                    anis.animator.SetBool("move", true);
                    anis.animator.SetBool("idle", false);
                    anis.animator.SetBool("backStep", true);
                    anis.movement.SetVelocityX(-anis.anisData.moveSpeed * anis.movement.facingDirection, true);
                    movementOption = MovementOption.Retreat;
                    RigidBodyController(false);
                }
            }
            else if (isTargetInAdequateDistanceRange)
            {
                if (initialFlag)
                {
                    if (isTargetInMinDistanceRange)
                    {
                        ChangeMovementOption();
                    }
                    movementOptionTimer.StartMultiUseTimer();
                    initialFlag = false;
                }

                SetAnimator();
            }
            else if (isTargetInAggroRange && !anis.detection.isDetectingLedgeFront())
            {
                initialFlag = true;

                SetAnimator();
            }
        }
    }

    private bool IsTargetInRange(float distance)
    {
        return Physics2D.OverlapCircleAll(anis.transform.position, distance, anis.detection.whatIsChaseTarget).Select(collider => collider.GetComponent<Entity>()).Contains(anis.detection.currentTarget);
    }

    private void SetAnimator()
    {
        switch (movementOption)
        {
            case MovementOption.Halt:
                anis.animator.SetBool("move", false);
                anis.animator.SetBool("idle", true);
                anis.movement.SetVelocityX(0.0f);
                RigidBodyController();
                break;

            case MovementOption.Approach:
                if (isDetectingLedgeFront)
                {
                    anis.animator.SetBool("move", false);
                    anis.animator.SetBool("idle", true);
                    anis.movement.SetVelocityX(0.0f);
                }
                else
                {
                    anis.animator.SetBool("move", true);
                    anis.animator.SetBool("idle", false);
                    anis.animator.SetBool("backStep", false);
                    anis.movement.SetVelocityX(anis.anisData.moveSpeed * anis.movement.facingDirection, true);
                }
                RigidBodyController();
                break;

            case MovementOption.Retreat:
                if (isDetectingLedgeBack)
                {
                    anis.animator.SetBool("move", false);
                    anis.animator.SetBool("idle", true);
                    anis.movement.SetVelocityX(0.0f);
                }
                else
                {
                    anis.animator.SetBool("move", true);
                    anis.animator.SetBool("idle", false);
                    anis.animator.SetBool("backStep", true);
                    anis.movement.SetVelocityX(-anis.anisData.moveSpeed * anis.movement.facingDirection, true);
                }
                RigidBodyController(false);
                break;

            default:
                break;
        }

        movementOptionTimer.Tick();
    }

    private void ChangeMovementOption()
    {
        if (!isTargetInAdequateDistanceRange)
        {
            movementOption = (MovementOption)UtilityFunctions.RandomOption(new float[] { anis.anisData.haltPossibility, anis.anisData.approachPossibility, 0.0f });
        }
        else
        {
            movementOption = (MovementOption)UtilityFunctions.RandomOption(new float[] { anis.anisData.haltPossibility, 0.0f, anis.anisData.retreatPossibility });
        }
        
        movementOptionTimer.ChangeDuration(UtilityFunctions.RandomFloat(anis.anisData.minMovementOptionMaintainTime, anis.anisData.maxMovementOptionMaintainTime));
    }
}
