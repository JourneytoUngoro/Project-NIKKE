using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NeonData", menuName = "Data/Enemy Data/Nikke/Neon")]
public class NeonData : NikkeData
{
    [Header("Combat Ability Information")]
    public float meleeAttackCoolDown;
    public float rangedAttackCoolDown;
    public float chargeAttackCoolDown;
}
