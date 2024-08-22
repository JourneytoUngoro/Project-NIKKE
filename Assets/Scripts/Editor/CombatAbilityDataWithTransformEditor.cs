using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CombatAbilityDataWithTransform))]
public class CombatAbilityDataWithTransformEditor : PropertyDrawer
{
    private SerializedProperty name;
    private SerializedProperty centerTransform;
    private SerializedProperty combatAbilityData;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        name = property.FindPropertyRelative("name");
        centerTransform = property.FindPropertyRelative("centerTransform");
        combatAbilityData = property.FindPropertyRelative("combatAbilityData");

        if (combatAbilityData.objectReferenceValue != null)
        {
            name.stringValue = combatAbilityData.objectReferenceValue.name;
        }
        else
        {
            name.stringValue = "Combat Ability Data";
        }

        Rect labelRect = new Rect(position.min.x, position.min.y, position.size.x, lineHeight);
        property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, new GUIContent(name.stringValue));

        if (property.isExpanded)
        {
            Rect centerTransformRect = new Rect(position.min.x, position.min.y + lineHeight, position.size.x, lineHeight);
            EditorGUI.PropertyField(centerTransformRect, centerTransform, new GUIContent("Center Transform"));
        
            Rect combatAbilityDataRect = new Rect(position.min.x, position.min.y + lineHeight * 2.0f, position.size.x, lineHeight);
            EditorGUI.PropertyField(combatAbilityDataRect, combatAbilityData, new GUIContent("Combat Ability Data"));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        return property.isExpanded ? lineHeight * 3.0f : lineHeight;
    }
}
