using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReboundComponent : CombatAbilityComponent
{
    // TODO: Add Timer logic when rebounded
    [field: SerializeField] public Vector2 onGroundReboundDirection { get; private set; }
    [field: SerializeField] public float onGroundReboundVelocity { get; private set; }
    [field: SerializeField] public float onGroundReboundTime { get; private set; }
    [field: SerializeField] public Ease onGroundReboundEaseFunction { get; private set; }
    [field: Space(15f)]
    [field: SerializeField] public Vector2 inAirReboundDirection { get; private set; }
    [field: SerializeField] public float inAirReboundVelocity { get; private set; }
    [field: SerializeField] public float inAirReboundTime { get; private set; }
    [field: SerializeField] public Ease inAirReboundEaseFunction { get; private set; }

    public override void ApplyCombatAbility(params object[] variables)
    {
        if (pertainedCombatAbility.sourceEntity.entityDetection.isGrounded())
        {
            pertainedCombatAbility.sourceEntity.entityCombat.GetRebound(onGroundReboundDirection, onGroundReboundVelocity, onGroundReboundTime, onGroundReboundEaseFunction);
        }
        else
        {
            pertainedCombatAbility.sourceEntity.entityCombat.GetRebound(inAirReboundDirection, inAirReboundVelocity, inAirReboundTime, inAirReboundEaseFunction);
        }
    }
}
