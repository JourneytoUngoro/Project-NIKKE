using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWasParriedState : EnemyState
{
    private Timer knockbackTimer;
    private bool canTransit;

    public EnemyWasParriedState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);
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

        knockbackTimer.Tick();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
