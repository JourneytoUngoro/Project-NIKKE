using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldParryComponentData : CombatAbilityComponentData
{
    [SerializeField, Range(0.0f, 180.0f)] private float clockwiseShieldParryAngle;
    [SerializeField, Range(0.0f, 180.0f)] private float counterClockwiseShieldParryAngle;
}
