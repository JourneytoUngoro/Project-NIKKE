using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAbilityWithTransform
{
    [HideInInspector] public string name;
    public Transform centerTransform;
    public OverlapCollider overlapCollider;
    public CombatAbility combatAbilityData;
}
