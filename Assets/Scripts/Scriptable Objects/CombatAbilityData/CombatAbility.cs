using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newCombatAbilityData", menuName = "Data/Combat Ability Data")]
public class CombatAbility : ScriptableObject
{
    public enum ThreatLevel { øÁ, êË, ß¯ }

    [field: SerializeField] public Sprite combatAbilityIcon { get; private set; }
    [field: SerializeField] public string combatAbilityName { get; private set; } = "Default Combat Ability Name";
    [field: SerializeField] public ThreatLevel threatLevel { get; private set; } = ThreatLevel.øÁ;
    [field: SerializeField, TextArea] public string combatAbilityDescription { get; private set; } = "Default Combat Ability Description";
    // TODO: Add stance logic
    [field: SerializeField] public bool stanceWhenHit { get; private set; }
    [field: SerializeField] public bool stanceWhenParried { get; private set; }
    // TODO: Add dazed logic
    [field: SerializeField] public bool getDazed { get; private set; }
    [field: SerializeReference] public List<CombatAbilityComponent> combatAbilityComponents { get; private set; }
    public Entity sourceEntity { get; set; }

    public void AddComponent(CombatAbilityComponent componentData)
    {
        if (componentData.GetType().Equals(typeof(ProjectileComponent)))
        {
            combatAbilityComponents.Add(componentData);
        }
        else if (combatAbilityComponents.FirstOrDefault(type => type.GetType().Equals(componentData.GetType())) == null)
        {
            combatAbilityComponents.Add(componentData);
        }
    }
}
