using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashAttackState : PlayerAttackState
{
    public Timer dashCoolDownTimer;

    // private Timer dashAttackTimer;

    private bool dashAttackAvail;

    public PlayerDashAttackState(Player player, Transform attackTransform, string animBoolName) : base(player, attackTransform, animBoolName)
    {
        dashAttackAvail = true;
        /*dashAttackTimer = new Timer(playerData.dashAttackTime);
        dashAttackTimer.timerAction += () => { isAbilityDone = true; };*/
        dashCoolDownTimer = new Timer(playerData.dashAttackCoolDowmTime);
        dashCoolDownTimer.timerAction += () => { dashAttackAvail = true; };
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        dashCoolDownTimer.StartSingleUseTimer();
        dashAttackAvail = false;
        player.movement.SetVelocityY(playerData.dashAttackSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // dashAttackTimer.Tick();

        if (!onStateExit)
        {
            if (isGrounded || currentVelocity.y < epsilon)
            {
                isAbilityDone = true;
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public bool IsDashAttackAvail() => dashAttackAvail;
}
