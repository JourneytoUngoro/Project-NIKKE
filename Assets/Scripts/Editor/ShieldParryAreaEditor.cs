using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShieldParryArea))]
public class ShieldParryComponentEditor : PropertyDrawer
{
    private SerializedProperty shieldParryType;
    private SerializedProperty changeToShield;
    private SerializedProperty parryTime;
    private SerializedProperty parryDurationTime;
    private SerializedProperty parryTimeDecrementReset;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        shieldParryType = property.FindPropertyRelative("<shieldParryType>k__BackingField");
        changeToShield = property.FindPropertyRelative("<changeToShield>k__BackingField");
        parryTime = property.FindPropertyRelative("<parryTime>k__BackingField");
        parryDurationTime = property.FindPropertyRelative("<parryDurationTime>k__BackingField");
        parryTimeDecrementReset = property.FindPropertyRelative("<parryTimeDecrementReset>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), shieldParryType, new GUIContent("Shield/Parry Type"));

            if ((shieldParryType.intValue).Equals((int)ShieldParryType.Parry))
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), changeToShield, new GUIContent("Change To Shield"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), parryTime, new GUIContent("Parry Time"));
                if (parryTime.isExpanded)
                {
                    position.y += newLineHeight * 2.5f;
                    position.y += newLineHeight * Mathf.Max(parryTime.arraySize - 1, 0);
                }

                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), parryDurationTime, new GUIContent("Parry Duration Time"));
                if (parryDurationTime.isExpanded)
                {
                    position.y += newLineHeight * 2.5f;
                    position.y += newLineHeight * Mathf.Max(parryDurationTime.arraySize - 1, 0);
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), parryTimeDecrementReset, new GUIContent("Parry Time Decrement Reset"));
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        shieldParryType = property.FindPropertyRelative("<shieldParryType>k__BackingField");
        parryTime = property.FindPropertyRelative("<parryTime>k__BackingField");
        parryDurationTime = property.FindPropertyRelative("<parryDurationTime>k__BackingField");

        if (!property.isExpanded)
        {
            return newLineHeight;
        }
        else
        {
            if ((shieldParryType.intValue).Equals((int)ShieldParryType.Parry))
            {
                float lineCount = 6;

                if (parryTime.isExpanded)
                {
                    lineCount += 2.5f;
                    lineCount += Mathf.Max(parryTime.arraySize - 1, 0);
                }

                if (parryDurationTime.isExpanded)
                {
                    lineCount += 2.5f;
                    lineCount += Mathf.Max(parryDurationTime.arraySize - 1, 0);
                }

                return newLineHeight * lineCount;
            }
            else
            {
                return newLineHeight * 2.0f;
            }
        }
    }
}
