using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonCombat : EnemyCombat
{
    [field: SerializeField] public List<CombatAbilityWithTransforms> meleeAttack { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> rangedAttack { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> chargeAttack { get; private set; }
    [field: SerializeField] public CombatAbilityWithTransforms shieldArea { get; private set; }
}
