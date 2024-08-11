using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePointSaveSlotMenu : Menu
{
    private Navigation customNav;
    private SavePointMenu savePointMenu;

    public override void Awake()
    {
        base.Awake();

        customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        savePointMenu = GetComponentInParent<SavePointMenu>();

        foreach (Button button in menuButtons)
        {
            customNav.selectOnUp = button.navigation.selectOnUp;
            customNav.selectOnDown = button.navigation.selectOnDown;
            customNav.selectOnLeft = savePointMenu.prevButton;
            button.navigation = customNav;
            if (Manager.Instance.dataManager.selectedProfileId == button.gameObject.GetComponent<SaveSlot>().GetProfileId())
            {
                SetFirstSelected(button);
            }
        }
    }
}
