using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTeleportState : EnemyState
{
    public Timer teleportCoolDownTimer;

    protected Vector2 destinationPosition;
    public bool isTeleportAvail { get; protected set; }

    public EnemyTeleportState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        isTeleportAvail = true;
        teleportCoolDownTimer = new Timer(enemyData.teleportCoolDown);
        teleportCoolDownTimer.timerAction += () => { isTeleportAvail = true; };
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        isTeleportAvail = false;
        teleportCoolDownTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            enemy.movement.SetPosition(destinationPosition);
            stateMachine.ChangeState(enemy.targetInAggroRangeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void SetDestinationPosition(Vector2 destinationPosition) => this.destinationPosition = destinationPosition;
}
