using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newNikkeData", menuName = "Data/Enemy Data")]
public class NeonData : EnemyData
{
    [Header("Mid Range Attack State")]
    public float midRAttackRecoilSpeed = 20.0f;
    public float midRAttackKRecoilTime = 0.2f;

    [Header("Ranged Attack State")]
    public float rangedAttackChargeTime;
    public float rangedAttackChargeSpeed;

    [Header("Attack Information")]
    public float midrAttackCoolDown;
    public EnemyAttackInfo midrAttackInfo;
}
