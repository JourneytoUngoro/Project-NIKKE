using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerDodgeState : PlayerAbilityState
{
    #region Timer Variable
    public Timer dodgeCoolDownTimer;
    #endregion

    #region Check Variables
    private bool isOnSlope;
    #endregion

    #region Other Variables
    private bool dodgeAvail;
    private bool doBackstep;
    private bool functionCalled;
    private float elapsedTime;
    #endregion

    public PlayerDodgeState(Player player, string animBoolName) : base(player, animBoolName)
    {
        dodgeCoolDownTimer = new Timer(playerData.dodgeCoolDownTime);
        dodgeCoolDownTimer.timerAction += () => { dodgeAvail = true; };
        // 자식 클래스에서 부모 클래스의 변수를 만들어도 부모 클래스에선 접근이 되지 않는다.
        dodgeAvail = true;
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        isAnimationActionTriggered = true;
        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        functionCalled = true;

        if (!isGrounded)
        {
            isAbilityDone = true;
        }
        else if (functionCalled)
        {
            isAbilityDone = true;
        }
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isOnSlope = player.detection.isOnSlope();
    }

    public override void Enter()
    {
        base.Enter();

        functionCalled = false;
        elapsedTime = 0.0f;
        dodgeAvail = false;
        player.inputHandler.InactiveDodgeInput();
        dodgeCoolDownTimer.StartSingleUseTimer();
        player.stateMachineToAnimator.state = this;
        player.gameObject.layer = LayerMask.NameToLayer("PlayerDodge");
        doBackstep = inputX == 0;
        player.animator.SetBool("backstep", doBackstep);
    }

    public override void Exit()
    {
        base.Exit();

        player.gameObject.layer = LayerMask.NameToLayer("Player");
        player.movement.SetVelocityMultiplier(Vector2.one);
        player.animator.SetBool("backstep", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            #region Physics Logic
            if (!isAnimationActionTriggered)
            {
                elapsedTime += Time.fixedDeltaTime;

                if (doBackstep)
                {
                    float velocityMultiplierOverTime = Mathf.Clamp(1.0f - DOVirtual.EasedValue(0.0f, 1.0f, elapsedTime / 0.2f, Ease.InCubic), 0.0f, 1.0f);
                    
                    if (isOnSlope && isGrounded)
                    {
                        if (player.detection.slopePerpNormal.y * -facingDirection > 0)
                        {
                            player.movement.SetVelocityMultiplier(Vector2.one * 0.8f);
                        }
                        else
                        {
                            player.movement.SetVelocityMultiplier(Vector2.one * 1.4f);
                        }

                        workSpace.Set(player.detection.slopePerpNormal.x * -facingDirection, player.detection.slopePerpNormal.y * -facingDirection);

                        player.movement.SetVelocity(workSpace * velocityMultiplierOverTime * playerData.backstepSpeed);
                    }
                    else
                    {
                        player.movement.SetVelocityMultiplier(Vector2.one);
                        if (isGrounded)
                        {
                            player.movement.SetVelocityY(0.0f);
                        }

                        player.movement.SetVelocityX(velocityMultiplierOverTime * -facingDirection * playerData.backstepSpeed);
                    }
                }
                else
                {
                    float velocityMultiplierOverTime = Mathf.Clamp(1.0f - DOVirtual.EasedValue(0.0f, 1.0f, elapsedTime / 0.4f, Ease.InSine), 0.0f, 1.0f);

                    if (isOnSlope && isGrounded)
                    {
                        if (player.detection.slopePerpNormal.y * facingDirection > 0)
                        {
                            player.movement.SetVelocityMultiplier(Vector2.one * 0.8f);
                        }
                        else
                        {
                            player.movement.SetVelocityMultiplier(Vector2.one * 1.4f);
                        }

                        workSpace.Set(player.detection.slopePerpNormal.x * facingDirection, player.detection.slopePerpNormal.y * facingDirection);

                        player.movement.SetVelocity(workSpace * velocityMultiplierOverTime * playerData.dodgeSpeed);
                    }
                    else
                    {
                        player.movement.SetVelocityMultiplier(Vector2.one);
                        if (isGrounded)
                        {
                            player.movement.SetVelocityY(0.0f);
                        }

                        player.movement.SetVelocityX(velocityMultiplierOverTime * facingDirection * playerData.dodgeSpeed);
                    }
                }
            }
            else if (isGrounded && currentVelocity.y < epsilon)
            {
                player.movement.SetVelocityZero();
            }
            #endregion
        }
    }

    public bool IsDodgeAvail() => dodgeAvail;
}
