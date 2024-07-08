using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId;

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI savePointName;
    [SerializeField] private TextMeshProUGUI lastPlayTime;

    private Button slotButton;
    private Button clearButton;

    public bool hasData { get; private set; } = false;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
        clearButton = GetComponentsInChildren<Button>(true)[1];
    }

    public void SetData(GameData data)
    {
        if (data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }
        else
        {
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            playerLevel.text = "Lv. " + data.playerLevel.ToString();
            savePointName.text = "Save Point: " + data.lastSavePoint.ToString();
            lastPlayTime.text = "Last Play Time: " + data.lastPlayTime;
        }
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    public void SetInteractable(bool interactable)
    {
        slotButton.interactable = interactable;
        clearButton.interactable = interactable;
    }
}
