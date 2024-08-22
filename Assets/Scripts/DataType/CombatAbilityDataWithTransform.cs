using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAbilityDataWithTransform
{
    [HideInInspector] public string name;
    [SerializeField] private Transform centerTransform;
    [SerializeField] private OverlapCollider overlapCollider;
    [SerializeField] private CombatAbilityData combatAbilityData;

    public Transform CenterTransform => centerTransform;
    public CombatAbilityData CombatAbilityData => combatAbilityData;
}
