using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReboundComponent : CombatAbilityComponent
{
    [Header("On Ground Rebound")]
    public Vector2 onGroundReboundDirection;
    public float onGroundReboundVelocity;
    public float onGroundReboundTime;
    public Ease onGroundReboundEaseFunction;

    [Header("In Air Rebound")]
    public Vector2 inAirReboundDirection;
    public float inAirReboundVelocity;
    public float inAirReboundTime;
    public Ease inAirReboundEaseFunction;

    public override void ApplyCombatAbility(params object[] variables)
    {
        throw new System.NotImplementedException();
    }
}
