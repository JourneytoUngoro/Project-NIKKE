using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Base Components
    public Animator animator { get; protected set; }
    public Rigidbody2D rigidBody { get; protected set; }
    public SpriteRenderer spriteRenderer { get; protected set; }
    public Collider2D entityCollider { get; protected set; }
    public Core core { get; protected set; }
    public StateMachineToAnimator stateMachineToAnimator { get; protected set; }
    #endregion

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        stateMachineToAnimator = GetComponent<StateMachineToAnimator>();

        core = GetComponentInChildren<Core>();
    }
}
