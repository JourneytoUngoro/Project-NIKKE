using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnisRangedAttackState : EnemyAttackState
{
    private Anis anis;

    public AnisRangedAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName, coolDown)
    {
        anis = enemy as Anis;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();


    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LateLogicUpdate()
    {
        base.LateLogicUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    protected override void TickPublicTimers()
    {
        base.TickPublicTimers();
    }
}
