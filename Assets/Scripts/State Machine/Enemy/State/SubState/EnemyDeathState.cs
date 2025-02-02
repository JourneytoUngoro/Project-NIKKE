using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        enemy.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        base.Enter();

        if (isGrounded)
        {
            enemy.movement.SetVelocity(Vector2.zero);
        }
    }
}
