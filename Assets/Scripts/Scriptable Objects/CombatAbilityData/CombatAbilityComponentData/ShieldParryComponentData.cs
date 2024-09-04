using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldParryComponentData : CombatAbilityComponentData
{
    public float parryTime;

    public override void ApplyCombatAbility(Collider2D target)
    {
        throw new System.NotImplementedException();
    }
}
