using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackComponent : CombatAbilityComponent
{
    [field: SerializeField] public int knockbackLevel { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirection { get; private set; }
    [field: SerializeField] public float knockbackSpeed { get; private set; }
    [field: SerializeField] public float knockbackTime { get; private set; }
    [field: SerializeField] public Ease easeFunction { get; private set; }

    [field: SerializeField] public bool canBeShielded { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenShielded { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenShielded { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenShielded { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenShielded { get; private set; }

    [field: SerializeField] public bool canBeParried { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenParried { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenParried { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenParried { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenParried { get; private set; }
    [field: SerializeField] public Vector2 counterKnockbackDirectionWhenParried { get; private set; }
    [field: SerializeField] public float counterKnockbackSpeedWhenParried { get; private set; }
    [field: SerializeField] public float counterKnockbackTimeWhenParried { get; private set; }
    [field: SerializeField] public Ease counterEaseFunctionWhenParried { get; private set; }

    /* public override void ApplyCombatAbility(Collider2D target)
     {
         // target.SendMessage("GetKnockback", this);
         target.GetComponentInChildren<Combat>().GetKnockback(this);
     }*/

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity target = (variables[0] as Collider2D).GetComponentInParent<Entity>();

        target.SendMessage("GetKnockback", this);
    }
}
