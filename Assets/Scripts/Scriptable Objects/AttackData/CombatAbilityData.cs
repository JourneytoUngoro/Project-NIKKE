using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttackData", menuName = "Data/Attack Data")]
public class CombatAbilityData : ScriptableObject
{
    [field: SerializeField] public Sprite combatAbilityIcon { get; private set; }
    [field: SerializeField] public string combatAbilityName { get; private set; }
    [field: SerializeField] public int numberOfStrokes { get; private set; }
    [field: SerializeReference] public List<CombatAbilityComponentData> combatAbilityComponents { get; private set; }

    public void AddComponent(CombatAbilityComponentData componentData)
    {
        if (combatAbilityComponents.FirstOrDefault(type => type.GetType().Equals(componentData.GetType())) == null)
        {
            combatAbilityComponents.Add(componentData);
        }
    }
}
