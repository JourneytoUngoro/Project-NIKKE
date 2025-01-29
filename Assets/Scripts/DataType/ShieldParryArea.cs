using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShieldParryArea
{
    [field: SerializeField] public ShieldParryType shieldParryType { get; private set; }
    [field: SerializeField] public bool changeToShield { get; private set; } = true;
    [field: SerializeField] public float[] parryTime { get; private set; }
    [field: SerializeField] public float[] parryDurationTime { get; private set; }
    [field: SerializeField] public float parryTimeDecrementReset { get; private set; }

    public float lastCalledTime;

    public int currentIndex;
}
