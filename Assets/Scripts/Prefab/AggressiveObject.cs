using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveObject : PooledObject
{
    [SerializeField] protected EnemyAttackInfo enemyAttackInfo;

    public void SetAttackSubject(GameObject attackSubject)
    {
        enemyAttackInfo.attackSubject = attackSubject;
    }

    public void SetAttackInfo(EnemyAttackInfo enemyAttackInfo)
    {
        this.enemyAttackInfo = enemyAttackInfo;
    }
}
