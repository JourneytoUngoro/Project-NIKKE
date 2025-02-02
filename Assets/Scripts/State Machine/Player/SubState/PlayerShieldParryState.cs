using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerShieldParryState : PlayerAbilityState
{
    public bool isShieldParryAvail { get; private set; }
    public bool isShieldParryInAirAvail { get; private set; }
    
    public Timer shieldParryCoolDownTimer;
    public Timer shieldParryInAirCoolDownTimer;

    private bool inAirParry;
    private bool isParried;
    private bool isShielded;

    public PlayerShieldParryState(Player player, string animBoolName) : base(player, animBoolName)
    {
        isShieldParryAvail = true;
        shieldParryCoolDownTimer = new Timer(playerData.shieldParryCoolDownTime);
        shieldParryCoolDownTimer.timerAction += () => { isShieldParryAvail = true; };
        shieldParryInAirCoolDownTimer = new Timer(playerData.shieldParryInAirCoolDownTime);
        shieldParryInAirCoolDownTimer.timerAction += () => { isShieldParryInAirAvail = true; };
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        Debug.Log("Animation Played. This should be called after damage function and knockback function");
        canTransit = false;

        if (index == 0)
        {
            isShielded = true;
            player.animator.ResetTrigger("shieldParryButtonPressed");
        }
        else if (index == 1)
        {
            isParried = true;

            if (!inAirParry)
            {
                shieldParryCoolDownTimer.StartSingleUseTimer();
            }
        }
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        canTransit = true;
        isShieldParryAvail = false;

        if (index == 0)
        {
            isShielded = false;
        }
        else if (index == 1)
        {
            isParried = false;
        }
    }

    public override void DoChecks()
    {
        base.DoChecks();

        player.animator.SetBool("inAir", !isGrounded);
    }

    public override void Enter()
    {
        base.Enter();

        player.stateMachineToAnimator.state = this;
        player.animator.SetBool("inAir", !isGrounded);
        inAirParry = !isGrounded;
        canTransit = isGrounded;

        isShieldParryAvail = false;
        if (inAirParry)
        {
            isShieldParryInAirAvail = false;
            player.combat.DoAttack(player.combat.inAirParryArea);
        }
        else
        {
            // player.movement.SetVelocityXChangeOverTime(0.0f, player.rigidBody.velocity.magnitude / 10.0f, Ease.OutSine, true);
            player.movement.SetVelocityX(0.0f);
            player.combat.DoAttack(player.combat.parryArea);
        }
    }

    public override void Exit()
    {
        base.Exit();

        isParried = false;
        isShielded = false;
        isGrounded = player.detection.isGrounded();
        player.animator.SetBool("inAir", !isGrounded);

        if (inAirParry)
        {
            shieldParryInAirCoolDownTimer.StartSingleUseTimer();
        }
        shieldParryCoolDownTimer.StartSingleUseTimer();

        player.combat.ReleaseShieldParryPrefabs(player.combat.parryArea);
        player.combat.ReleaseShieldParryPrefabs(player.combat.inAirParryArea);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void LateLogicUpdate()
    {
        base.LateLogicUpdate();

        // player.animator.ResetTrigger("parried");
        // player.animator.ResetTrigger("shielded");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region State Transition Logic
        if (!onStateExit)
        {
            if (inAirParry)
            {
                if (isGrounded || canTransit)
                {
                    isAbilityDone = true;
                }
            }
            else
            {
                if (isParried && isGrounded && player.inputHandler.shieldParryInputPressed)
                {
                    player.animator.SetTrigger("shieldParryButtonPressed");
                    player.combat.DoAttack(player.combat.parryArea);
                }
                else if (!shieldParryInput)
                {
                    if (canTransit)
                    {
                        isAbilityDone = true;
                    }
                }

                player.movement.CheckIfShouldFlip(inputX);
            }
        }
        #endregion

        #region Physics Logic
        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (isParried || isShielded)
                {
                    if (player.detection.isDetectingLedgeBack() && player.rigidBody.velocity.x * player.transform.right.x < 0)
                    {
                        player.movement.SetVelocityX(0.0f);
                    }
                }
                else
                {
                    player.movement.SetVelocityX(0.0f);

                    if (isOnSlope)
                    {
                        player.movement.SetVelocityY(0.0f);
                    }
                }
            }

            player.movement.RigidBodyController(false);
        }
        #endregion
    }
}
