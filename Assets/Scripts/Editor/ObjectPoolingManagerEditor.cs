using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectPoolingManager))]
public class ObjectPoolingManagerEditor : Editor
{
    private ObjectPoolingManager objectPoolingManager;

    private void OnEnable()
    {
        objectPoolingManager = target as ObjectPoolingManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        foreach (ObjectInfo objectInfo in objectPoolingManager.objectInfos)
        {
            if (objectInfo.prefab != null)
            {
                objectInfo.objectName = objectInfo.prefab.name;
            }
        }
    }
}
