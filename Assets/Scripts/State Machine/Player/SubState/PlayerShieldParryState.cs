using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldParryState : PlayerAbilityState
{
    public bool isParrying { get; private set; }
    public bool isParryAvail { get; private set; }
    
    public Timer shieldCoolDownTimer;

    private Timer parryTimer;

    private bool canTransit;

    public PlayerShieldParryState(Player player, string animBoolName) : base(player, animBoolName)
    {
        isParryAvail = true;
        shieldCoolDownTimer = new Timer(playerData.shieldParryCoolDownTime);
        shieldCoolDownTimer.timerAction += () => { isParryAvail = true; };
        parryTimer = new Timer(playerData.parryTime);
        parryTimer.timerAction += () => { isParrying = false; };
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        canTransit = false;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        canTransit = true;

        if (!shieldParryInput)
        {
            isAbilityDone = true;
        }
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        shieldParryInputActive = false;
        isParrying = true;
        isParryAvail = false;
        canTransit = true;
        parryTimer.StartSingleUseTimer();
        player.movement.SetVelocityX(0.0f);
    }

    public override void Exit()
    {
        base.Exit();

        isParrying = false;
        shieldCoolDownTimer.StartSingleUseTimer();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        parryTimer.Tick();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            /*if (!isAbilityDone)
            {
                if (isParryAvail)
                {
                    if (shieldParryInputActive)
                    {
                        stateMachine.ChangeState(player.shieldParryState);
                    }
                }
                else
                {
                    if (!shieldParryInput)
                    {
                        isAbilityDone = true;
                    }
                }
            }*/

            if (!shieldParryInput)
            {
                if (canTransit)
                {
                    isAbilityDone = true;
                }
            }
        }

        if (!onStateExit)
        {
            player.movement.RigidBodyController(false);
        }
    }

    public void GotHit()
    {
        player.animator.SetTrigger("gotHit");
    }
}
