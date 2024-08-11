using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavePointMenu : Menu
{
    [SerializeField] private GameObject slotMenu;
    [SerializeField] private Button saveMenuButton;
    [SerializeField] private Button loadMenuButton;
    [SerializeField] private Button statsMenuButton;

    public Button prevButton { get; private set; }

    private bool isSlotOpened;

    public void OnSaveMenuButtonClicked()
    {
        if (isSlotOpened)
        {
            slotMenu.SetActive(false);
        }
        else
        {
            prevButton = saveMenuButton;
            slotMenu.SetActive(true);
        }
    }

    public void OnLoadMenuButtonClicked()
    {
        if (isSlotOpened)
        {
            slotMenu.SetActive(false);
        }
        else
        {
            prevButton = loadMenuButton;
            slotMenu.SetActive(true);
        }
    }

    public void OnStatsMenuButtonClicked()
    {
        prevButton = statsMenuButton;
        
        // TODO: Implement stat menu
        Debug.Log("open stat menu");
    }

    public void OnResumeMenuButtonClicked()
    {
        isSlotOpened = false;
        slotMenu.SetActive(false);
        Manager.Instance.uiManager.CloseSavePointMenu();
    }

    public void OnMainMenuButtonClicked()
    {
        isSlotOpened = false;
        slotMenu.SetActive(false);
        Manager.Instance.uiManager.CloseSavePointMenu();
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
