using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : CoreComponent
{
    public event Action synchronizeValues;

    private Rigidbody2D rigidBody;
    private Vector2 velocityMultiplier = Vector2.one;
    private Vector2 baseVelocity = Vector2.zero;

    private Coroutine velocityChangeOverTimeCoroutine;

    private bool isOnSlope;
    private bool isGrounded;

    public int facingDirection { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        rigidBody = GetComponentInParent<Rigidbody2D>();
    }

    private void Start()
    {
        facingDirection = entity.transform.rotation.y == 0 ? 1 : -1;
    }

    private void FixedUpdate()
    {
        isOnSlope = entity.entityDetection.isOnSlope();
        isGrounded = entity.entityDetection.isGrounded();
    }

    public void SetVelocityX(float velocity, bool considerGroundCondition = false)
    {
        if (considerGroundCondition)
        {
            if (isOnSlope && isGrounded)
            {
                SetVelocity(entity.entityDetection.slopePerpNormal * velocity);
            }
            else
            {
                if (isGrounded)
                {
                    SetVelocityLimitY(0.0f);
                }

                SetVelocityX(velocity);
            }
        }
        else
        {
            // workSpace.Set(velocity, rigidBody.velocity.y);
            SetWorkSpace(velocity, rigidBody.velocity.y);
            rigidBody.velocity = workSpace;
            // player.playerStateMachine.currentState.SetMovementVariables();
        }
        synchronizeValues?.Invoke();
    }

    public void SetVelocityY(float velocity)
    {
        // workSpace.Set(rigidBody.velocity.x, velocity);
        SetWorkSpace(rigidBody.velocity.x, velocity);
        rigidBody.velocity = workSpace;
        synchronizeValues?.Invoke();
        // player.playerStateMachine.currentState.SetMovementVariables();
    }

    public void SetVelocityLimitY(float velocity)
    {
        if (rigidBody.velocity.y > velocity)
        {
            SetWorkSpace(rigidBody.velocity.x, velocity);
            rigidBody.velocity = workSpace;
            synchronizeValues?.Invoke();
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        SetWorkSpace(velocity.x, velocity.y);
        rigidBody.velocity = workSpace;
        synchronizeValues?.Invoke();
    }

    public void AddVelocityX(float velocity)
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x + velocity, rigidBody.velocity.y);
        synchronizeValues?.Invoke();
    }

    public void AddVelocityXChangeByTime(float velocity, float moveTime, Ease easeFunction)
    {

    }

    public void MultiplyVelocity(float multiplier)
    {
        rigidBody.velocity = multiplier * rigidBody.velocity;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityWithDirection(Vector2 angleVector, int direction, float speed)
    {
        workSpace.Set(angleVector.x * direction, angleVector.y);
        rigidBody.velocity = workSpace.normalized * speed;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityZero()
    {
        workSpace = Vector2.zero;
        rigidBody.velocity = workSpace;
        synchronizeValues?.Invoke();
    }

    public void SetPositionX(float xPos)
    {
        workSpace.Set(xPos, transform.position.y);
        entity.transform.position = workSpace;
    }

    public void SetPositionY(float yPos)
    {
        workSpace.Set(transform.position.x, yPos);
        entity.transform.position = workSpace;
    }

    public void SetPosition(Vector2 position)
    {
        entity.transform.position = position;
    }

    public void MovePosition(Vector2 angleVector, float direction, float distance)
    {
        workSpace.Set(angleVector.x * direction, angleVector.y);
        transform.position += (Vector3)workSpace.normalized * distance;
    }

    public void CheckIfShouldFlip(float velocityX)
    {
        if (Mathf.Abs(velocityX) > 0.001f && facingDirection * velocityX > 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        facingDirection *= -1;
        entity.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        synchronizeValues?.Invoke();
    }

    public void ChangeBaseVelocity(Vector2 baseVelocity)
    {
        this.baseVelocity += baseVelocity;
    }

    public void SetBaseVelocity(Vector2 baseVelocity)
    {
        this.baseVelocity = baseVelocity;
    }

    public void MultiplyVelocityMultiplier(Vector2 velocityMultiplier)
    {
        workSpace.Set(this.velocityMultiplier.x * velocityMultiplier.x, this.velocityMultiplier.y * velocityMultiplier.y);
        this.velocityMultiplier = workSpace;
    }

    public void SetVelocityMultiplier(Vector2 velocityMultiplier)
    {
        this.velocityMultiplier = velocityMultiplier;
    }

    public void RigidBodyController(bool isMovingForward = true, bool limitYVelocity = true)
    {
        if (isGrounded)
        {
            if (isOnSlope)
            {
                entity.rigidBody.gravityScale = rigidBody.velocity.magnitude > epsilon ? 9.5f : 0.0f;

                if (isMovingForward)
                {
                    if (entity.entityDetection.slopePerpNormal.y * facingDirection > 0)
                    {
                        SetVelocityMultiplier(Vector2.one * 0.8f);
                    }
                    else
                    {
                        SetVelocityMultiplier(Vector2.one * 1.4f);
                    }
                }
                else
                {
                    if (entity.entityDetection.slopePerpNormal.y * -facingDirection > 0)
                    {
                        SetVelocityMultiplier(Vector2.one * 0.8f);
                    }
                    else
                    {
                        SetVelocityMultiplier(Vector2.one * 1.4f);
                    }
                }
            }
            else
            {
                SetVelocityMultiplier(Vector2.one);
                entity.rigidBody.gravityScale = 9.5f;

                if (limitYVelocity)
                {
                    SetVelocityLimitY(0.0f);
                }
            }
        }
        else
        {
            if (entity.entityDetection.isOnSlopeBack())
            {
                if (limitYVelocity)
                {
                    SetVelocityLimitY(0.0f);
                }
            }

            SetVelocityMultiplier(Vector2.one);
            entity.rigidBody.gravityScale = 9.5f;
        }
    }

    public void SetVelocityXChangeOverTime(float velocity, float moveTime, Ease easeFunction, bool slowDown)
    {
        if (velocityChangeOverTimeCoroutine != null)
        {
            StopCoroutine(velocityChangeOverTimeCoroutine);
        }
        velocityChangeOverTimeCoroutine = StartCoroutine(VelocityChangeOverTime(velocity, moveTime, easeFunction, slowDown));
    }

    public void StopVelocityXChangeOverTime()
    {
        StopCoroutine(velocityChangeOverTimeCoroutine);
    }

    private void SetWorkSpace(float x, float y)
    {
        workSpace.Set(x, y);
        workSpace += baseVelocity;
        workSpace *= velocityMultiplier;
    }

    private IEnumerator VelocityChangeOverTime(float velocity, float moveTime, Ease easeFunction, bool slowDown)
    {
        float coroutineElapsedTime = 0.0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            if (easeFunction == Ease.Unset)
            {
                float velocityMultiplierOverTime = 1.0f;
                SetVelocityX(velocityMultiplierOverTime * velocity, true);
            }
            else
            {
                float velocityMultiplierOverTime = slowDown ? Mathf.Clamp(1.0f - DOVirtual.EasedValue(0.0f, 1.0f, coroutineElapsedTime / moveTime, easeFunction), 0.0f, 1.0f) : Mathf.Clamp(DOVirtual.EasedValue(0.0f, 1.0f, coroutineElapsedTime / moveTime, easeFunction), 0.0f, 1.0f);
                SetVelocityX(velocityMultiplierOverTime * velocity, true);
            }


            if (coroutineElapsedTime > moveTime)
            {
                if (easeFunction == Ease.Unset)
                {
                    SetVelocityX(0.0f, true);
                }
                yield break;
            }
            else
            {
                yield return waitForFixedUpdate;
            }

            coroutineElapsedTime += Time.deltaTime;
        }
    }
}
