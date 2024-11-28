using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileComponent : CombatAbilityComponent
{
    private enum TargetType { None, Nearest, Farthest, LowestHealth, HighestHealth, Random }

    [SerializeField] private List<Tuple<GameObject, int>> projectilePrefabs;

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity[] targetEntities = (variables[0] as List<Collider2D>).Select(collider => collider.GetComponentInParent<Entity>()).ToArray();
        OverlapCollider[] overlapColliders = (variables[1] as OverlapCollider[]);

        int index = 0;
        /*for (int count = 0; count < entity.entityCombat.rangedAttacks[attackStroke].overlapColliders.Length; count++)
        {
            GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefabs[index].Item1.name);
            Projectile projectile = projectileGameObject.GetComponent<Projectile>();
            projectileGameObject.transform.position = overlapCollider.centerTransform.position;
            projectile.FireProjectile(entity, targetEntity)
        }

        foreach (OverlapCollider overlapCollider in entity.entityCombat.rangedAttacks[entity.entityCombat.currentAttackStroke].overlapColliders)
        {
            GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefab.name);
            Projectile projectile = projectileGameObject.GetComponent<Projectile>();
            projectileGameObject.transform.position = overlapCollider.centerTransform.position;
            projectile.FireProjectile(entity, targetEntity)
        }*/
    }
}
