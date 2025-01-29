using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    private int jumpCount;

    public PlayerJumpState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Manager.Instance.dialogueManager.SendDialogue("Default Dialogue");

        if (isGrounded && player.detection.currentPlatform != null && player.detection.currentPlatform.CompareTag("OneWayPlatform") && inputY == -1)
        {
            player.detection.currentPlatform.GetComponent<OneWayPlatform>().DisableCollision(player);
        }
        else
        {
            jumpCount += 1;
            player.movement.SetVelocityY(playerData.jumpSpeed);
            player.inAirState.InavailCoyoteTime();
            player.inAirState.SetVariableJumpHeightAvail();
        }

        player.inputHandler.InactiveJumpInput();
        player.animator.SetFloat("yVelocity", currentVelocity.y);
        isAbilityDone = true;
    }

    public bool IsJumpAvail() => jumpCount < playerData.maxJumpCount;

    public void ResetJumpCount() => jumpCount = 0;

    public void IncreaseJumpCount() => jumpCount += 1;
}
