using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OverlapCollider))]
public class OverlapColliderEditor : PropertyDrawer
{
    private SerializedProperty overlapBox;
    private SerializedProperty overlapCircle;
    private SerializedProperty boxSize;
    private SerializedProperty circleRadius;

    private bool toggle = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        overlapBox = property.FindPropertyRelative("overlapBox");
        overlapCircle = property.FindPropertyRelative("overlapCircle");
        boxSize = property.FindPropertyRelative("boxSize");
        circleRadius = property.FindPropertyRelative("circleRadius");

        Rect labelText = new Rect(position.min.x, position.min.y, position.size.x, lineHeight);
        property.isExpanded = EditorGUI.Foldout(labelText, true, label);

        Rect boolSelection = new Rect(position.min.x, position.min.y + lineHeight, position.size.x, lineHeight);
        EditorGUI.PropertyField(boolSelection, overlapBox, new GUIContent("OverlapBox"));
        boolSelection = new Rect(position.min.x + position.size.x / 2.0f, position.min.y + lineHeight, position.size.x, lineHeight);
        EditorGUI.PropertyField(boolSelection, overlapCircle, new GUIContent("OverlapCircle"));

        if (overlapBox.boolValue)
        {
            if (toggle)
            {
                overlapCircle.boolValue = false;
                toggle = false;
            }
            Rect drawArea = new Rect(position.min.x, position.min.y + lineHeight * 2, position.size.x, lineHeight);
            EditorGUI.PropertyField(drawArea, boxSize, new GUIContent("Box Size"));
        }

        if (overlapCircle.boolValue)
        {
            toggle = true;
            overlapBox.boolValue = false;
            Rect drawArea = new Rect(position.min.x, position.min.y + lineHeight * 2, position.size.x, lineHeight);
            EditorGUI.PropertyField(drawArea, circleRadius, new GUIContent("Circle Radius"));
        }

        /*if (!overlapBox.boolValue && !overlapCircle.boolValue)
        {
            EditorGUILayout.HelpBox("Caution: Current Entity will always be considered as not grounded.", MessageType.Warning);
        }*/

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        overlapBox = property.FindPropertyRelative("overlapBox");
        overlapCircle = property.FindPropertyRelative("overlapCircle");

        if (overlapBox.boolValue)
        {
            if (EditorGUIUtility.currentViewWidth > 450.0f)
            {
                return lineHeight * 3.0f;
            }
            else return lineHeight * 4.0f;
        }

        if (overlapCircle.boolValue)
        {
            return lineHeight * 3.0f;
        }

        return lineHeight * 2.0f;
    }
}
