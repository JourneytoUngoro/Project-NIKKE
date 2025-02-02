using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public Player player { get; private set; }
    public bool isPaused { get; private set; }
    private float originalTimeScale = 1.0f;

    public void PauseGame()
    {
        Debug.Log("Game Paused");
        originalTimeScale = Time.timeScale;
        isPaused = true;
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");
        isPaused = false;
        Time.timeScale = originalTimeScale;
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    /// <summary>
    /// A temporary pause/slow effect function that adjusts the timescale for specific time amount.
    /// </summary>
    /// <param name="pauseDuration"></param>
    public void AdjustTimeScaleGameEffect(float adjustedTimeScale, float pauseDuration)
    {
        Debug.Log($"Timescale adjusted to {adjustedTimeScale} for {pauseDuration} seconds");
        originalTimeScale = Time.timeScale;
        Time.timeScale = adjustedTimeScale;

    }
}