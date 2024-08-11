using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttackState : PlayerAttackState
{
    private Timer rangedAttackTimer;
    public Timer attackComboResetTimer;

    private int attackStroke;

    public int currentAmmo { get; private set; }

    public PlayerMeleeAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        rangedAttackTimer = new Timer(playerData.rangedAttackPeriod);
        rangedAttackTimer.timerAction += RangedAttack;
        attackComboResetTimer = new Timer(playerData.attackStrokeResetTime);
        attackComboResetTimer.timerAction += () => { attackStroke = 0; player.animator.SetInteger("attackStroke", attackStroke); };
        currentAmmo = playerData.maxAmmo;
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        player.inputHandler.InactiveAttackInput();
        player.combat.DoMeleeAttack();
        attackComboResetTimer.StartSingleUseTimer();
        // player.animator.SetBool("connectToNextAttackStroke", false);
    }

    public override void AnimationActionTrigger(int index = 0)
    {
        base.AnimationActionTrigger(index);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        if (attackStroke == 0 || attackInputActive)
        {
            isAbilityDone = false;
            // player.animator.SetBool("connectToNextAttackStroke", true);
        }
        attackStroke = attackStroke % 4 + 1;
        player.animator.SetInteger("attackStroke", attackStroke);
        // player.inputHandler.InactiveAttackInput(); // 애니메이션이 얼어붙는 현상 방지
        /*if (attackStroke != 0)
        {
            player.animator.SetBool("meleeAttack", false);
        }*/
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.inputHandler.InactiveAttackInput();
        player.movement.SetVelocityX(0.0f);
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
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region Physics Logic
        if (!onStateExit)
        {
            player.movement.SetVelocityX(0.0f);

            if (isOnSlope)
            {
                player.rigidBody.gravityScale = 0.0f;
                player.movement.SetVelocityY(0.0f);
            }
            else
            {
                player.rigidBody.gravityScale = 9.5f;
            }
        }
        #endregion
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
