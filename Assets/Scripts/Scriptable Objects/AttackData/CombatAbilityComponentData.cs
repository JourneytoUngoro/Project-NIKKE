using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CombatAbilityComponentData
{
    [SerializeField, HideInInspector] private string name;

    [SerializeField] private bool repeatData;

    public CombatAbilityComponentData()
    {
        name = this.GetType().Name;
    }

    public virtual void InitializeCombatAbilityData(int numberOfAttacks) { }
}

[System.Serializable]
public abstract class CombatAbilityComponentData<T> : CombatAbilityComponentData where T : CombatAbilityComponentElementData
{
    [SerializeField] private bool repeatData;
    [SerializeField] private T[] elementData;
    
    public T GetElementData(int index) => elementData[repeatData ? 0 : index];

    public T[] GetAllElementData() => elementData;

    public void SetElementDataNames()
    {
        for (int i = 0; i <  elementData.Length; i++)
        {
            elementData[i].SetElementName(i + 1);
        }
    }

    public override void InitializeCombatAbilityData(int numberOfAttacks)
    {
        int newLength = repeatData ? 1 : numberOfAttacks;

        int oldLength = elementData != null ? elementData.Length : 0;

        if (!oldLength.Equals(newLength))
        {
            Array.Resize(ref elementData, newLength);

            if (oldLength < newLength)
            {
                for (int i = oldLength; i < elementData.Length; i++)
                {
                    T newElementData = Activator.CreateInstance(typeof(T)) as T;
                    elementData[i] = newElementData;
                }
            }

            SetElementDataNames();
        }
    }
}