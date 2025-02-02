using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PlayerCombat : Combat
{
    [field: SerializeField] public List<CombatAbilityWithTransforms> meleeAttacks { get; protected set; }
    [field: SerializeField] public List<CombatAbilityWithTransforms> rangedAttacks { get; protected set; }
    [field: SerializeField] public CombatAbilityWithTransforms parryArea { get; protected set; }
    [field: SerializeField] public CombatAbilityWithTransforms inAirParryArea { get; private set; }

    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }

    protected override void ChangeToKnockbackState(KnockbackComponent knockbackComponent, bool isGrounded)
    {
        if (knockbackComponent.isKnockbackDifferentWhenAerial)
        {
            if (!isGrounded)
            {
                player.knockbackState.knockbackTimer.ChangeDuration(knockbackComponent.knockbackTimeWhenAerial);
            }
            else
            {
                player.knockbackState.knockbackTimer.ChangeDuration(knockbackComponent.knockbackTime);
            }
        }
        else
        {
            player.knockbackState.knockbackTimer.ChangeDuration(knockbackComponent.knockbackTime);
        }

        player.playerStateMachine.ChangeState(player.knockbackState);
    }

    protected override void ChangeToKnockbackState(float knockbackTime)
    {
        player.knockbackState.knockbackTimer.ChangeDuration(knockbackTime);
        player.playerStateMachine.ChangeState(player.knockbackState);
    }
}
