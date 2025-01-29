using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionBase { Transform, Absolute, Rotation }

public class KnockbackComponent : CombatAbilityComponent
{
    [field: SerializeField] public DirectionBase directionBase { get; private set; } = DirectionBase.Transform;
    [field: SerializeField] public int knockbackLevel { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirection { get; private set; }
    [field: SerializeField] public float knockbackSpeed { get; private set; }
    [field: SerializeField] public float knockbackTime { get; private set; }
    [field: SerializeField] public Ease easeFunction { get; private set; }
    [field: SerializeField] public bool isKnockbackDifferentInAir { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionInAir { get; private set; }
    [field: SerializeField] public float knockbackSpeedInAir { get; private set; }
    [field: SerializeField] public float knockbackTimeInAir { get; private set; }
    [field: SerializeField] public Ease easeFunctionInAir { get; private set; }

    [field: SerializeField] public bool canBeShielded { get; private set; }
    [field: SerializeField] public int knockbackLevelWhenShielded { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenShielded { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenShielded { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenShielded { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenShielded { get; private set; }
    [field: SerializeField] public bool isShieldedKnockbackDifferentInAir { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenShieldedInAir { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenShieldedInAir { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenShieldedInAir { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenShieldedInAir { get; private set; }

    [field: SerializeField] public bool canBeParried { get; private set; }
    [field: SerializeField] public int knockbackLevelWhenParried { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenParried { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenParried { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenParried { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenParried { get; private set; }
    [field: SerializeField] public Vector2 counterKnockbackDirectionWhenParried { get; private set; }
    [field: SerializeField] public float counterKnockbackSpeedWhenParried { get; private set; }
    [field: SerializeField] public float counterKnockbackTimeWhenParried { get; private set; }
    [field: SerializeField] public Ease counterEaseFunctionWhenParried { get; private set; }
    [field: SerializeField] public bool isParriedKnockbackDifferentInAir { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenParriedInAir { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenParriedInAir { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenParriedInAir { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenParriedInAir { get; private set; }
    [field: SerializeField] public Vector2 counterKnockbackDirectionWhenParriedInAir { get; private set; }
    [field: SerializeField] public float counterKnockbackSpeedWhenParriedInAir { get; private set; }
    [field: SerializeField] public float counterKnockbackTimeWhenParriedInAir { get; private set; }
    [field: SerializeField] public Ease counterEaseFunctionWhenParriedInAir { get; private set; }
    [HideInInspector] public Transform knockbackSourceTransform;

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity target = (variables[0] as Collider2D).GetComponentInParent<Entity>();
        OverlapCollider[] overlapColliders = (variables[1] as OverlapCollider[]);

        target.entityCombat.GetKnockback(this, overlapColliders);
    }
}
