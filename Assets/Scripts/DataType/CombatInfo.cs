using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatInfo
{
    [HideInInspector] public GameObject sender;
    public float damage;
    public float poiseDamage;
    public float blockMultiplier = 0.1f;

    public CombatInfo(GameObject sender, float damage, float poiseDamage)
    {
        this.sender = sender;
        this.damage = damage;
        this.poiseDamage = poiseDamage;
    }
}
