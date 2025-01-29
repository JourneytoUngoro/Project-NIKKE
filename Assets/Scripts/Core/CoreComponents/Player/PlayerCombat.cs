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

    /*[SerializeField] private Transform jumpAttackTransform;
    [SerializeField] private Vector2 jumpAttackSize;
    [SerializeField] private Transform jumpFinishAttackTransform;
    [SerializeField] private float jumpFinishAttackRadius;
    [SerializeField] [Range(0, 180)] private float jumpFinishAttackClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float jumpFinishAttackCounterClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float shieldParryAreaClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float shieldParryAreaCounterClockwiseAngle;*/

    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }

    protected override void ChangeToKnockbackState(KnockbackComponent knockbackComponent, bool isGrounded)
    {
        if (knockbackComponent.isKnockbackDifferentInAir)
        {
            if (!isGrounded)
            {
                player.knockbackState.knockbackTimer.ChangeDuration(knockbackComponent.knockbackTimeInAir);
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
