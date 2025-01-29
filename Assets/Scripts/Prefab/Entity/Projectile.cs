using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GD.MinMaxSlider;

public enum ProjectileType { None, Straight, Throw, Bazier, Targeting }
public enum StraightProjectileDirection { Forward, Toward, Manual };

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : PooledObject
{
    public Entity sourceEntity { get; protected set; }
    public Entity targetEntity { get; protected set; }

    [field: SerializeField] public OverlapCollider overlapCollider { get; protected set; }
    [SerializeField] protected ProjectileType projectileType;
    [SerializeField] protected ProjectileType projectileTypeWhenParried;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected bool piercingProjectile;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float explosionDurationTime = 0.0f;
    [SerializeField] protected int maximumDeflection = 1;
    [SerializeField] protected float autoDestroyTime = 30.0f;

    [Header("Bazier Projectile")]
    #region Bazier Projectile Variables
    [SerializeField] private float pointAXOffsetBase = 0.0f;
    [SerializeField, MinMaxSlider(-0.5f, 0.5f)] private Vector2 pointAXOffsetDeviation = new Vector2 (-0.25f, 0.25f);
    [SerializeField] private float pointAYOffsetBase = 0.25f;
    [SerializeField, MinMaxSlider(-0.5f, 0.5f)] private Vector2 pointAYOffsetDeviation = new Vector2(0.0f, 0.25f);
    [SerializeField, Min(0.0f)] private float pointBXOffsetBase = 0.0f;
    [SerializeField, MinMaxSlider(-0.5f, 0.5f)] private Vector2 pointBXOffsetDeviation = new Vector2(0.0f, 0.5f);
    [SerializeField] private float pointBYOffsetBase = 0.25f;
    [SerializeField, MinMaxSlider(-0.5f, 0.5f)] private Vector2 pointBYOffsetDeviation = new Vector2(-0.5f, -0.25f);
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
    [SerializeField] private StraightProjectileDirection straightProjectileDirection;
    [SerializeField, Tooltip("Lose Speed Time of zero means that the projectile won't lose its speed forever.")] private float loseSpeedTime;

    [Header("Throw Projectile")]
    [SerializeField] private float timeStepValue = 0.1f;
    [SerializeField] private float speedStepValue = 5.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float targetingSuccessDistance = 5.0f;

    #region Shared Variables
    [SerializeField] private Transform knockbackSourceTransform;
    [field: SerializeField] public CombatAbility combatAbility { get; private set; }
    private List<Collider2D> damagedTargets = new List<Collider2D>();

    protected Rigidbody2D rigidBody;
    protected Collider2D projectileCollider;
    protected LayerMask whatIsDamageable;
    protected bool isTouchedGround;
    protected bool isTouchedTarget;
    protected Coroutine projectileCoroutine;
    protected Vector2 initialPosition;
    protected Vector2 destinationPosition;
    private float elapsedTime;
    private int currentDeflection;
    #endregion

    #region Other Variables
    private ProjectileType initialProjectileType;
    private float initialProjectileSpeed;
    private LayerMask initialTarget;
    #endregion

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        initialProjectileType = projectileType;
        initialProjectileSpeed = projectileSpeed;
        initialTarget = whatIsDamageable;

        foreach (CombatAbilityComponent combatAbilityComponent in combatAbility.combatAbilityComponents)
        {
            combatAbilityComponent.pertainedCombatAbility = combatAbility;

            if (combatAbilityComponent.GetType().Equals(typeof(KnockbackComponent)))
            {
                KnockbackComponent knockbackComponent = combatAbilityComponent as KnockbackComponent;
                knockbackComponent.knockbackSourceTransform = knockbackSourceTransform ? knockbackSourceTransform : transform;
            }
        }
    }

    protected virtual void OnEnable()
    {
        whatIsDamageable = initialTarget;
        projectileType = initialProjectileType;
        CancelInvoke("ReleaseObject");
        Invoke("ReleaseObject", autoDestroyTime);
    }

    public override void ReleaseObject()
    {
        base.ReleaseObject();

        elapsedTime = 0.0f;
        currentDeflection = 0;
        isTouchedGround = false;
        isTouchedTarget = false;
        sourceEntity = null;
        damagedTargets.Clear();
        CancelInvoke("ReleaseObject");
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
                    if (loseSpeedTime == 0.0f || (loseSpeedTime != 0.0f && elapsedTime < loseSpeedTime))
                    {
                        rigidBody.velocity = transform.right * projectileSpeed;
                    }
                    Rotate();
                    break;

                case ProjectileType.Targeting:
                    rigidBody.velocity = transform.right * projectileSpeed;
                    Rotate();
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
        isTouchedTarget = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsDamageable);

        if (isTouchedGround || isTouchedTarget)
        {
            if (projectileCoroutine != null)
            {
                StopCoroutine(projectileCoroutine);
            }
            rigidBody.velocity *= 0.8f;
            OnCollision(collision.collider);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Parry"))
        {
            // Check within angle range
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        isTouchedGround = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsGround);
        isTouchedTarget = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsDamageable);

        if (isTouchedGround || isTouchedTarget)
        {
            rigidBody.velocity *= 0.99f;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        isTouchedGround = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsGround);
        isTouchedTarget = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsDamageable);

        if (isTouchedTarget)
        {
            if (projectileCoroutine != null)
            {
                StopCoroutine(projectileCoroutine);
            }
            rigidBody.velocity *= 0.8f;
            OnCollision(collision);
        }

        if (isTouchedGround)
        {
            ReleaseObject();
        }
    }

    public void FireProjectile(Entity sourceEntity, Entity[] targetEntities, bool checkProjectileRoute, Vector2? manualDirectionVector = null)
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

        if ((projectileType == ProjectileType.Bazier || projectileType == ProjectileType.Throw || (projectileType == ProjectileType.Straight && straightProjectileDirection == StraightProjectileDirection.Toward)) && (targetEntities == null || targetEntities.Count() == 0))
        {
            Debug.LogError($"{gameObject.name}: Projectile type of {projectileType} needs target entity which is null or does not exist.");
            ReleaseObject();
        }

        bool fireProjectile = true;
        this.sourceEntity = sourceEntity;
        this.whatIsDamageable = sourceEntity.entityCombat.whatIsDamageable;
        initialPosition = transform.position;

        if (projectileType == ProjectileType.Straight && straightProjectileDirection != StraightProjectileDirection.Toward)
        {
            switch (straightProjectileDirection)
            {
                case StraightProjectileDirection.Forward:
                    transform.rotation = sourceEntity.transform.rotation;
                    break;

                case StraightProjectileDirection.Manual:
                    Vector2 manualDirection = manualDirectionVector.Value;
                    float manualAngle = Mathf.Atan2(manualDirection.y, manualDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(manualAngle, Vector3.forward);
                    break;

                default:
                    break;
            }
        }
        else
        {
            foreach (Entity entity in targetEntities)
            {
                this.targetEntity = entity;
                destinationPosition = targetEntity.transform.position;

                switch (projectileType)
                {
                    case ProjectileType.Bazier:
                        float distance = Vector2.Distance(transform.position, destinationPosition);
                        float xDifference = destinationPosition.x - transform.position.x;

                        if (xDifference < 0)
                        {
                            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f);
                        }

                        float pointAXOffset = (UtilityFunctions.RandomFloat(pointAXOffsetDeviation.x, pointAXOffsetDeviation.y) + pointAXOffsetBase) * xDifference;
                        float pointAYOffset = (UtilityFunctions.RandomFloat(pointAYOffsetDeviation.x, pointAYOffsetDeviation.y) + pointAYOffsetBase) * distance;
                        pointA = new Vector2(pointAXOffset + transform.position.x, pointAYOffset + Mathf.Max(transform.position.y, destinationPosition.y));

                        float pointBXOffset = (UtilityFunctions.RandomFloat(pointBXOffsetDeviation.x, pointBXOffsetDeviation.y) + pointBXOffsetBase) * xDifference;
                        float pointBYOffset = (UtilityFunctions.RandomFloat(pointBYOffsetDeviation.x, pointBYOffsetDeviation.y) + pointBYOffsetBase) * distance;
                        pointB = new Vector2(pointBXOffset + pointA.x, pointBYOffset + pointA.y);

                        transferTime = distance / projectileSpeed;
                        currentPosition = transform.position;
                        break;

                    case ProjectileType.Straight:
                        switch (straightProjectileDirection)
                        {
                            case StraightProjectileDirection.Toward:
                                Quaternion? rotation = CheckProjectileRoute(transform.position, targetEntity.transform.position, checkProjectileRoute);

                                if (rotation.HasValue)
                                {
                                    transform.rotation = rotation.Value;
                                }
                                else
                                {
                                    fireProjectile = false;
                                }
                                break;

                            default:
                                break;
                        }
                        break;

                    case ProjectileType.Throw:
                        Vector2? projectileVelocity = CalculateProjectileVelocity(transform.position, destinationPosition, checkProjectileRoute);

                        if (projectileVelocity.HasValue)
                        {
                            rigidBody.velocity = projectileVelocity.Value;
                        }
                        else
                        {
                            fireProjectile = false;
                        }
                        break;

                    default:
                        break;
                }

                if (fireProjectile) break;
            }
        }

        if (fireProjectile)
        {
            projectileCoroutine = StartCoroutine(FireProjectileCoroutine());
        }
    }

    protected virtual void OnCollision(Collider2D collision)
    {
        isTouchedGround = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsGround);
        isTouchedTarget = UtilityFunctions.IsInLayerMask(collision.gameObject.layer, whatIsDamageable);

        if (combatAbility == null)
        {
            if (explosionDurationTime == 0.0f)
            {
                Explode();
            }
            else
            {
                Invoke("Explode", explosionDurationTime);
            }
        }
        else if (isTouchedTarget)
        {
            foreach (CombatAbilityComponent combatAbilityComponent in combatAbility.combatAbilityComponents)
            {
                switch (combatAbilityComponent)
                {
                    case DamageComponent damageComponent:
                        damageComponent.ApplyCombatAbility(collision, new OverlapCollider[1] { overlapCollider });
                        break;
                    case KnockbackComponent knockbackComponent:
                        knockbackComponent.ApplyCombatAbility(collision, new OverlapCollider[1] { overlapCollider });
                        break;
                    case StatusEffectComponent statusEffectComponent:
                        statusEffectComponent.ApplyCombatAbility(collision, new OverlapCollider[1] { overlapCollider });
                        break;
                }
            }

            ReleaseObject();
        }
    }

    protected void Explode()
    {
        GameObject projectileExplosion = Manager.Instance.objectPoolingManager.GetGameObject(gameObject.name + "Explosion");
        projectileExplosion.transform.position = transform.position;

        ReleaseObject();
    }

    public void ProjetileDeflected()
    {
        currentDeflection += 1;

        if (projectileTypeWhenParried == ProjectileType.None)
        {
            ReleaseObject();
        }
        else
        {
            projectileSpeed = initialProjectileSpeed;
            projectileType = projectileTypeWhenParried;
            straightProjectileDirection = StraightProjectileDirection.Toward;

            FireProjectile(targetEntity, new Entity[1] { sourceEntity }, false, null);
        }
    }

    public Vector2? CalculateProjectileVelocity(Vector2 projectileFirePosition, Vector2 targetPosition, bool checkProjectileRoute)
    {
        float distance = Vector2.Distance(projectileFirePosition, targetPosition);
        List<Tuple<Vector2, bool>> availableVelocities = new List<Tuple<Vector2, bool>>();
        float gravityAccelaration = Mathf.Abs(Physics2D.gravity.y * rigidBody.gravityScale);
        float xDifference = targetPosition.x - projectileFirePosition.x;
        float yDifference = targetPosition.y - projectileFirePosition.y;
        float currentSpeed = speedStepValue;

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
            Vector2 movementVector = new Vector2(projectileVelocity.x * timeElapsed, projectileVelocity.y * timeElapsed + 0.5f * Physics2D.gravity.y * rigidBody.gravityScale * Mathf.Pow(timeElapsed, 2));
            prevPosition = currentPosition;
            currentPosition = movementVector + projectileFirePosition;
            Vector2 directionVector = currentPosition - prevPosition;
            float directionAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

            if (overlapCollider.overlapBox)
            {
                if (Physics2D.BoxCast(prevPosition, overlapCollider.boxSize, directionAngle, directionVector, Vector2.Distance(currentPosition, prevPosition), whatIsGround))
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
            }
            else if (overlapCollider.overlapCircle)
            {
                if (Physics2D.CircleCast(prevPosition, overlapCollider.circleRadius, directionVector, Vector2.Distance(currentPosition, prevPosition), whatIsGround))
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
            }

            timeElapsed += timeStep;
        }

        return true;
    }

    private Quaternion? CheckProjectileRoute(Vector2 projectileFirePosition, Vector2 targetPosition, bool checkProjectileRoute)
    {
        Vector2 targetDirection = targetEntity.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        if (checkProjectileRoute)
        {
            if (Physics2D.Raycast(projectileFirePosition, targetDirection, Vector2.Distance(projectileFirePosition, targetPosition), whatIsGround))
            {
                return null;
            }
            else
            {
                return Quaternion.AngleAxis(targetAngle, Vector3.forward);
            }
        }
        else
        {
            return Quaternion.AngleAxis(targetAngle, Vector3.forward);
        }
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

    public ProjectileType GetProjectileType() => projectileType;
    public StraightProjectileDirection GetStraightProjectileDirection() => straightProjectileDirection;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (overlapCollider.overlapBox)
        {
            Gizmos.DrawWireCube(overlapCollider.centerTransform.position, overlapCollider.boxSize);
        }
        else if (overlapCollider.overlapCircle)
        {
            Gizmos.DrawWireSphere(overlapCollider.centerTransform.position, overlapCollider.circleRadius);
        }
    }
}
