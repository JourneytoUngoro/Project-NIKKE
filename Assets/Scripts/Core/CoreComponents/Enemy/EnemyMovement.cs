using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Movement
{
    [SerializeField] private float maxInAirTime;
    [SerializeField] private float speedStepValue;
    [SerializeField] private float timeStepValue;

    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
    }

    public Vector2? CalculateJumpAngle(Vector2 jumpInitialPosition, Vector2 targetPosition, float jumpSpeed, float entityGravityScale, Collider2D projectileCollider = null)
    {
        float distance = Vector2.Distance(jumpInitialPosition, targetPosition);
        float gravityAccelaration = Mathf.Abs(Physics2D.gravity.y * entityGravityScale);
        float xDifference = targetPosition.x - jumpInitialPosition.x;
        float yDifference = targetPosition.y - jumpInitialPosition.y;
        float speedSquare = Mathf.Pow(jumpSpeed, 2);
        float rootUnderValue = Mathf.Pow(speedSquare, 2) - gravityAccelaration * (gravityAccelaration * Mathf.Pow(xDifference, 2) + 2 * yDifference * speedSquare);

        if (rootUnderValue >= 0.0f)
        {
            float lowAngle = Mathf.Atan2(speedSquare - Mathf.Sqrt(rootUnderValue), gravityAccelaration * xDifference);
            float highAngle = Mathf.Atan2(speedSquare + Mathf.Sqrt(rootUnderValue), gravityAccelaration * xDifference);
            Vector2 highAngleVector = new Vector2(Mathf.Cos(highAngle), Mathf.Sin(highAngle));
            Vector2 lowAngleVector = new Vector2(Mathf.Cos(lowAngle), Mathf.Sin(lowAngle));

            if (CheckJumpRoute(jumpInitialPosition, targetPosition, jumpSpeed * lowAngleVector, entityGravityScale, projectileCollider))
            {
                return lowAngleVector;
            }
            else
            {
                if (CheckJumpRoute(jumpInitialPosition, targetPosition, jumpSpeed * highAngleVector, entityGravityScale, projectileCollider))
                {
                    return highAngleVector;
                }
                else
                {
                    return null;
                }
            }
        }
        else return null;
    }

    private bool CheckJumpRoute(Vector2 jumpInitialPosition, Vector2 targetPosition, Vector2 jumpVelocity, float gravityScale, Collider2D projectileCollider = null)
    {
        float timeElapsed = 0.0f;
        float xDifference = targetPosition.x - jumpInitialPosition.x;
        float expectedTravelDuration = xDifference / jumpVelocity.x;
        float timeStep = expectedTravelDuration / 20.0f;

        Vector2 prevPosition;
        Vector2 currentPosition = jumpInitialPosition;
        for (int i = 0; i < 20; i++)
        {
            timeElapsed = timeStep * i;
            Vector2 movementVector = new Vector2(jumpVelocity.x * timeElapsed, jumpVelocity.y * timeElapsed + 0.5f * Physics2D.gravity.y * gravityScale * Mathf.Pow(timeElapsed, 2));
            prevPosition = currentPosition;
            currentPosition = movementVector + jumpInitialPosition;

            if (projectileCollider == null)
            {
                if (Physics2D.Raycast(prevPosition, currentPosition - prevPosition, Vector2.Distance(currentPosition, prevPosition), enemy.detection.whatIsGround))
                {
                    return false;
                }
            }
            else if (projectileCollider is BoxCollider2D)
            {
                if (Physics2D.BoxCast(prevPosition, (projectileCollider as BoxCollider2D).size, 0.0f, currentPosition - prevPosition, Vector2.Distance(currentPosition, prevPosition), enemy.detection.whatIsGround))
                {
                    return false;
                }
            }
            else if (projectileCollider is CircleCollider2D)
            {
                if (Physics2D.CircleCast(prevPosition, (projectileCollider as CircleCollider2D).radius, currentPosition - prevPosition, Vector2.Distance(currentPosition, prevPosition), enemy.detection.whatIsGround))
                {
                    return false;
                }
            }
            else if (projectileCollider is CapsuleCollider2D)
            {
                if (Physics2D.CapsuleCast(prevPosition, (projectileCollider as CapsuleCollider2D).size, (projectileCollider as CapsuleCollider2D).direction, 0.0f, currentPosition - prevPosition, Vector2.Distance(currentPosition, prevPosition), enemy.detection.whatIsGround))
                {
                    return false;
                }
            }
        }

        timeStep = (maxInAirTime - timeElapsed) / 20.0f;
        while (timeElapsed < maxInAirTime)
        {
            timeElapsed += timeStep;
            Vector2 movementVector = new Vector2(jumpVelocity.x * timeElapsed, jumpVelocity.y * timeElapsed + 0.5f * Physics2D.gravity.y * gravityScale * Mathf.Pow(timeElapsed, 2));
            prevPosition = currentPosition;
            currentPosition = movementVector + jumpInitialPosition;

            if (Physics2D.Linecast(prevPosition, currentPosition, enemy.detection.whatIsGround))
            {
                return true;
            }
        }

        return false;
    }

    public Vector2? CalculateProjectileVelocity(Vector2 projectileFirePosition, Vector2 targetPosition, bool checkProjectileRoute)
    {
        float distance = Vector2.Distance(projectileFirePosition, targetPosition);
        List<Tuple<Vector2, bool>> availableVelocities = new List<Tuple<Vector2, bool>>();
        float gravityAccelaration = Mathf.Abs(Physics2D.gravity.y * enemy.rigidBody.gravityScale);
        float xDifference = targetPosition.x - projectileFirePosition.x;
        float yDifference = targetPosition.y - projectileFirePosition.y;
        float currentSpeed = speedStepValue;

        while (currentSpeed <= enemy.enemyData.jumpSpeed)
        {
            float speedSquare = Mathf.Pow(currentSpeed, 2);
            float rootUnderValue = Mathf.Pow(speedSquare, 2) - gravityAccelaration * (gravityAccelaration * Mathf.Pow(xDifference, 2) + 2 * yDifference * speedSquare);

            if (rootUnderValue >= 0.0f)
            {
                float lowAngle = Mathf.Atan2(speedSquare - Mathf.Sqrt(rootUnderValue), gravityAccelaration * xDifference);
                float highAngle = Mathf.Atan2(speedSquare + Mathf.Sqrt(rootUnderValue), gravityAccelaration * xDifference);
                Vector2 highAngleVector = new Vector2(Mathf.Cos(highAngle), Mathf.Sin(highAngle));
                Vector2 lowAngleVector = new Vector2(Mathf.Cos(lowAngle), Mathf.Sin(lowAngle));

                if (checkProjectileRoute)
                {
                    if (CheckProjectileRoute(projectileFirePosition, targetPosition, currentSpeed * lowAngleVector))
                    {
                        availableVelocities.Add(new Tuple<Vector2, bool>(lowAngleVector * currentSpeed, true));
                    }
                    else if (CheckProjectileRoute(projectileFirePosition, targetPosition, currentSpeed * highAngleVector))
                    {
                        availableVelocities.Add(new Tuple<Vector2, bool>(highAngleVector * currentSpeed, false));
                    }
                }
                else
                {
                    availableVelocities.Add(new Tuple<Vector2, bool>(lowAngleVector * currentSpeed, true));
                    availableVelocities.Add(new Tuple<Vector2, bool>(highAngleVector * currentSpeed, false));
                }
            }

            currentSpeed += speedStepValue;
        }

        Tuple<Vector2, bool> bestVelocity = null;

        foreach (Tuple<Vector2, bool> availableVelocity in availableVelocities)
        {
            if (bestVelocity == null)
            {
                bestVelocity = availableVelocity;
            }
            else
            {
                if (bestVelocity.Item2)
                {
                    if (availableVelocity.Item1.magnitude > bestVelocity.Item1.magnitude)
                    {
                        bestVelocity = availableVelocity;
                    }
                }
                else
                {
                    if (availableVelocity.Item2)
                    {
                        bestVelocity = availableVelocity;
                    }
                    else if (availableVelocity.Item1.magnitude < bestVelocity.Item1.magnitude)
                    {
                        bestVelocity = availableVelocity;
                    }
                }
            }
        }

        return bestVelocity != null ? bestVelocity.Item1 : null;
    }

    private bool CheckProjectileRoute(Vector2 projectileFirePosition, Vector2 targetPosition, Vector2 projectileVelocity)
    {
        // float distance = Vector2.Distance(projectileFirePosition, targetPosition);
        float xDifference = targetPosition.x - projectileFirePosition.x;
        float expectedTravelDuration = xDifference / projectileVelocity.x;
        float timeStep = Mathf.Min(timeStepValue, expectedTravelDuration / 5.0f);

        float timeElapsed = 0.0f;
        Vector2 prevPosition;
        Vector2 currentPosition = projectileFirePosition;

        while (timeElapsed < expectedTravelDuration)
        {
            Vector2 movementVector = new Vector2(projectileVelocity.x * timeElapsed, projectileVelocity.y * timeElapsed + 0.5f * Physics2D.gravity.y * enemy.rigidBody.gravityScale * Mathf.Pow(timeElapsed, 2));
            prevPosition = currentPosition;
            currentPosition = movementVector + projectileFirePosition;
            Vector2 directionVector = currentPosition - prevPosition;
            float directionAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

            if (Physics2D.BoxCast(prevPosition, enemy.entityCollider.bounds.size, directionAngle, directionVector, Vector2.Distance(currentPosition, prevPosition), enemy.detection.whatIsGround))
            {
                if (Vector2.Distance(currentPosition, targetPosition) < 5.0f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            timeElapsed += timeStep;
        }

        return true;
    }

    private bool LandingSuccess(Vector2 currentPosition, Vector2 targetPosition)
    {
        if (enemy.entityCollider.bounds.Contains(targetPosition))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
