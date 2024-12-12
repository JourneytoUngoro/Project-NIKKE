using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PlayerCombat : Combat
{
    [SerializeField] private Transform jumpAttackTransform;
    [SerializeField] private Vector2 jumpAttackSize;
    [SerializeField] private Transform jumpFinishAttackTransform;
    [SerializeField] private float jumpFinishAttackRadius;
    [SerializeField] [Range(0, 180)] private float jumpFinishAttackClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float jumpFinishAttackCounterClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float shieldParryAreaClockwiseAngle;
    [SerializeField] [Range(0, 180)] private float shieldParryAreaCounterClockwiseAngle;

    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
        /*foreach (MemberInfo memberInfo in this.GetType().GetMembers())
        {
            Debug.Log(memberInfo.MemberType + ": " + memberInfo.Name);
        }
        Debug.Log("------------------------------------");*/
        foreach (FieldInfo memberInfo in typeof(Combat).GetFields())//.Where(field => field.FieldType == typeof(Transform)))
        {
            Debug.Log(memberInfo.MemberType + ": " + memberInfo.Name);
        }
    }

    public override void GetKnockback(KnockbackComponent knockbackComponent)
    {
        base.GetKnockback(knockbackComponent);

        player.playerStateMachine.ChangeState(player.knockbackState);
    }

    public void DoJumpAttack()
    {
        Collider2D[] damageTargets = Physics2D.OverlapBoxAll(jumpAttackTransform.position, jumpAttackSize, whatIsDamageable);

        foreach (Collider2D damageTarget in damageTargets)
        {
            if (!damagedTargets.Contains(damageTarget))
            {
                damageTarget.SendMessage("GetDamage", player.playerData.playerJumpAttackInfo);
                damagedTargets.Add(damageTarget);
            }
        }
    }

    public void DoJumpFinishAttack()
    {
        Collider2D[] damageTargets = Physics2D.OverlapCircleAll(jumpFinishAttackTransform.position, jumpFinishAttackRadius, whatIsDamageable);

        foreach (Collider2D damageTarget in damageTargets)
        {
            Vector2 targetVector = damageTarget.transform.position - jumpFinishAttackTransform.position;
            if (CheckWithinAngle(transform.up, targetVector, jumpFinishAttackCounterClockwiseAngle, jumpFinishAttackClockwiseAngle))
            {
                damageTarget.SendMessage("GetDamage", player.playerData.playerJumpAttackInfo);
            }
        }
    }
}
