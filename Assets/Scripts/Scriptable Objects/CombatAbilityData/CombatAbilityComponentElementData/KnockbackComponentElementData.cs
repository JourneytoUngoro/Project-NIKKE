using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnockbackComponentElementData : CombatAbilityComponentElementData
{
    public Vector2 knockbackDirection;
    public float knockbackSpeed;
    public float knockbackTime;

    public Vector2 knockbackDirectionWhenShielded;
    public float knockbackSpeedWhenShielded;
    public float knockbackTimeWhenShielded;

    public Vector2 knockbackDirectionWhenParried;
    public float knockbackSpeedWhenParried;
    public float knockbackTimeWhenParried;
}
