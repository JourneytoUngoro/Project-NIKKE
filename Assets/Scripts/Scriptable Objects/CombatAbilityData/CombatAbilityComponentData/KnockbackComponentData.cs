using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackComponentData : CombatAbilityComponentData
{
    public Vector2 knockbackDirection;
    public float knockbackSpeed;
    public float knockbackTime;

    public override void ApplyCombatAbility(Collider2D target)
    {
        // target.SendMessage
        throw new System.NotImplementedException();
    }
}
