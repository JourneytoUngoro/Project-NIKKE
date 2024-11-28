using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomPropertyDrawer(typeof(CombatAbilityWithTransform))]
public class CombatAbilityWithTransformEditor : PropertyDrawer
{
    private SerializedProperty name;
    private SerializedProperty overlapColliders;
    private SerializedProperty combatAbilityData;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        name = property.FindPropertyRelative("name");
        overlapColliders = property.FindPropertyRelative("<overlapColliders>k__BackingField");
        combatAbilityData = property.FindPropertyRelative("<combatAbilityData>k__BackingField");

        if (combatAbilityData.objectReferenceValue != null)
        {
            name.stringValue = combatAbilityData.objectReferenceValue.name;
        }
        else
        {
            name.stringValue = "Combat Ability Data";
        }

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, singleLineHeight), property.isExpanded, new GUIContent(name.stringValue));

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, singleLineHeight), overlapColliders, new GUIContent("Overlap Collider"));

            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, singleLineHeight), combatAbilityData, new GUIContent("Combat Ability Data"));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        overlapColliders = property.FindPropertyRelative("overlapColliders");

        if (property.isExpanded)
        {
            return EditorGUI.GetPropertyHeight(overlapColliders) + newLineHeight * 3.0f;
        }
        else
        {
            return newLineHeight;
        }
    }
}
