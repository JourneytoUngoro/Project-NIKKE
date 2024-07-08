using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalAttackState : PlayerAttackState
{
    private Timer rangedAttackTimer;

    public int currentAmmo { get; private set; }

    public PlayerNormalAttackState(Player player, Transform attackTransform, string animBoolName) : base(player, attackTransform, animBoolName)
    {
        rangedAttackTimer = new Timer(playerData.rangedAttackCoolDownTime);
        rangedAttackTimer.timerAction += RangedAttack;
        currentAmmo = playerData.maxAmmo;
    }

    public override void AnimationStartTrigger()
    {
        base.AnimationStartTrigger();
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        rangedAttackTimer.StartMultiUseTimer();
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
            rangedAttackTimer.Tick();

            player.movement.SetVelocityX(0.0f);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void RangedAttack()
    {
        if (currentAmmo > 0)
        {
            player.combat.DoRangedAttack();
        }
    }

    public void DecreaseAmmo() => currentAmmo -= 1;

    public void ReloadAmmo() => currentAmmo = playerData.maxAmmo;
}
