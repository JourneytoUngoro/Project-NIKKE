using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldParryComponentData : CombatAbilityComponentData
{
    public OverlapCollider shieldParryCollider;
    [SerializeField, Range(0.0f, 180.0f)] private float clockwiseShieldParryAngle;
    [SerializeField, Range(0.0f, 180.0f)] private float counterClockwiseShieldParryAngle;
    public float parryTime;

    public override void CombatAbility()
    {
        throw new System.NotImplementedException();
    }
}
