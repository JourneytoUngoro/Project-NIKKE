using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : CoreComponent
{
    public event Action synchronizeValues;

    private Rigidbody2D rigidBody;
    private Vector2 workSpace;
    private Vector2 velocityMultiplier = Vector2.one;
    private Vector2 baseVelocity = Vector2.zero;

    public int facingDirection { get; private set; }

    private void Awake()
    {
        rigidBody = GetComponentInParent<Rigidbody2D>();
    }

    private void Start()
    {
        facingDirection = 1;
    }

    public void SetVelocityX(float velocity)
    {
        // workSpace.Set(velocity, rigidBody.velocity.y);
        SetWorkSpace(velocity, rigidBody.velocity.y);
        rigidBody.velocity = workSpace;
        synchronizeValues?.Invoke();
        // player.playerStateMachine.currentState.SetMovementVariables();
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

    public void SetVelocity(Vector2 angleVector, float speed)
    {
        // workSpace.Set(angleVector.x * facingDirection, angleVector.y);
        SetWorkSpace(angleVector.x * facingDirection, angleVector.y);
        rigidBody.velocity = workSpace.normalized * speed;
        synchronizeValues?.Invoke();
        // player.playerStateMachine.currentState.SetMovementVariables();
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
    }

    public void MultiplyVelocity(float multiplier)
    {
        rigidBody.velocity = multiplier * rigidBody.velocity;
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
        // player.playerStateMachine.currentState.SetMovementVariables();
    }

    public void SetPositionX(float xPos)
    {
        workSpace.Set(xPos, transform.position.y);
        transform.parent.position = workSpace;
    }

    public void SetPositionY(float yPos)
    {
        workSpace.Set(transform.position.x, yPos);
        transform.parent.position = workSpace;
    }

    public void SetPosition(Vector2 position)
    {
        transform.parent.position = position;
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
        transform.parent.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
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

    private void SetWorkSpace(float x, float y)
    {
        workSpace.Set(x, y);
        workSpace += baseVelocity;
        workSpace *= velocityMultiplier;
    }
}
