using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAttackInfo : AttackInfo
{
    [HideInInspector] public GameObject attackSubject;
    
    public int attackLevel;
    
    public bool isProjectile;
    [HideInInspector] public GameObject projectileSender;
   
    public bool isDazedWhenAvoided;
    public float dazedTime;
    
    public float damageWhenShielded;
    public float postureDamageWhenShielded;
    public float knockbackTimeWhenShielded;
    public Vector2 knockbackVelocityWhenShielded;
    public float damageWhenParried;
    public float postureDamageWhenParried;
    public float counterPostureDamageWhenParried;
    public float knockbackTimeWhenParried;
    public Vector2 knockbackVelocityWhenParried;
    public float counterKnockbackTimeWhenParried;
    public Vector2 counterKnockbackVelocityWhenParried;

    public EnemyAttackInfo(GameObject sender, float damage, float poiseDamage)
    {
        this.attackSubject = sender;
        this.healthDamage = damage;
        this.postureDamage = poiseDamage;
    }
}
