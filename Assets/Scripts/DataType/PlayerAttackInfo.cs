using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttackInfo : AttackInfo
{
    public bool canParry;
    public float parryRadius;
    public Vector2 parrySize;
    public float damageWhenDazed;
    public float postureDamageWhenDazed;
    public float damageWhenStunned;
    public float postureDamageWhenStunned;
}
