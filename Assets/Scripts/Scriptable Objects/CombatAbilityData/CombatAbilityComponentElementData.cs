using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAbilityComponentElementData
{
    [SerializeField, HideInInspector] private string name;

    public void SetElementName(int index) => name = $"Stroke {index}";
}
