using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileComponent : CombatAbilityComponent
{
    private enum TargetType { None, Nearest, Farthest, LowestHealth, HighestHealth, Random }

    [field: SerializeField] public GameObject projectilePrefab { get; private set; }
    [field: SerializeField] public bool checkProjectileRoute { get; private set; }
    [field: SerializeField] public bool autoManualDirection { get; private set; }

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity[] targetEntities = (variables[0] as List<Collider2D>).OrderBy(collider => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, collider.transform.position)).Select(collider => collider.GetComponentInParent<Entity>()).ToArray();
        Transform[] projectileFireTransforms = (variables[1] as Transform[]);

        foreach (Transform projectileFireTransform in projectileFireTransforms)
        {
            GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefab.name);
            projectileGameObject.transform.position = projectileFireTransform.position;
            projectileGameObject.transform.rotation = pertainedCombatAbility.sourceEntity.transform.rotation;

            Projectile projectile = projectileGameObject.GetComponent<Projectile>();
            Explosion explosion = projectileGameObject.GetComponent<Explosion>();

            if (projectile != null)
            {
                if (projectile.GetProjectileType() == ProjectileType.Straight)
                {
                    switch (projectile.GetStraightProjectileDirection())
                    {
                        case StraightProjectileDirection.Manual:
                            if (autoManualDirection)
                            {
                                Vector2 direction = projectileFireTransform.position - pertainedCombatAbility.sourceEntity.transform.position;
                                projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, checkProjectileRoute, direction);
                            }
                            else
                            {
                                projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, checkProjectileRoute, projectileFireTransform.right);
                            }
                            break;

                        case StraightProjectileDirection.Toward:
                            projectile.FireProjectile(pertainedCombatAbility.sourceEntity, targetEntities, checkProjectileRoute);
                            break;

                        case StraightProjectileDirection.Forward:
                            projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, checkProjectileRoute);
                            break;
                    }
                }
                else
                {
                    projectile.FireProjectile(pertainedCombatAbility.sourceEntity, targetEntities, checkProjectileRoute);
                }
            }

            if (explosion != null)
            {
                explosion.SetExplosion(pertainedCombatAbility.sourceEntity);
            }
        }
    }
}
