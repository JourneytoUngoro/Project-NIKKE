using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileComponent : CombatAbilityComponent
{
    private enum TargetType { None, Nearest, Farthest, LowestHealth, HighestHealth, Random }

    [SerializeField] private List<GameObject> projectilePrefabs;
    [SerializeField] private ProjectileType projectileTypeWhenParried;

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity targetEntity = (variables[0] as List<Collider2D>).OrderBy(collider => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, collider.transform.position)).Select(collider => collider.GetComponentInParent<Entity>()).FirstOrDefault();
        OverlapCollider[] overlapColliders = (variables[1] as OverlapCollider[]);

        int index = 0;
        foreach (OverlapCollider overlapCollider in overlapColliders)
        {
            if (!overlapCollider.overlapBox && !overlapCollider.overlapCircle)
            {
                GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefabs[index].name);
                Projectile projectile = projectileGameObject.GetComponent<Projectile>();
                projectileGameObject.transform.position = overlapCollider.centerTransform.position;
                projectile.FireProjectile(pertainedCombatAbility.sourceEntity, targetEntity);
                index += 1;
            }
        }
    }
}
