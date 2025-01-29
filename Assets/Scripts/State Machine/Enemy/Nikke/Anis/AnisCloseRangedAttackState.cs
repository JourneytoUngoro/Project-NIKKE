using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnisCloseRangedAttackState : EnemyAttackState
{
    public AnisCloseRangedAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
    }
}
