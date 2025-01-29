using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShieldParryType { Shield, Parry };

public class ShieldParryComponent : CombatAbilityComponent
{
    [field: SerializeField] public ShieldParryArea[] shieldParryAreas { get; private set; }

    public override void ApplyCombatAbility(params object[] variables)
    {
        OverlapCollider[] overlapColliders = variables[0] as OverlapCollider[];
        ShieldParry[] shieldParryPrefabs = pertainedCombatAbility.sourceEntity.entityCombat.GetComponentsInChildren<ShieldParry>();
        foreach (ShieldParry shieldParryPrefab in shieldParryPrefabs)
        {
            shieldParryPrefab.ReleaseObject();
        }

        if (shieldParryAreas.Count() > overlapColliders.Count())
        {
            Debug.LogWarning($"ShieldParryComponent of {pertainedCombatAbility.sourceEntity.name}'s {pertainedCombatAbility.name} combat ability does not match length between given overlapColliders({overlapColliders.Count()}) and shieldParryAreas({shieldParryAreas.Count()}). Oversized shieldParryAreas will be ignored.");
        }

        for (int index = 0; index < overlapColliders.Count(); index++)
        {
            ShieldParryArea shieldParryArea = shieldParryAreas[Mathf.Min(index, shieldParryAreas.Count() - 1)];
            GameObject shieldParryPrefabGameObject = Manager.Instance.objectPoolingManager.GetGameObject("ShieldParryPrefab");
            shieldParryPrefabGameObject.transform.SetParent(pertainedCombatAbility.sourceEntity.entityCombat.transform);
            ShieldParry shieldParryPrefab = shieldParryPrefabGameObject.GetComponent<ShieldParry>();

            if (shieldParryArea.shieldParryType.Equals(ShieldParryType.Parry))
            {
                shieldParryPrefab.SetParryData(pertainedCombatAbility, shieldParryArea.parryTime[shieldParryArea.currentIndex], shieldParryArea.parryDurationTime[shieldParryArea.currentIndex], shieldParryArea.changeToShield, overlapColliders[index]);
            }
            else
            {
                shieldParryPrefab.SetShieldData(pertainedCombatAbility, overlapColliders[index]);
            }

            if (Time.time != shieldParryArea.lastCalledTime)
            {
                if (Time.time - shieldParryArea.lastCalledTime < shieldParryArea.parryTimeDecrementReset)
                {
                    shieldParryArea.currentIndex += 1;
                    shieldParryArea.currentIndex = Mathf.Clamp(0, shieldParryArea.parryTime.Count() - 1, shieldParryArea.currentIndex);
                }
                else
                {
                    shieldParryArea.currentIndex = 0;
                }
                shieldParryArea.lastCalledTime = Time.time;
            }
        }
    }
}
