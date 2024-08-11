using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotMenu saveSlotMenu;

    [Header("Menu Button")]
    [SerializeField] private Button loadGameButton;

    private void Start()
    {
        if (Manager.Instance.dataManager.HasGameData())
        {
            SetFirstSelected(loadGameButton);
        }

        DisableButtonsDependingOnData();
    }

    private void DisableButtonsDependingOnData()
    {
        if (!Manager.Instance.dataManager.HasGameData())
        {
            loadGameButton.interactable = false;
        }
    }

    public override void ActivateMenu()
    {
        base.ActivateMenu();

        DisableButtonsDependingOnData();
    }

    public void OnNewGameClicked()
    {
        saveSlotMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }

    public void OnLoadGameClicked()
    {
        saveSlotMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void OnExitGameClicked()
    {
        Application.Quit();
    }
}
