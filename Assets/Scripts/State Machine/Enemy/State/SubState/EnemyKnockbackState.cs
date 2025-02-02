using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockbackState : EnemyState
{
    protected bool shouldTransitToStunnedState;

    protected bool canTransit;

    protected Vector2 knockbackVelocity;

    public Timer knockbackTimer { get; private set; }

    public EnemyKnockbackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        knockbackTimer = new Timer(0.0f);
        knockbackTimer.timerAction += () => { canTransit = true; };
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        canTransit = true;
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;
        enemy.rigidBody.gravityScale = 9.5f;
        enemy.movement.SetVelocityMultiplier(Vector2.one);
        knockbackTimer.StartSingleUseTimer();
        enemy.stateMachineToAnimator.state = this;
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
                else if (isTargetInAggroRange)
                {
                    stateMachine.ChangeState(enemy.targetInAggroRangeState);
                }
                else
                {
                    stateMachine.ChangeState(enemy.lookForTargetState);
                }
            }

            knockbackTimer.Tick();
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
