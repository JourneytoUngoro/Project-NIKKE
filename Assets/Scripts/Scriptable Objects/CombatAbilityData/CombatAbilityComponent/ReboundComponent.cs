using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReboundComponent : CombatAbilityComponent
{
    public Vector2 reboundDirection;
    public float reboundVelocity;
    public float reboundTime;
    public Ease reboundEaseFunction;

    public override void ApplyCombatAbility(Collider2D target)
    {
        throw new System.NotImplementedException();
    }
}
