using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CombatAbilityComponent : ICombatAbility
{
    [SerializeField, HideInInspector] private string name = "";

    public Entity entity { get; set; }
    public CombatAbility combatAbility { get; set; }

    public CombatAbilityComponent()
    {
        name = GetType().Name;
    }

    public abstract void ApplyCombatAbility(params object[] variables);
}