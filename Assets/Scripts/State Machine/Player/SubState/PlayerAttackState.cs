using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private Transform attackTransform;

    public PlayerAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public PlayerAttackState(Player player, Transform attackTransform, string animBoolName) : base(player, animBoolName)
    {
        this.attackTransform = attackTransform;
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        player.combat.DoMeleeAttack(playerData.meleeAttackDamage, attackTransform.position, playerData.meleeAttackRadius);
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

        player.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        player.combat.ClearDamagedTargets();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
