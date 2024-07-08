using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool isInteractableStay;

    private void Start()
    {
        isInteractableStay = false;
        Manager.instance.player.OnInteractionInput += OpenInteractionMenu;
    }

    private void OnDestroy()
    {
        Manager.instance.player.OnInteractionInput -= OpenInteractionMenu;
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
        if (isInteractableStay)
        {
            Debug.Log("Open Save Point Menu");
            Manager.instance.uiManager.OpenSavePointMenu();
        }
    }
}
