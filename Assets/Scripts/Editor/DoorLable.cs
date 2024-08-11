using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorTriggerInteraction))]
public class DoorLable : Editor
{
    private static GUIStyle labelStyle;

    private void OnEnable()
    {
        labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnSceneGUI()
    {
        DoorTriggerInteraction door = (DoorTriggerInteraction)target;

        Handles.BeginGUI();
        Handles.Label(door.transform.position + door.gameObject.GetComponent<Collider2D>().bounds.extents.y * 1.2f * Vector3.up, door.currentDoorIndex.ToString(), labelStyle);
        Handles.EndGUI();
    }
}
