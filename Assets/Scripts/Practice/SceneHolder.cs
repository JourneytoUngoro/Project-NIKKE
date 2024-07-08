using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHolder : MonoBehaviour
{
    [SerializeField] private List<SceneField> scenesToLoad;
    [SerializeField] private SceneField currentScene;
    [SerializeField] private SceneField targetScene;

    private void Start()
    {
        Manager.instance.sceneTransitionManager.LoadAndUnloadScenes(scenesToLoad, currentScene);
        Manager.instance.sceneTransitionManager.SceneTransition(targetScene);
    }
}
