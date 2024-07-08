using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    private int jumpCount;

    public PlayerJumpState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        jumpCount += 1;
        player.movement.SetVelocityY(playerData.jumpSpeed);
        player.inAirState.InavailCoyoteTime();
        player.inAirState.SetVariableJumpHeightAvail();
        player.inputHandler.InactiveJumpInput();
        player.animator.SetFloat("yVelocity", currentVelocity.y);
        isAbilityDone = true;
    }

    public bool IsJumpAvail() => jumpCount < playerData.maxJumpCount;

    public void ResetJumpCount() => jumpCount = 0;

    public void IncreaseJumpCount() => jumpCount += 1;
}
