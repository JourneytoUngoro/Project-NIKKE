using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDazedState : EnemyState
{
    private Timer dazedTimer;

    public EnemyDazedState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        
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

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}