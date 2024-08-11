using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConsistentGameObject : MonoBehaviour
{
    public static ConsistentGameObject Instance;

    [SerializeField] private GameObject Manager;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject player;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        #endregion

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDiable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name == "MainMenu")
        {
            mainCamera.SetActive(false);
            canvas.SetActive(false);
            player.SetActive(false);
        }
        else
        {
            mainCamera.SetActive(true);
            canvas.SetActive(true);
            player.SetActive(true);
        }
    }
}
