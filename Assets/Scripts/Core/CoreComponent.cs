using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreComponent : MonoBehaviour
{
    protected Player player;
    protected Enemy enemy;
    protected Entity entity;
    protected float epsilon = 0.001f;
    protected Vector2 workSpace;

    protected virtual void Awake()
    {
        player = GetComponentInParent<Player>();
        enemy = GetComponentInParent<Enemy>();
        entity = GetComponentInParent<Entity>();
    }
}