using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("First Selected Button")]
    [SerializeField] protected Button firstSelected;

    protected Button[] menuButtons;

    public virtual void Awake()
    {
        menuButtons = GetComponentsInChildren<Button>(true);
    }

    public virtual void OnEnable()
    {
        EnableMenuButtons();
        SetFirstSelected(firstSelected);
    }

    public void SetFirstSelected(Button firstSelectedButton)
    {
        firstSelectedButton.Select();
    }

    public virtual void EnableMenuButtons()
    {
        foreach (Button button in menuButtons)
        {
            button.interactable = true;
        }
    }

    public virtual void DisableMenuButtons()
    {
        foreach(Button button in menuButtons)
        {
            button.interactable = false;
        }
    }

    public virtual void ActivateMenu()
    {
        gameObject.SetActive(true);
    }

    public virtual void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnBackClicked()
    {
        DeactivateMenu();
    }
}
