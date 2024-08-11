using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetInAggroRangeState : EnemyState
{
    protected Timer pathUpdateTimer;
    protected bool isPlayerInAggroRange;
    protected bool isPlayerInMeleeAttackRange;
    protected bool isPlayerInMidRAttackRange;
    protected bool isPlayerInRangedAttackRange;
    protected bool shouldJump;

    protected int currentWaypoint;
    protected Path path;

    private Vector2 prevDirection;
    private bool didJump;

    public EnemyTargetInAggroRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        pathUpdateTimer = new Timer(enemyData.pathUpdatePeriods);
        pathUpdateTimer.timerAction += UpdatePath;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        shouldJump = enemy.detection.ShouldJump();
        isPlayerInAggroRange = enemy.detection.isPlayerInAggroRange();
        isPlayerInMeleeAttackRange = enemy.combat.isTargetInMeleeAttackRange();
        isPlayerInMidRAttackRange = enemy.combat.isTargetInMidRAttackRange();
        isPlayerInRangedAttackRange = enemy.combat.isTargetInRangedAttackRange();
    }

    public override void Enter()
    {
        base.Enter();

        currentWaypoint = 0;
        pathUpdateTimer.StartMultiUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        pathUpdateTimer.StopTimer();
        enemy.movement.SetVelocityX(0.0f);
        enemy.movement.SetVelocityMultiplier(Vector2.one);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (enemyData.canSmartPathFind)
            {
                pathUpdateTimer.Tick();
            }

            if (isGrounded)
            {
                if (!isPlayerInAggroRange)
                {
                    stateMachine.ChangeState(enemy.lookForTargetState);
                }
                else if (enemyData.canMeleeAttack && isPlayerInMeleeAttackRange && enemy.meleeAttackState.canAttack)
                {
                    stateMachine.ChangeState(enemy.meleeAttackState);
                }
                else if (enemyData.canMidRAttack && isPlayerInMidRAttackRange && enemy.midRAttackState.canAttack)
                {
                    stateMachine.ChangeState(enemy.midRAttackState);
                }
                else if (enemyData.canRangedAttack && isPlayerInRangedAttackRange && enemy.rangedAttackState.canAttack)
                {
                    stateMachine.ChangeState(enemy.rangedAttackState);
                }
                else if (isDetectingLedge && !enemyData.canFallDownLedge)
                {
                    stateMachine.ChangeState(enemy.idleState);
                }
            }
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

    protected void AStartPathFollow()
    {
        if (((Vector2)enemy.detection.target.transform.position - enemy.rigidBody.position).magnitude > Mathf.Abs(enemy.detection.target.GetComponent<Collider2D>().bounds.size.y - enemy.entityCollider.bounds.size.y) / 2.0f * 1.2f)
        {
            enemy.rigidBody.gravityScale = 9.5f;

            if (path == null)
            {
                enemy.rigidBody.gravityScale = isOnSlope ? 0.0f : 9.5f;
                enemy.movement.SetVelocityX(0.0f);
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                currentWaypoint = path.vectorPath.Count - 1;
            }

            if (isGrounded)
            {
                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - enemy.rigidBody.position).normalized;

                if (direction.x * (enemy.detection.target.transform.position.x - enemy.rigidBody.position.x) < 0)
                {
                    direction = prevDirection;
                }
                else
                {
                    prevDirection = direction;
                }
                // 가끔씩 A* 알고리즘이 잘못된 방향을 제시하는 오류를 방지

                if (direction.x * facingDirection < 0)
                {
                    enemy.movement.Flip();
                }

                if (isOnSlope)
                {
                    workSpace.Set(enemy.detection.slopePerpNormal.x * facingDirection, enemy.detection.slopePerpNormal.y * facingDirection);
                    enemy.movement.SetVelocity(workSpace * enemyData.moveSpeed);
                }
                else if ((!isOnSlope && shouldJump && path.vectorPath[currentWaypoint].y - enemy.rigidBody.position.y > enemyData.jumpHeightRequirement) || isDetectingLedge)
                {
                    if (enemy.detection.InStepbackDistance() && !didJump)
                    {
                        enemy.movement.SetVelocityX(-facingDirection * enemyData.moveSpeed);
                    }
                    else
                    {
                        didJump = true;
                        direction = Vector2.zero;

                        for (int index = currentWaypoint + 1; index < path.vectorPath.Count; index++)
                        {
                            direction = enemy.movement.CalculateJumpAngle(enemy.rigidBody.position, path.vectorPath[index], enemyData.jumpSpeed, enemy.rigidBody.gravityScale) ?? direction;
                            // direction = enemy.combat.CalculateProjectileAngle(enemy.rigidBody.position, path.vectorPath[index], enemyData.jumpSpeed, 9.5f) ?? direction;
                        }

                        if (direction.x * (enemy.detection.target.transform.position.x - enemy.rigidBody.position.x) >= 0)
                        {
                            enemy.movement.SetVelocity(direction * enemyData.jumpSpeed);
                        }
                    }
                }
                else
                {
                    enemy.movement.SetVelocityMultiplier(Vector2.one);
                    enemy.movement.SetVelocityX(facingDirection * enemyData.moveSpeed);
                    enemy.movement.SetVelocityLimitY(0.0f);
                }
            }
            else
            {
                didJump = false;
                enemy.rigidBody.gravityScale = 9.5f;
            }
            
            float distance = Vector2.Distance(enemy.rigidBody.position, path.vectorPath[currentWaypoint]);

            if (distance < enemyData.nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
        else if (isGrounded)
        {
            enemy.rigidBody.gravityScale = isOnSlope ? 0.0f : 9.5f;

            enemy.movement.SetVelocityX(0.0f);
        }
    }

    private void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            this.path = path;
            currentWaypoint = 0;
        }
    }

    public void UpdatePath()
    {
        if (isPlayerInAggroRange && enemy.seeker.IsDone())
        {
            enemy.seeker.StartPath(enemy.rigidBody.position, enemy.detection.target.transform.position, OnPathComplete);
        }
    }

    protected virtual void NoAStarPathFollow()
    {
        if (Mathf.Abs(enemy.detection.target.transform.position.x - enemy.rigidBody.position.x) > 1.0f)
        {
            if ((enemy.detection.target.transform.position.x - enemy.rigidBody.position.x) * facingDirection < 0)
            {
                enemy.movement.Flip();
            }

            if (isGrounded)
            {
                if (isOnSlope)
                {
                    enemy.rigidBody.gravityScale = 0.0f;

                    if (enemy.detection.slopePerpNormal.y * facingDirection > 0)
                    {
                        enemy.movement.SetVelocityMultiplier(Vector2.one * 0.8f);
                    }
                    else
                    {
                        enemy.movement.SetVelocityMultiplier(Vector2.one * 1.4f);
                    }

                    workSpace.Set(enemy.detection.slopePerpNormal.x * facingDirection, enemy.detection.slopePerpNormal.y * facingDirection);

                    enemy.movement.SetVelocity(workSpace * enemyData.moveSpeed);
                }
                else
                {
                    enemy.rigidBody.gravityScale = 9.5f;
                    enemy.movement.SetVelocityX(facingDirection * enemyData.moveSpeed);
                    enemy.movement.SetVelocityLimitY(0.0f);
                }
            }
            else
            {
                enemy.rigidBody.gravityScale = 9.5f;
                enemy.movement.SetVelocityX(facingDirection * enemyData.moveSpeed);
            }
        }
        else
        {
            enemy.rigidBody.gravityScale = isOnSlope ? 0.0f : 9.5f;

            enemy.movement.SetVelocityX(0.0f);
        }
    }

    protected void IgnoreGroundPathFollow()
    {
        Vector2 directionVector = ((Vector2)enemy.detection.target.transform.position - enemy.rigidBody.position).normalized;
        
        if (directionVector.x * facingDirection < 0)
        {
            enemy.movement.Flip();
        }
        
        enemy.movement.SetVelocity(directionVector * enemyData.moveSpeed);
    }
}
