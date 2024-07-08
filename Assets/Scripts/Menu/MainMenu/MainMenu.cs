using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenuSaveSlotMenu saveSlotMenu;

    [Header("Menu Button")]
    [SerializeField] private Button loadGameButton;

    public override void Awake()
    {
        base.Awake();


    }

    private void Start()
    {
        if (Manager.instance.dataManager.HasGameData())
        {
            SetFirstSelected(loadGameButton);
        }

        DisableButtonsDependingOnData();
    }

    private void DisableButtonsDependingOnData()
    {
        if (!Manager.instance.dataManager.HasGameData())
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
}
