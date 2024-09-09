using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CombatAbilityWithTransform))]
public class CombatAbilityWithTransformEditor : PropertyDrawer
{
    private SerializedProperty name;
    private SerializedProperty centerTransform;
    private SerializedProperty overlapCollider;
    private SerializedProperty combatAbilityData;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        name = property.FindPropertyRelative("name");
        overlapCollider = property.FindPropertyRelative("overlapCollider");
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
            
            Rect overlapColliderRect = new Rect(position.min.x, position.min.y + lineHeight * 2.0f, position.size.x, lineHeight);
            EditorGUI.PropertyField(overlapColliderRect, overlapCollider, new GUIContent("Overlap Collider"));
        
            Rect combatAbilityDataRect = new Rect(position.min.x, position.min.y + EditorGUI.GetPropertyHeight(overlapCollider) + lineHeight * 2.0f, position.size.x, lineHeight);
            EditorGUI.PropertyField(combatAbilityDataRect, combatAbilityData, new GUIContent("Combat Ability Data"));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        overlapCollider = property.FindPropertyRelative("overlapCollider");

        if (property.isExpanded)
        {
            return EditorGUI.GetPropertyHeight(overlapCollider) + lineHeight * 3.0f;
        }
        else
        {
            return lineHeight;
        }
    }
}
