using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReboundComponentElementData : CombatAbilityComponentElementData
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private float speed;
}
