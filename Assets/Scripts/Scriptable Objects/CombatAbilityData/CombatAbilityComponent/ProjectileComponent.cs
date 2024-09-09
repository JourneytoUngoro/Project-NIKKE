using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : CombatAbilityComponent
{
    [SerializeField] GameObject projectilePrefab;

    public override void ApplyCombatAbility(Collider2D target)
    {
        GameObject projectile = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefab.name);
    }
}
