using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockParryState : PlayerAbilityState
{
    public bool isParrying { get; private set; }
    public Timer defendCoolDownTimer;

    private Timer parryTimer;

    private bool isBlockAvail;

    public PlayerBlockParryState(Player player, string animBoolName) : base(player, animBoolName)
    {
        isBlockAvail = true;
        defendCoolDownTimer = new Timer(playerData.defendCoolDownTime);
        defendCoolDownTimer.timerAction += () => { isBlockAvail = true; };
        parryTimer = new Timer(playerData.parryTime);
        parryTimer.timerAction += () => { isParrying = false; };
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        isParrying = true;
        isBlockAvail = false;
        parryTimer.StartSingleUseTimer();
        player.blockParryArea.SetActive(true);
        player.movement.SetVelocityX(0.0f);
    }

    public override void Exit()
    {
        base.Exit();

        player.blockParryArea.SetActive(false);
        defendCoolDownTimer.StartSingleUseTimer();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        parryTimer.Tick();

        if (!player.inputHandler.blockParryInput)
        {
            isAbilityDone = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public bool IsBlockAvail() => isBlockAvail;
}
