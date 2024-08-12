using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractBase
{
    public enum DoorToSpawnAt
    {
        None,
        One,
        Two,
        Three,
        Four,
        Five
    }

    [Header("Spawn to")]
    [SerializeField] private DoorToSpawnAt doorToSpawnAt;
    [SerializeField] private SceneField targetScene;

    [Space(10f)]
    [Header("Current Door")]
    public DoorToSpawnAt currentDoorIndex;

    public override void Interact()
    {
        if (doorToSpawnAt == DoorToSpawnAt.None) return;
        Manager.Instance.sceneTransitionManager.SceneTransition(targetScene, doorToSpawnAt, true);
    }
}
