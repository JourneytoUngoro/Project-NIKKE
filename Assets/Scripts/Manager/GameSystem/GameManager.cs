using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour, IDataPersistance
{
    [field: SerializeField] public Player player { get; private set; }
    public bool isPaused { get; private set; }

    public void PauseGame()
    {
        Debug.Log("Game Paused");
        isPaused = true;
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");
        isPaused = false;
        Time.timeScale = 1.0f;
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void LoadData(GameData data)
    {
        player.transform.position = data.lastSavePosition;
    }

    public void SaveData(GameData data)
    {
        data.lastSavePosition = player.transform.position;
    }
}