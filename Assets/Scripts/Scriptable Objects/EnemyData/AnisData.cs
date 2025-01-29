using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NeonData", menuName = "Data/Enemy Data/Nikke/Anis")]
public class AnisData : NikkeData
{
    [Header("Combat Ability Information")]
    public float meleeAttack0CoolDown;
    public float meleeAttack1CoolDown;
    public float meleeAttack2CoolDown;
    public float meleeAttack3CoolDown;
    public float rangedAttackCoolDown;
    public float closeRangedAttackCoolDown;
}
