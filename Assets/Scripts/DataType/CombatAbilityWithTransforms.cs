using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAbilityWithTransforms
{
    // [HideInInspector] public string name = "";
    [field: SerializeField] public bool visualize { get; private set; } = true;
    [field: SerializeField] public OverlapCollider[] overlapColliders { get; private set; }
    [field: SerializeField] public CombatAbility combatAbilityData { get; private set; }
}
