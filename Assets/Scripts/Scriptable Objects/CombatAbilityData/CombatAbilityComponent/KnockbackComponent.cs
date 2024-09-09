using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackComponent : CombatAbilityComponent
{
    public int knockbackLevel;
    public Vector2 knockbackDirection;
    public float knockbackSpeed;
    public float knockbackTime;
    public Ease easeFunction;

    public bool canBeShielded;
    public Vector2 knockbackDirectionWhenShielded;
    public float knockbackSpeedWhenShielded;
    public float knockbackTimeWhenShielded;
    public Ease easeFunctionWhenShielded;

    public bool canBeParried;
    public Vector2 knockbackDirectionWhenParried;
    public float knockbackSpeedWhenParried;
    public float knockbackTimeWhenParried;
    public Ease easeFunctionWhenParried;
    public Vector2 counterKnockbackDirectionWhenParried;
    public float counterKnockbackSpeedWhenParried;
    public float counterKnockbackTimeWhenParried;
    public Ease counterEaseFunctionWhenParried;

    public override void ApplyCombatAbility(Collider2D target)
    {
        // target.SendMessage("GetKnockback", this);
        target.GetComponentInChildren<Combat>().GetKnockback(this);
    }
}
