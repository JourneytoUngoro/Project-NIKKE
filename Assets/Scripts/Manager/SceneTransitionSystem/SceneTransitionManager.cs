using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // called when player arrives specific position for scene transition
    public void SceneTransition(SceneField targetScene)
    {
        // TODO: Still gives error when the scene is not yet loaded.
        // What if the scene gets loaded and then unloaded and then loaded in a very short period of time?

        bool isLoading = false;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == targetScene.SceneName)
            {
                isLoading = true;
                // scene is added to scene manager even its loading is not yet done.
            }
        }

        // if scene is not in SceneManager, it means that the scene is neither loaded nor currently loading
        if (!isLoading)
        {
            Debug.Log("isLoading is false");
            SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        }

        // if the scene is at least currently loading, though it gives error logs, it will eventually transit to target scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
    }

    // called when scene transition is complete
    // load the scenes that are adjacent to current scene and unload that are not
    public void LoadAndUnloadScenes(List<SceneField> scenesToLoad, SceneField targetScene)
    {
        scenesToLoad.Add(targetScene);
        LoadScene(scenesToLoad);
        UnloadScene(scenesToLoad);
    }

    // this function loads the scene that is adjacent to target scene
    private void LoadScene(List<SceneField> scenesToLoad)
    {
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            bool isSceneLoaded = false;

            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToLoad[i].SceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if (!isSceneLoaded)
            {
                SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
            }
        }
    }

    // this function unloads the scenes that seems to be unnecessary
    private void UnloadScene(List<SceneField> scenesToLoad)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            bool isSceneRequired = false;

            Scene loadedScene = SceneManager.GetSceneAt(i);

            for (int j = 0; j < scenesToLoad.Count; j++)
            {
                if (loadedScene.name == scenesToLoad[j].SceneName)
                {
                    isSceneRequired = true;
                    break;
                }
            }

            if (!isSceneRequired)
            {
                SceneManager.UnloadSceneAsync(loadedScene);
            }
        }
    }
}
