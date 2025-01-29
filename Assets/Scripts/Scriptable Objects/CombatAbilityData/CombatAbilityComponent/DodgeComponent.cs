using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeComponent : CombatAbilityComponent
{
    public override void ApplyCombatAbility(params object[] variables)
    {
        pertainedCombatAbility.sourceEntity.gameObject.layer = LayerMask.NameToLayer("DodgeLayer");
    }
}
