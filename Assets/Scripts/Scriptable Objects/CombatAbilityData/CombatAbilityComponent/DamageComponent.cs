using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageComponent : CombatAbilityComponent
{
    [field: SerializeField] public float baseHealthDamage { get; private set; }
    [field: SerializeField] public float basePostureDamage { get; private set; }
    [field: SerializeField] public float healthDamageIncreaseByLevel { get; private set; }
    [field: SerializeField] public float postureDamageIncreaseByLevel { get; private set; }
    [field: SerializeField] public float pauseTimeWhenHit { get; private set; }

    [field: SerializeField] public bool canBeShielded { get; private set; } = true;
    [field: SerializeField, Range(0.0f, 1.0f)] public float healthDamageShieldRate { get; private set; } = 1.0f;
    [field: SerializeField, Range(0.0f, 1.0f)] public float postureDamageShieldRate { get; private set; } = 0.5f;
    [field: SerializeField]  public float pauseTimeWhenShielded { get; private set; }

    [field: SerializeField] public bool canBeParried { get; private set; } = true;
    [field: SerializeField, Range(0.0f, 1.0f)] public float healthDamageParryRate { get; private set; } = 1.0f;
    [field: SerializeField, Range(0.0f, 1.0f)] public float postureDamageParryRate { get; private set; } = 0.7f;
    [field: SerializeField, Range(0.0f, 1.0f)] public float healthCounterDamageRate { get; private set; } = 0.0f;
    [field: SerializeField, Range(0.0f, 1.0f)] public float postureCounterDamageRate { get; private set; } = 0.8f;
    [field: SerializeField] public float pauseTimeWhenParried { get; private set; }

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity target = (variables[0] as Collider2D).GetComponentInParent<Entity>();

        target.SendMessage("GetDamage", this);
        // target.SendMessage("GetHealthDamage", this);
        // target.SendMessage("GetPostureDamage", this);
        // target.SendMessage("GetDamage", this);
    }
}
