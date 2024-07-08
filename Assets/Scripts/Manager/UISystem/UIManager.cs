using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [field: Header("Menu")]
    [field: SerializeField] public GameObject notificationWindow { get; private set; }
    [field: SerializeField] public GameObject pauseMenu { get; private set; }
    [field: SerializeField] public GameObject savePointMenu { get; private set; }
    [field: SerializeField] public GameObject slotMenu { get; private set; }

    [Header("Health Bar")]
    [SerializeField] private Slider healthBar;

    private void Update()
    {
        // healthBar.value = Manager.instance.player.stats.health.SliderValue();
    }

    public void OpenSavePointMenu()
    {
        Manager.instance.gameManager.isPaused = true;
        Time.timeScale = 0.0f;
        Manager.instance.player.playerInput.SwitchCurrentActionMap("UI");
        savePointMenu.SetActive(true);
        notificationWindow.SetActive(false);
    }

    public void CloseSavePointMenu()
    {
        Manager.instance.gameManager.isPaused = false;
        Time.timeScale = 1.0f;
        Manager.instance.player.playerInput.SwitchCurrentActionMap("InGame");
        savePointMenu.SetActive(false);
    }

    public void OpenNotificationWindow(string notification)
    {
        notificationWindow.SetActive(true);
        notificationWindow.GetComponentInChildren<TextMeshProUGUI>().text = notification;
    }

    public void CloseNotificationWindow()
    {
        notificationWindow.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        Manager.instance.gameManager.isPaused = true;
        Time.timeScale = 0.0f;
        pauseMenu.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        Manager.instance.gameManager.isPaused = false;
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
    }
}
