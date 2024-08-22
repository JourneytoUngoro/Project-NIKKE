using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DamageComponentElementData : CombatAbilityComponentElementData
{
    public float healthDamage;
    public float postureDamage;
    
    public float pauseTimeWhenHit;

    public bool followUpAttack = false;
    public float followUpAttackTime;
    public float followUpAttackHealthDamage;
    public float followUpAttackPostureDamage;
}
