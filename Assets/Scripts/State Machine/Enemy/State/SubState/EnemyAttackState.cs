using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public Timer attackCoolDownTimer { get; private set; }
    public bool canAttack { get; private set; }

    public EnemyAttackState(Enemy enemy, string animBoolName, float coolDown) : base(enemy, animBoolName)
    {
        canAttack = true;
        attackCoolDownTimer = new Timer(coolDown);
        attackCoolDownTimer.timerAction += () => { canAttack = true; };
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
        enemy.combat.damagedTargets.Clear();
    }

    public override void Enter()
    {
        base.Enter();

        canAttack = false;
        enemy.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.combat.damagedTargets.Clear();
        attackCoolDownTimer.StartSingleUseTimer();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isAbilityDone)
            {
                stateMachine.ChangeState(enemy.targetInAggroRangeState);
            }
        }
    }
}
