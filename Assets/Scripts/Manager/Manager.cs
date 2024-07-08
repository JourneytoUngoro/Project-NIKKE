using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager instance { get; private set; }
    public DataManager dataManager { get; private set; }
    public UIManager uiManager { get; private set; }
    public GameManager gameManager { get; private set; }
    public SceneTransitionManager sceneTransitionManager { get; private set; }

    public Player player { get; private set; }

    private void Awake()
    {
        #region Singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        #endregion

        dataManager = GetComponentInChildren<DataManager>();
        uiManager = GetComponentInChildren<UIManager>();
        gameManager = GetComponentInChildren<GameManager>();
        sceneTransitionManager = GetComponentInChildren<SceneTransitionManager>();

        player = FindObjectOfType<Player>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<Player>();
    }
}
