using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        isAnimationActionTriggered = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.movement.SetVelocityMultiplier(Vector2.one);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (enemy.got[(int)GotConditions.Hit])
            {
                stateMachine.ChangeState(enemy.lookForTargetState);
            }
            else if (isTargetInDetectionRange)
            {
                stateMachine.ChangeState(enemy.targetInDetectionRangeState);
            }
            else if (isDetectingLedgeFront)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
            else if (isDetectingWall)
            {
                enemy.movement.Flip();
            }
        }
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
