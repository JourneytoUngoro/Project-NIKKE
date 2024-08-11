using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private WarningMenu warningMenu;

    [Header("Back Button")]
    [SerializeField] private Button backButton;

    private SaveSlot[] slots;

    private bool isLoadingGame = false;

    public override void Awake()
    {
        base.Awake();

        slots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        if (isLoadingGame)
        {
            Manager.Instance.dataManager.ChangeSelectedProfileIdAndLoadGameWithData(saveSlot.GetProfileId());
            SaveGameAndLoadScene();
        }
        else if (saveSlot.hasData)
        {
            warningMenu.ActivateMenu("Selected slot already has existing data and will be overwrittened.\nDo you wish to continue?",
                () =>
                {
                    Manager.Instance.dataManager.ChangeSelectedProfileIdAndLoadGameWithData(saveSlot.GetProfileId());
                    Manager.Instance.dataManager.NewGame();
                    SaveGameAndLoadScene();
                },
                () =>
                {
                    ActivateMenu(isLoadingGame);
                });
        }
        else
        {
            Manager.Instance.dataManager.ChangeSelectedProfileIdAndLoadGameWithData(saveSlot.GetProfileId());
            Manager.Instance.dataManager.NewGame();
            SaveGameAndLoadScene();
        }
    }

    private void SaveGameAndLoadScene()
    {
        Manager.Instance.dataManager.SaveGame();

        SceneManager.LoadSceneAsync(Manager.Instance.dataManager.gameData.currentScene);
    }

    public void OnClearClicked(SaveSlot slot)
    {
        DisableMenuButtons();

        warningMenu.ActivateMenu("Data cannot be recovered once deleted.\nDo you wish to continue?",
            () =>
            {
                Manager.Instance.dataManager.DeleteProfileData(slot.GetProfileId());
                ActivateMenu(isLoadingGame);
            },
            () =>
            {
                ActivateMenu(isLoadingGame);
            });
    }

    public override void OnBackClicked()
    {
        base.OnBackClicked();

        mainMenu.ActivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        gameObject.SetActive(true);

        this.isLoadingGame = isLoadingGame;

        Dictionary<string, GameData> profilesGameData = Manager.Instance.dataManager.GetAllProfilesGameData();

        backButton.interactable = true;

        GameObject firstSelected = backButton.gameObject;
        foreach (SaveSlot saveSlot in slots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);

            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
        }

        Button firstSelectedButton = firstSelected.GetComponent<Button>();
        SetFirstSelected(firstSelectedButton);
    }
}
