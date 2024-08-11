using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHolder : MonoBehaviour
{
    [SerializeField] private List<SceneField> scenesToLoad;
    [SerializeField] private SceneField currentScene;
    [SerializeField] private SceneField targetScene;

    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void Start()
    {
        Debug.Log("Start");
    }
}
