using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageComponentData : CombatAbilityComponentData
{
    public float healthDamage;
    public float postureDamage;

    public float pauseTimeWhenHit;

    public override void ApplyCombatAbility(Collider2D target)
    {
        target.SendMessage("GetHealthDamage", healthDamage);
        target.SendMessage("GetPostureDamage", postureDamage);
        // throw new System.NotImplementedException();
    }
}
