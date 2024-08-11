using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockbackState : EnemyState
{
    protected bool shouldTransitToStunnedState;

    protected bool canTransit;

    protected Vector2 knockbackVelocity;

    protected float elapsedTime;

    public EnemyKnockbackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        canTransit = true;
    }

    public override void Enter()
    {
        base.Enter();

        elapsedTime = 0.0f;
        canTransit = false;
        enemy.stateMachineToAnimator.state = this;
        enemy.movement.SetVelocityXChangeOverTime(knockbackVelocity.x, 0.3f, Ease.InCubic, true);
    }

    public override void Exit()
    {
        base.Exit();

        shouldTransitToStunnedState = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (canTransit)
            {
                if (shouldTransitToStunnedState)
                {
                    stateMachine.ChangeState(enemy.stunnedState);
                }
                else
                {
                    stateMachine.ChangeState(enemy.targetInAggroRangeState);
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(false);
        }
    }

    public void ShouldTransitToStunnedState() => shouldTransitToStunnedState = true;
    public void SetKnockbackVelocity(Vector2 knockbackVelocity) => this.knockbackVelocity = knockbackVelocity;
}
