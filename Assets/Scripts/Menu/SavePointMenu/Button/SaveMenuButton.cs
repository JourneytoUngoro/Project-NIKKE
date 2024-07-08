using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveMenuButton : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(gameObject.name + " selected");
    }
}
