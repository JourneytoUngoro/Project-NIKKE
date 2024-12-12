using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileComponent : CombatAbilityComponent
{
    private enum TargetType { None, Nearest, Farthest, LowestHealth, HighestHealth, Random }

    [field: SerializeField] public GameObject projectileExplosionPrefab { get; private set; }
    [field: SerializeField] public bool rotateIdentical { get; private set; }
    [field: SerializeField] public bool rotateIndependent { get; private set; }
    [field: SerializeField] public Vector2[] manualDirectionVector { get; private set; }

    public override void ApplyCombatAbility(params object[] variables)
    {
        Entity[] targetEntities = (variables[0] as List<Collider2D>).OrderBy(collider => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, collider.transform.position)).Select(collider => collider.GetComponentInParent<Entity>()).ToArray();
        Transform[] projectileFireTransforms = (variables[1] as Transform[]);

        foreach (Transform projectileFireTransform in projectileFireTransforms)
        {
            GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectileExplosionPrefab.name);
            projectileGameObject.transform.position = projectileFireTransform.position;

            Projectile projectile = projectileGameObject.GetComponent<Projectile>();


            /*if (projectile != null)
            {
                projectile.FireProjectile(pertainedCombatAbility.sourceEntity, targetEntity);
            }
            else if (explosion != null)
            {

            }*/
        }
    }
}
