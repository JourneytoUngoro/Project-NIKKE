using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionBase { Transform, Absolute, Rotation, Relative }

public class KnockbackComponent : CombatAbilityComponent
{
    [field: SerializeField] public bool entityProtruded { get; private set; }
    [field: SerializeField] public DirectionBase directionBase { get; private set; } = DirectionBase.Transform;
    [field: SerializeField] public Vector2 knockbackDirection { get; private set; }
    [field: SerializeField] public float knockbackSpeed { get; private set; }
    [field: SerializeField, Tooltip("KnockbackTime of 0 means that the entity will transit from knockbackState when it hits the ground.")] public float knockbackTime { get; private set; }
    [field: SerializeField] public Ease easeFunction { get; private set; }

    [field: SerializeField] public bool isKnockbackDifferentWhenAerial { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenAerial { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenAerial { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenAerial { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenAerial { get; private set; }



    [field: SerializeField] public bool canBeShielded { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenShielded { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenShielded { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenShielded { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenShielded { get; private set; }

    [field: SerializeField] public bool isKnockbackDifferentWhenAerialShielded { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenAerialShielded { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenAerialShielded { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenAerialShielded { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenAerialShielded { get; private set; }



    [field: SerializeField] public bool canBeParried { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenParried { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenParried { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenParried { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenParried { get; private set; }

    [field: SerializeField] public bool counterProtrudedWhenParried { get; private set; }
    [field: SerializeField] public Vector2 counterKnockbackDirectionWhenParried { get; private set; }
    [field: SerializeField] public float counterKnockbackSpeedWhenParried { get; private set; }
    [field: SerializeField] public float counterKnockbackTimeWhenParried { get; private set; }
    [field: SerializeField] public Ease counterEaseFunctionWhenParried { get; private set; }

    [field: SerializeField] public bool isKnockbackDifferentWhenAerialParried { get; private set; }
    [field: SerializeField] public Vector2 knockbackDirectionWhenAerialParried { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenAerialParried { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenAerialParried { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenAerialParried { get; private set; }

    [field: SerializeField] public bool counterProtrudedWhenAerialParried { get; private set; }
    [field: SerializeField] public Vector2 counterKnockbackDirectionWhenAerialParried { get; private set; }
    [field: SerializeField] public float counterKnockbackSpeedWhenAerialParried { get; private set; }
    [field: SerializeField] public float counterKnockbackTimeWhenAerialParried { get; private set; }
    [field: SerializeField] public Ease counterEaseFunctionWhenAerialParried { get; private set; }

    [HideInInspector] public Transform knockbackSourceTransform;

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity target = (variables[0] as Collider2D).GetComponentInParent<Entity>();
        OverlapCollider[] overlapColliders = (variables[1] as OverlapCollider[]);

        target.entityCombat.GetKnockback(this, overlapColliders);
    }
}
