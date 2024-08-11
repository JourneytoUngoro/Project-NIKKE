using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    public DataManager dataManager { get; private set; }
    public UIManager uiManager { get; private set; }
    public GameManager gameManager { get; private set; }
    public SceneTransitionManager sceneTransitionManager { get; private set; }
    public ObjectPoolingManager objectPoolingManager { get; private set; }
    public DialogueManager dialogueManager { get; private set; }

    private void Awake()
    {
        Instance = this;

        dataManager = GetComponentInChildren<DataManager>();
        uiManager = GetComponentInChildren<UIManager>();
        gameManager = GetComponentInChildren<GameManager>();
        sceneTransitionManager = GetComponentInChildren<SceneTransitionManager>();
        objectPoolingManager = GetComponentInChildren<ObjectPoolingManager>();
        dialogueManager = GetComponentInChildren<DialogueManager>();
    }
}
