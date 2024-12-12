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
    [field: SerializeField] public bool rotateTransform { get; private set; }
    [field: SerializeField] public bool rotatePrefab { get; private set; }
    [field: SerializeField] public bool autoManualDirection { get; private set; }
    [field: SerializeField] public Vector2[] manualDirectionVector { get; private set; }
    // TODO: Auto manual direction vector that sets the direction of the projectile to 'instantiated position - source entity position.'

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity[] targetEntities = (variables[0] as List<Collider2D>).OrderBy(collider => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, collider.transform.position)).Select(collider => collider.GetComponentInParent<Entity>()).ToArray();
        Transform[] projectileFireTransforms = (variables[1] as Transform[]);

        foreach (Transform projectileFireTransform in projectileFireTransforms)
        {
            GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefab.name);
            projectileGameObject.transform.position = projectileFireTransform.position;

            Projectile projectile = projectileGameObject.GetComponent<Projectile>();

            if (projectile != null)
            {
                if (projectile.GetProjectileType() == ProjectileType.Straight && projectile.GetStraightProjectileDirection() == StraightProjectileDirection.Manual)
                {
                    // projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, checkProjectileRoute, manualDirectionVector);
                }
                else
                {

                }
            }
        }
    }
}
