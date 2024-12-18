using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WarningMenu : Menu
{
    [Header("Components")]
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject confirmButtonPointer;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject cancelButtonPointer;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == confirmButton.gameObject)
        {
            confirmButtonPointer.SetActive(true);
            cancelButtonPointer.SetActive(false);
        }
        else if (EventSystem.current.currentSelectedGameObject == cancelButton.gameObject)
        {
            confirmButtonPointer.SetActive(false);
            cancelButtonPointer.SetActive(true);
        }
    }

    public void ActivateMenu(string mainText, UnityAction confirmAction, UnityAction cancelAction)
    {
        gameObject.SetActive(true);

        this.mainText.text = mainText;

        foreach (Button button in menuButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        confirmButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            confirmAction();
        });

        cancelButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            cancelAction();
        });
    }
}
