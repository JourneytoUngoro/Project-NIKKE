using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TargetType { CurrentTarget, Nearest, Farthest, LowestHealth, HighestHealth, Random }

public class ProjectileComponent : CombatAbilityComponent
{
    [field: SerializeField] public GameObject projectilePrefab { get; private set; }
    [field: SerializeField] public bool singleTarget { get; private set; } = true;
    [field: SerializeField] public bool multipleTarget { get; private set; } = false;
    [field: SerializeField] public bool identicalTargetHit { get; private set; } = false;
    [field: SerializeField] public TargetType targetTypePriority { get; private set; } = TargetType.CurrentTarget;
    [field: SerializeField] public List<TargetType> targetTypePriorities { get; private set; } = new List<TargetType>();
    [field: SerializeField] public bool checkProjectileRoute { get; private set; }
    [field: SerializeField, Tooltip("directionVector = projectileGeneratePosition - sourceEntityPosition")] public bool autoManualDirection { get; private set; }

    // TODO: 단일 목표물인 경우에도 복수의 목표물 우선순위를 정하도록 변경?
    // TODO: 복수 목표물인 경우 동일 대상 타격 비허용일 때 모든 타격 대상을 순회한 상황에 대한 예외처리
    public override void ApplyCombatAbility(params object[] variables)
    {
        List<Entity> targetEntities = (variables[0] as List<Collider2D>).OrderBy(collider => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, collider.transform.position)).Select(collider => collider.GetComponentInParent<Entity>()).ToList();
        Transform[] projectileFireTransforms = (variables[1] as Transform[]);
        if (!targetEntities.Contains(pertainedCombatAbility.sourceEntity.entityDetection.currentTarget))
        {
            targetEntities.Add(pertainedCombatAbility.sourceEntity.entityDetection.currentTarget);
        }

        Entity currentTargetEntity = null;

        if (singleTarget)
        {
            switch (targetTypePriority)
            {
                case TargetType.CurrentTarget:
                    currentTargetEntity = pertainedCombatAbility.sourceEntity.entityDetection.currentTarget; break;
                case TargetType.Nearest:
                    currentTargetEntity = targetEntities.OrderBy(targetEntity => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, targetEntity.transform.position)).FirstOrDefault(); break;
                case TargetType.Farthest:
                    currentTargetEntity = targetEntities.OrderByDescending(targetEntity => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, targetEntity.transform.position)).FirstOrDefault(); break;
                case TargetType.LowestHealth:
                    currentTargetEntity = targetEntities.OrderBy(targetEntity => targetEntity.entityStats.health.currentValue).FirstOrDefault(); break;
                case TargetType.HighestHealth:
                    currentTargetEntity = targetEntities.OrderByDescending(targetEntity => targetEntity.entityStats.health.currentValue).FirstOrDefault(); break;
                case TargetType.Random:
                    currentTargetEntity = targetEntities.GetRandom(); break;
                default:
                    break;
            }
        }

        Transform[] shuffledProjectileFireTransforms = projectileFireTransforms.Shuffle(false).ToArray();
        for (int index = 0; index < shuffledProjectileFireTransforms.Count(); index++)
        {
            if (multipleTarget)
            {
                int typePriorityIndex = Mathf.Min(index, targetTypePriorities.Count() - 1);

                switch (targetTypePriorities[typePriorityIndex])
                {
                    case TargetType.CurrentTarget:
                        currentTargetEntity = targetEntities.Contains(pertainedCombatAbility.sourceEntity.entityDetection.currentTarget) ? pertainedCombatAbility.sourceEntity.entityDetection.currentTarget : null; break;
                    case TargetType.Nearest:
                        currentTargetEntity = targetEntities.OrderBy(targetEntity => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, targetEntity.transform.position)).FirstOrDefault(); break;
                    case TargetType.Farthest:
                        currentTargetEntity = targetEntities.OrderByDescending(targetEntity => Vector2.Distance(pertainedCombatAbility.sourceEntity.transform.position, targetEntity.transform.position)).FirstOrDefault(); break;
                    case TargetType.LowestHealth:
                        currentTargetEntity = targetEntities.OrderBy(targetEntity => targetEntity.entityStats.health.currentValue).FirstOrDefault(); break;
                    case TargetType.HighestHealth:
                        currentTargetEntity = targetEntities.OrderByDescending(targetEntity => targetEntity.entityStats.health.currentValue).FirstOrDefault(); break;
                    case TargetType.Random:
                        currentTargetEntity = targetEntities.GetRandom(); break;
                    default:
                        break;
                }

                if (!identicalTargetHit)
                {
                    targetEntities.Remove(currentTargetEntity);
                }
            }

            GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefab.name);
            projectileGameObject.transform.position = projectileFireTransforms[index].position;
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
                                Vector2 direction = projectileFireTransforms[index].position - pertainedCombatAbility.sourceEntity.transform.position;
                                projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, checkProjectileRoute, direction);
                            }
                            else
                            {
                                projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, checkProjectileRoute, projectileFireTransforms[index].right);
                            }
                            break;

                        case StraightProjectileDirection.Toward:
                            if (currentTargetEntity == null)
                            {
                                Debug.LogWarning($"GameObject '{projectile.name}' needs target but failed to find one.");
                                continue;
                            }
                            projectile.FireProjectile(pertainedCombatAbility.sourceEntity, currentTargetEntity, checkProjectileRoute);
                            break;

                        case StraightProjectileDirection.Forward:
                            projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, checkProjectileRoute);
                            break;
                    }
                }
                else
                {
                    if (currentTargetEntity == null)
                    {
                        Debug.LogWarning($"GameObject '{projectile.name}' needs target but failed to find one.");
                        continue;
                    }
                    projectile.FireProjectile(pertainedCombatAbility.sourceEntity, currentTargetEntity, checkProjectileRoute);
                }
            }

            if (explosion != null)
            {
                explosion.SetExplosion(pertainedCombatAbility.sourceEntity);
            }
        }
    }
}
