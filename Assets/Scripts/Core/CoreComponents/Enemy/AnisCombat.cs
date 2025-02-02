using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnisCombat : EnemyCombat
{
    [field: SerializeField] public List<CombatAbilityWithTransforms> meleeAttack0 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> meleeAttack1 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> meleeAttack2 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> meleeAttack3 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> closeRangedAttack { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> rangedAttack { get; private set; }
}
