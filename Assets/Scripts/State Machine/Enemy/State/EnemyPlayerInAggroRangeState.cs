using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerInAggroRangeState : EnemyState
{
    protected Timer pathUpdateTimer;
    protected bool isPlayerInAggroRange;

    protected int currentWaypoint;
    protected Path path;

    public EnemyPlayerInAggroRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        pathUpdateTimer = new Timer(enemyData.pathUpdatePeriods);
        pathUpdateTimer.timerAction += UpdatePath;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInAggroRange = enemy.detection.isPlayerInAggroRange();
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
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        pathUpdateTimer.Tick();

        // enemy.movement.CheckIfShouldFlip(enemy.rigidBody.velocity.x);

        // Debug.Log("isPlayerInAggroRange: " + isPlayerInAggroRange);

        if (!isPlayerInAggroRange)
        {
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
        else if (isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        PathFollow();
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        if (isGrounded)
        {
            Vector2 directionBackup = Vector2.zero;
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - enemy.rigidBody.position).x > 0 ? Vector2.right : Vector2.left;
            Debug.Log($"currentWaypoint: {currentWaypoint}\nDistance: {Vector2.Distance(path.vectorPath[currentWaypoint], enemy.rigidBody.position)}\npath.vectorPath[currentWaypoint]: {path.vectorPath[currentWaypoint]}\nenemy.rigidBody.position: {enemy.rigidBody.position}");
            // Vector2 direction = enemy.detection.target.transform.position.x > enemy.rigidBody.position.x ? Vector2.right : Vector2.left;
            Vector2 destinationPosition = path.vectorPath[currentWaypoint];

            /*if (enemy.teleportState.isTeleportAvail)
            {
                for (int index = currentWaypoint; index < path.vectorPath.Count; index++)
                {
                    if (Vector2.Distance(path.vectorPath[index], enemy.rigidBody.position) < enemyData.maxMovementDistance)
                    {
                        if (!enemy.detection.isDetectingLedge(path.vectorPath[index]))
                        {
                            destinationPosition = path.vectorPath[index];
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                enemy.teleportState.SetDestinationPosition(destinationPosition);
                stateMachine.ChangeState(enemy.teleportState);
            }
            else*/ if (direction.y > epsilon && isDetectingWall && !isOnSlope)
            {
                for (int index = currentWaypoint; index < path.vectorPath.Count; index++)
                {
                    if (Vector2.Distance(path.vectorPath[index], enemy.rigidBody.position) < enemyData.maxMovementDistance)
                    {
                        if (!enemy.detection.isDetectingLedge(path.vectorPath[index]))
                        {
                            if (direction != Vector2.zero)
                            {
                                directionBackup = direction;
                            }

                            direction = enemy.combat.CalculateAngle(enemy.rigidBody.position, path.vectorPath[index], enemyData.jumpSpeed, enemy.rigidBody.gravityScale);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (direction != Vector2.zero)
                {
                    enemy.movement.SetVelocity(direction, enemyData.jumpSpeed);
                }
                else
                {
                    enemy.movement.SetVelocity(directionBackup, enemyData.jumpSpeed);
                }
            }
            else
            {
                Debug.Log("Enemy Speed: " + direction);
                enemy.movement.SetVelocity(direction, enemyData.moveSpeed);
            }
        }

        float distance = Vector2.Distance(enemy.rigidBody.position, path.vectorPath[currentWaypoint]);

        if (distance < enemyData.nextWaypointDistance)
        {
            currentWaypoint++;
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
}
