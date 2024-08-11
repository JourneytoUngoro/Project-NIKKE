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

    private void Update()
    {
        // healthBar.value = Manager.instance.player.stats.health.SliderValue();
    }

    public void OpenSavePointMenu()
    {
        Manager.Instance.gameManager.PauseGame();
        Manager.Instance.gameManager.player.playerInput.SwitchCurrentActionMap("UI");
        savePointMenu.SetActive(true);
        notificationWindow.SetActive(false);
    }

    public void CloseSavePointMenu()
    {
        Manager.Instance.gameManager.ResumeGame();
        Manager.Instance.gameManager.player.playerInput.SwitchCurrentActionMap("InGame");
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
        Manager.Instance.gameManager.PauseGame();
        pauseMenu.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        Manager.Instance.gameManager.ResumeGame();
        pauseMenu.SetActive(false);
    }
}
