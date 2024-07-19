using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttackState : EnemyState
{
    public Timer meleeAttackCoolDownTimer;
    public bool canMeleeAttack { get; private set; }

    protected bool isPlayerInMeleeAttackRange;
    protected bool isPlayerInRangedAttackRange;
    protected bool isPlayerInAggroRange;

    public EnemyMeleeAttackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        meleeAttackCoolDownTimer = new Timer(enemyData.meleeAttackCoolDown);
        meleeAttackCoolDownTimer.timerAction += () => { canMeleeAttack = true; };
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
        meleeAttackCoolDownTimer.StartSingleUseTimer();
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

        canMeleeAttack = false;
        enemy.stateMachineToAnimator.state = this;
        enemy.movement.SetVelocityX(0.0f);
    }

    public override void Exit()
    {
        base.Exit();
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
