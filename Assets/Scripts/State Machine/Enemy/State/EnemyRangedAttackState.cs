using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttackState : EnemyState
{
    public Timer rangedAttackCoolDownTimer;
    public bool canRangedAttack { get; private set; }

    protected bool isPlayerInMeleeAttackRange;
    protected bool isPlayerInRangedAttackRange;
    protected bool isPlayerInAggroRange;

    public EnemyRangedAttackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        rangedAttackCoolDownTimer = new Timer(enemyData.rangedAttackCoolDown);
        rangedAttackCoolDownTimer.timerAction += () => { canRangedAttack = true; };
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
        rangedAttackCoolDownTimer.StartSingleUseTimer();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMeleeAttackRange = enemy.detection.isPlayerInMeleeAttackRange();
        isPlayerInRangedAttackRange = enemy.detection.isPlayerInRangedAttackRange();
        isPlayerInAggroRange = enemy.detection.isPlayerInAggroRange();
    }

    public override void Enter()
    {
        base.Enter();

        canRangedAttack = false;
        enemy.stateMachineToAnimator.state = this;
        enemy.movement.SetVelocityX(0.0f);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.movement.SetVelocityX(0.0f);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isAbilityDone)
            {
                stateMachine.ChangeState(enemy.playerInAggroRangeState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            enemy.movement.SetVelocityZero();
        }
    }
}
