using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu 
{
    public void OnResumeClicked()
    {
        Manager.Instance.uiManager.ClosePauseMenu();
        DeactivateMenu();
    }

    public void OnMainMenuClicked()
    {
        DeactivateMenu();
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
