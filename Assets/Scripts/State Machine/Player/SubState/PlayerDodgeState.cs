using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerDodgeState : PlayerAbilityState
{
    #region Timer Variable
    public Timer dodgeCoolDownTimer;
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

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        if (doBackstep)
        {
            isAbilityDone = true;
        }
        else
        {
            if (!isGrounded)
            {
                isAbilityDone = true;
            }
            else if (functionCalled)
            {
                isAbilityDone = true;
            }
        }
        
        functionCalled = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        functionCalled = false;
        elapsedTime = 0.0f;
        dodgeAvail = false;
        player.inputHandler.InactiveDodgeInput();
        player.stateMachineToAnimator.state = this;
        player.gameObject.layer = LayerMask.NameToLayer("PlayerDodge");
        doBackstep = inputX == 0;
        player.animator.SetBool("backstep", doBackstep);
        
        if (doBackstep)
        {
            player.movement.SetVelocityXChangeOverTime(playerData.backstepSpeed * -facingDirection, playerData.backstepTime, Ease.InCubic, true);
        }
        else
        {
            player.movement.SetVelocityXChangeOverTime(playerData.dodgeSpeed * facingDirection, playerData.dodgeTime, Ease.InSine, true);
        }
    }

    public override void Exit()
    {
        base.Exit();

        dodgeCoolDownTimer.StartSingleUseTimer();
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

        #region Physics Logic
        if (!onStateExit)
        {
            player.movement.RigidBodyController(!doBackstep);
        }
        #endregion
    }

    public bool IsDodgeAvail() => dodgeAvail;
}
