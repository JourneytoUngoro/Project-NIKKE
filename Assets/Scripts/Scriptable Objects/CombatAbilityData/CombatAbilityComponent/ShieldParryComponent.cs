using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShieldParryComponent : CombatAbilityComponent
{
    [field: SerializeField] public bool isParry { get; private set; } = true;
    [field: SerializeField] public bool changeToShield { get; private set; } = true;
    [field: SerializeField] public float[] parryTime { get; private set; }
    [field: SerializeField] public float[] parryDurationTime { get; private set; }
    [field: SerializeField] public float parryTimeDecrementReset { get; private set; }
    
    private float lastCalledTime;
    private int currentIndex;

    public override void ApplyCombatAbility(params object[] variables)
    {
        if (Time.time - lastCalledTime < parryTimeDecrementReset)
        {
            currentIndex += 1;
            currentIndex = Mathf.Clamp(0, parryTime.Count() - 1, currentIndex);
        }
        else
        {
            currentIndex = 0;
        }
        lastCalledTime = Time.time;

        OverlapCollider[] overlapColliders = variables[0] as OverlapCollider[];

        ShieldParryPrefab[] shieldParryPrefabs = entity.entityCombat.GetComponentsInChildren<ShieldParryPrefab>();
        foreach (ShieldParryPrefab shieldParryPrefab in shieldParryPrefabs)
        {
            Object.Destroy(shieldParryPrefab.gameObject);
        }

        foreach (OverlapCollider overlapCollider in overlapColliders)
        {
            GameObject shieldParryPrefabGameObject = Manager.Instance.objectPoolingManager.GetGameObject("ShieldParryPrefab");
            shieldParryPrefabGameObject.transform.SetParent(pertainedCombatAbility.sourceEntity.entityCombat.transform);
            shieldParryPrefabGameObject.layer = isParry ? LayerMask.NameToLayer("ParryLayer") : LayerMask.NameToLayer("ShieldLayer");
            ShieldParryPrefab shieldParryPrefab = shieldParryPrefabGameObject.GetComponent<ShieldParryPrefab>();
            shieldParryPrefab.SetParryData(pertainedCombatAbility.sourceEntity, parryTime[currentIndex], parryDurationTime[currentIndex], changeToShield, overlapCollider);
        }
    }
}
