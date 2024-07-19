using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAttackInfo : AttackInfo
{
    [HideInInspector] public GameObject attackSubject;
    public Vector3 initialAttackPoint;
    
    public int attackLevel;
    
    public bool isProjectile;
    public bool dazedWhenAvoided;
    
    public float damageWhenShielded;
    public float postureDamageWhenShielded;
    public float damageWhenParried;
    public float postureDamageWhenParried;
    public float counterPostureDamageWhenParried;
    public float xVelocityWhenHit;
    public float yVelocityWhenHit;
    public float dazedTime;

    public EnemyAttackInfo(GameObject sender, float damage, float poiseDamage)
    {
        this.attackSubject = sender;
        this.damage = damage;
        this.postureDamage = poiseDamage;
    }
}
