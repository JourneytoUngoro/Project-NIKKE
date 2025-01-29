using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GotConditions { Hit, HealthDamage, PostureDamage, Knockback, Parried, Shielded, wasParried }

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

    #region Entity Components
    public Movement entityMovement { get; protected set; }
    public Detection entityDetection { get; protected set; }
    public Combat entityCombat { get; protected set; }
    public Stats entityStats { get; protected set; }
    #endregion

    #region Other Variables
    public int entityLevel { get; protected set; }
    public bool isDead { get; protected set; }
    public bool[] got { get; protected set; }
    #endregion

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        stateMachineToAnimator = GetComponent<StateMachineToAnimator>();

        core = GetComponentInChildren<Core>();
        got = new bool[Enum.GetValues(typeof(GotConditions)).Length];
    }

    protected virtual void Start()
    {
        entityDetection = core.GetCoreComponent<Detection>();
        entityMovement = core.GetCoreComponent<Movement>();
        entityCombat = core.GetCoreComponent<Combat>();
        entityStats = core.GetCoreComponent<Stats>();
    }

    protected virtual void LateUpdate()
    {
        Array.Fill(got, false);
    }

    public void UseAfterImage(Color color)
    {
        GameObject afterImage = Manager.Instance.objectPoolingManager.GetGameObject("AfterImage");
        afterImage.GetComponent<AfterImage>().spriteRenderer.sprite = spriteRenderer.sprite;
        afterImage.GetComponent<AfterImage>().spriteRenderer.color = color;
        afterImage.transform.position = transform.position;
        afterImage.transform.rotation = transform.rotation;
    }
}
