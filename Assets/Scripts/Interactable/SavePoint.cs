using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private bool isInteractableStay;

    private bool isFirstActive = true;

    private void Start()
    {
        isInteractableStay = false;
    }

    private void OnEnable()
    {
        // Manager.Instance.gameManager.player.OnObjectInteractionInput += OpenInteractionMenu;
    }

    private void OnDisable()
    {
        // Manager.Instance.gameManager.player.OnInteractionInput -= OpenInteractionMenu;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInteractableStay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInteractableStay = false;
        }
    }

    private void OpenInteractionMenu()
    {
        if (isFirstActive)
        {
            Manager.Instance.dataManager.SaveGame();
        }
        else if (isInteractableStay)
        {
            Debug.Log("Open Save Point Menu");
            Manager.Instance.uiManager.OpenSavePointMenu();
        }
    }

    public void LoadData(GameData data)
    {
        if (data.unlockedSavePoints.Contains(id))
        {
            isFirstActive = false;
        }
    }

    public void SaveData(GameData data)
    {
        data.unlockedSavePoints.Add(id);
        data.lastSavePoint = id;
        data.currentScene = gameObject.scene.name;
    }
}
