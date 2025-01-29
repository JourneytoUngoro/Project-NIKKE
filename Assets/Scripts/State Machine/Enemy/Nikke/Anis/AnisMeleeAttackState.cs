using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnisMeleeAttackState : EnemyAttackState
{
    public AnisMeleeAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
    }
}
