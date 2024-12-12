using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum ProjectileType { Straight, Throw, Bazier, Targeting }

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Projectile : PooledObject
{
    private enum StraightProjectileDirection { Forward, Toward, Manual };

    public Entity sourceEntity { get; protected set; }
    public Entity targetEntity { get; protected set; }

    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsTarget;
    [SerializeField] protected ProjectileType projectileType;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected bool explodeImmediately = true;
    [SerializeField] protected float explosionDurationTime;
    [SerializeField] protected bool canBeDeflected = true;
    [SerializeField] protected int maximumRelay = 3;
    [SerializeField] protected float autoDestroyTime = 30.0f;

    [Header("Bazier Projectile")]
    #region Bazier Projectile Variables
    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 prevPosition;
    private Vector2 currentPosition;

    private float transferTime;
    #endregion

    [Header("Targeting Projectile")]
    [SerializeField] private float minStep;
    [SerializeField] private float maxStep;
    [SerializeField] private float timeToMaxStep;

    [Header("Straight Projectile")]
    [SerializeField] private StraightProjectileDirection projectileDirection;
    [SerializeField, Tooltip("Lose Speed Time of zero means that the projectile won't lose its speed forever.")] private float loseSpeedTime;

    [Header("Throw Projectile")]
    [SerializeField] private float timeStepValue = 0.1f;
    [SerializeField] private float speedStepCount = 5.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float targetingSuccessDistance = 5.0f;

    #region Shared Variables
    protected Rigidbody2D rigidBody;
    protected Collider2D projectileCollider;
    protected bool isTouchedGround;
    protected bool isTouchedTarget;
    protected Coroutine projectileCoroutine;
    protected Vector2 initialPosition;
    protected Vector2 destinationPosition;
    private float elapsedTime;
    #endregion

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
    }

    protected virtual void OnEnable()
    {
        elapsedTime = 0.0f;
        isTouchedGround = false;
        isTouchedTarget = false;

        Invoke("ReleaseObject", autoDestroyTime);
    }

    protected IEnumerator FireProjectileCoroutine()
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            elapsedTime += Time.fixedDeltaTime;

            switch (projectileType)
            {
                case ProjectileType.Straight:
                    Rotate();
                    if (loseSpeedTime == 0.0f || (loseSpeedTime != 0.0f && elapsedTime < loseSpeedTime))
                    {
                        rigidBody.velocity = transform.right * projectileSpeed;
                    }
                    break;

                case ProjectileType.Targeting:
                    Rotate();
                    rigidBody.velocity = transform.right * projectileSpeed;
                    break;

                case ProjectileType.Bazier:
                    if (elapsedTime < transferTime)
                    {
                        prevPosition = currentPosition;
                        currentPosition = BezierCurve(initialPosition, pointA, pointB, destinationPosition, elapsedTime / transferTime);
                        if (currentPosition.y < prevPosition.y)
                        {
                            Rotate();
                            projectileSpeed = Mathf.Max(projectileSpeed, Vector2.Distance(prevPosition, currentPosition) / Time.fixedDeltaTime);
                        }
                        transform.position = currentPosition;
                    }
                    else
                    {
                        rigidBody.velocity = (currentPosition - prevPosition).normalized * projectileSpeed;
                    }
                    break;

                default:
                    break;
            }

            yield return waitForFixedUpdate;
        }
    }

    private void Rotate()
    {
        switch (projectileType)
        {
            case ProjectileType.Throw:
            case ProjectileType.Straight:
                float directionAngle = Mathf.Atan2(rigidBody.velocity.y, rigidBody.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(directionAngle, Vector3.forward);
                break;

            case ProjectileType.Bazier:
                float bazierAngle = Vector2.SignedAngle(Vector2.right, currentPosition - prevPosition);
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, bazierAngle);
                break;

            case ProjectileType.Targeting:
                float currentStep = Mathf.Lerp(minStep, maxStep, elapsedTime / timeToMaxStep);
                Vector2 targetDirection = targetEntity.transform.position - transform.position;
                float targetingAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                Quaternion toRotation = Quaternion.Euler(0.0f, 0.0f, targetingAngle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, currentStep * Time.deltaTime);
                break;

            default:
                break;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        isTouchedGround = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsGround);
        isTouchedTarget = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsTarget);

        if (isTouchedGround || isTouchedTarget)
        {
            if (projectileCoroutine != null)
            {
                StopCoroutine(projectileCoroutine);
            }
            rigidBody.velocity *= 0.8f;
            OnCollision();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Parry"))
        {
            // Check within angle range
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        isTouchedGround = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsGround);
        isTouchedTarget = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsTarget);

        if (isTouchedGround || isTouchedTarget)
        {
            rigidBody.velocity *= 0.99f;
        }
    }

    public void FireProjectile(Entity sourceEntity, Entity targetEntity, Vector2? manualDestinationPosition = null)
    {
        if (projectileCoroutine != null)
        {
            StopCoroutine(projectileCoroutine);
        }

        if (sourceEntity == null)
        {
            Debug.LogError($"Source Entity: {sourceEntity} is null.");
            ReleaseObject();
        }
        
        if (targetEntity == null && !manualDestinationPosition.HasValue)
        {
            Debug.LogError($"Target Entity: {targetEntity} is null and manual destination does not exist.");
            ReleaseObject();
        }

        this.sourceEntity = sourceEntity;
        this.targetEntity = targetEntity;
        initialPosition = transform.position;
        destinationPosition = manualDestinationPosition.HasValue ? manualDestinationPosition.Value : targetEntity.transform.position;

        switch (projectileType)
        {
            case ProjectileType.Bazier:
                float distance = Vector2.Distance(transform.position, destinationPosition);
                float xDifference = destinationPosition.x - transform.position.x;

                if (xDifference < 0)
                {
                    transform.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f);
                }

                Random rand = new Random();
                float pointAXOffset = -(float)rand.NextDouble() * xDifference / 4.0f - xDifference / 4.0f;
                float pointAYOffset = (float)rand.NextDouble() * distance / 4.0f + distance / 4.0f;
                pointA = new Vector2(pointAXOffset + transform.position.x, pointAYOffset + Mathf.Max(transform.position.y, destinationPosition.y));
                float pointBXOffset = (float)rand.NextDouble() * xDifference / 2.0f + xDifference / 4.0f;
                float pointBYOffset = ((float)rand.NextDouble() - 0.5f) * pointAYOffset + distance / 4.0f;
                pointB = new Vector2(pointBXOffset + pointA.x, pointBYOffset + pointA.y);
                
                transferTime = distance / projectileSpeed;
                currentPosition = transform.position;
                break;

            case ProjectileType.Straight:
                switch (projectileDirection)
                {
                    case StraightProjectileDirection.Forward:
                        transform.rotation = sourceEntity.transform.rotation;
                        break;

                    case StraightProjectileDirection.Toward:
                        Vector2 targetDirection = targetEntity.transform.position - transform.position;
                        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
                        break;

                    case StraightProjectileDirection.Manual:
                        Vector2 manualDirection = manualDestinationPosition.Value;
                        float manualAngle = Mathf.Atan2(manualDirection.y, manualDirection.x) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.AngleAxis(manualAngle, Vector3.forward);
                        break;

                    default:
                        break;
                }
                break;

            case ProjectileType.Throw:
                Vector2? projectileVelocity = CalculateProjectileVelocity(transform.position, destinationPosition);

                if (projectileVelocity.HasValue)
                {
                    rigidBody.velocity = projectileVelocity.Value;
                }
                else
                {
                    ReleaseObject();
                }
                break;

            default:
                break;
        }

        projectileCoroutine = StartCoroutine(FireProjectileCoroutine());
    }

    protected virtual void OnCollision()
    {
        if (explodeImmediately)
        {
            Explode();
        }
        else
        {
            Invoke("Explode", explosionDurationTime);
        }
    }

    protected void Explode()
    {
        GameObject projectileExplosion = Manager.Instance.objectPoolingManager.GetGameObject(gameObject.name + "Explosion");
        projectileExplosion.transform.position = transform.position;

        ReleaseObject();
    }

    public void ProjetileDeflected(float? projectileSpeed)
    {
        if (projectileSpeed.HasValue)
        {
            this.projectileSpeed = projectileSpeed.Value;
        }

        /*if (sourceEntity.isDead)
        {

        }*/
        FireProjectile(targetEntity, sourceEntity);
    }

    public Vector2? CalculateProjectileVelocity(Vector2 projectileFirePosition, Vector2 targetPosition)
    {
        float distance = Vector2.Distance(projectileFirePosition, targetPosition);
        List<Tuple<Vector2, bool>> availableVelocities = new List<Tuple<Vector2, bool>>();
        float gravityAccelaration = Mathf.Abs(Physics2D.gravity.y * rigidBody.gravityScale);
        float xDifference = targetPosition.x - projectileFirePosition.x;
        float yDifference = targetPosition.y - projectileFirePosition.y;
        float currentSpeed = projectileSpeed / speedStepCount;

        while (currentSpeed <= projectileSpeed)
        {
            float speedSquare = Mathf.Pow(currentSpeed, 2);
            float rootUnderValue = Mathf.Pow(speedSquare, 2) - gravityAccelaration * (gravityAccelaration * Mathf.Pow(xDifference, 2) + 2 * yDifference * speedSquare);

            if (rootUnderValue >= 0.0f)
            {
                float lowAngle = Mathf.Atan2(speedSquare - Mathf.Sqrt(rootUnderValue), gravityAccelaration * xDifference);
                float highAngle = Mathf.Atan2(speedSquare + Mathf.Sqrt(rootUnderValue), gravityAccelaration * xDifference);
                Vector2 highAngleVector = new Vector2(Mathf.Cos(highAngle), Mathf.Sin(highAngle));
                Vector2 lowAngleVector = new Vector2(Mathf.Cos(lowAngle), Mathf.Sin(lowAngle));

                if (CheckProjectileRoute(projectileFirePosition, targetPosition, currentSpeed * lowAngleVector))
                {
                    availableVelocities.Add(new Tuple<Vector2, bool>(lowAngleVector * currentSpeed, true));
                }
                else if (CheckProjectileRoute(projectileFirePosition, targetPosition, currentSpeed * highAngleVector))
                {
                    availableVelocities.Add(new Tuple<Vector2, bool>(highAngleVector * currentSpeed, false));
                }
            }

            currentSpeed += speedStepCount;
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
        float distance = Vector2.Distance(projectileFirePosition, targetPosition);
        float xDifference = targetPosition.x - projectileFirePosition.x;
        float expectedTravelDuration = xDifference / projectileVelocity.x;
        float timeStep = Mathf.Min(timeStepValue, expectedTravelDuration / 5.0f);

        float timeElapsed = 0.0f;
        Vector2 prevPosition;
        Vector2 currentPosition = projectileFirePosition;

        while (timeElapsed < expectedTravelDuration)
        {
            Vector2 movementVector = new Vector2(projectileVelocity.x * timeElapsed, projectileVelocity.y * timeElapsed + 0.5f * Physics2D.gravity.y * rigidBody.gravityScale * Mathf.Pow(timeElapsed, 2));
            prevPosition = currentPosition;
            currentPosition = movementVector + projectileFirePosition;
            Vector2 directionVector = currentPosition - prevPosition;
            float directionAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

            if (Physics2D.BoxCast(prevPosition, projectileCollider.bounds.size, directionAngle, directionVector, Vector2.Distance(currentPosition, prevPosition), whatIsGround))
            {
                if (Vector2.Distance(currentPosition, targetPosition) < targetingSuccessDistance)
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

    private Vector2 BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float rate)
    {
        Vector2 a = Vector2.Lerp(p1, p2, rate);
        Vector2 b = Vector2.Lerp(p2, p3, rate);
        Vector2 c = Vector2.Lerp(p3, p4, rate);

        Vector2 d = Vector2.Lerp(a, b, rate);
        Vector2 e = Vector2.Lerp(b, c, rate);

        Vector2 f = Vector2.Lerp(d, e, rate);

        return f;
    }
}
