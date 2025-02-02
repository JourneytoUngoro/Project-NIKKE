using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnisMoveState : EnemyMoveState
{
    private Anis anis;

    public AnisMoveState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        anis = enemy as Anis;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        anis.movement.SetVelocityX(0.0f);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            anis.movement.SetVelocityX(anis.anisData.moveSpeed * anis.movement.facingDirection, true);

            RigidBodyController();
        }
    }
}
