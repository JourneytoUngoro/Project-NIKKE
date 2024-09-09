using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageComponent : CombatAbilityComponent
{
    public float baseHealthDamage;
    public float basePostureDamage;
    public float healthDamageIncreaseByLevel;
    public float postureDamageIncreaseByLevel;
    public float pauseTimeWhenHit;

    public bool canBeShielded = true;
    [Range(0.0f, 1.0f)] public float healthDamageShieldRate = 1.0f;
    [Range(0.0f, 1.0f)] public float postureDamageShieldRate = 0.5f;
    public float pauseTimeWhenShielded;

    public bool canBeParried = true;
    [Range(0.0f, 1.0f)] public float healthDamageParryRate = 1.0f;
    [Range(0.0f, 1.0f)] public float postureDamageParryRate = 0.7f;
    [Range(0.0f, 1.0f)] public float healthCounterDamageRate = 0.0f;
    [Range(0.0f, 1.0f)] public float postureCounterDamageRate = 0.8f;
    public float pauseTimeWhenParried;

    public override void ApplyCombatAbility(Collider2D target)
    {
        // target.SendMessage("GetHealthDamage", this);
        // target.SendMessage("GetPostureDamage", this);
        // target.SendMessage("GetDamage", this);
        target.GetComponentInChildren<Combat>().GetDamage(this);
    }
}
