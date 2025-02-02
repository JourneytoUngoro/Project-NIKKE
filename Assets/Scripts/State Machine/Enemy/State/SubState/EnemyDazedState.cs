using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyDazedState : EnemyState
{
    private Timer dazedTimer;

    private bool canTransit;

    public EnemyDazedState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        dazedTimer = new Timer(enemyData.dazedTime);
        dazedTimer.timerAction += () => { canTransit = false; };
    }

    public override void Enter()
    {
        base.Enter();

        dazedTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        canTransit = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            dazedTimer.Tick();

            if (canTransit)
            {
                if (isTargetInDetectionRange)
                {
                    stateMachine.ChangeState(enemy.targetInAggroRangeState);
                }
                else if (enemy.got[(int)GotConditions.Hit] || canTransit)
                {
                    stateMachine.ChangeState(enemy.lookForTargetState);
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            RigidBodyController(true);

            if (isGrounded)
            {
                enemy.movement.SetVelocityX(0.0f);

                if (isOnSlope)
                {
                    enemy.movement.SetVelocityY(0.0f);
                }
            }
        }
    }
}
