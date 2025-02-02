using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonMoveState : EnemyMoveState
{
    private Neon neon;

    public NeonMoveState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        neon = enemy as Neon;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        neon.movement.SetVelocityXChangeOverTime(neon.neonData.moveSpeed * neon.movement.facingDirection, neon.neonData.moveTime, neon.neonData.moveEaseFunction, true, isDetectingLedgeFront);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void Enter()
    {
        base.Enter();

        neon.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        neon.movement.StopVelocityXChangeOverTime();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController();
        }
    }
}
