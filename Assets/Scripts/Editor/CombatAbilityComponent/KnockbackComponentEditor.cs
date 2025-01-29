using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(KnockbackComponent))]
public class KnockbackComponentEditor : PropertyDrawer
{
    private SerializedProperty knockbackLevel;
    private SerializedProperty directionBase;
    private SerializedProperty knockbackDirection;
    private SerializedProperty knockbackSpeed;
    private SerializedProperty knockbackTime;
    private SerializedProperty easeFunction;

    private SerializedProperty canBeShielded;
    private SerializedProperty knockbackDirectionWhenShielded;
    private SerializedProperty knockbackSpeedWhenShielded;
    private SerializedProperty knockbackTimeWhenShielded;
    private SerializedProperty easeFunctionWhenShielded;

    private SerializedProperty canBeParried;
    private SerializedProperty knockbackDirectionWhenParried;
    private SerializedProperty knockbackSpeedWhenParried;
    private SerializedProperty knockbackTimeWhenParried;
    private SerializedProperty easeFunctionWhenParried;
    private SerializedProperty counterKnockbackDirectionWhenParried;
    private SerializedProperty counterKnockbackSpeedWhenParried;
    private SerializedProperty counterKnockbackTimeWhenParried;
    private SerializedProperty counterEaseFunctionWhenParried;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        knockbackLevel = property.FindPropertyRelative("<knockbackLevel>k__BackingField");
        directionBase = property.FindPropertyRelative("<directionBase>k__BackingField");
        knockbackDirection = property.FindPropertyRelative("<knockbackDirection>k__BackingField");
        knockbackSpeed = property.FindPropertyRelative("<knockbackSpeed>k__BackingField");
        knockbackTime = property.FindPropertyRelative("<knockbackTime>k__BackingField");
        easeFunction = property.FindPropertyRelative("<easeFunction>k__BackingField");

        canBeShielded = property.FindPropertyRelative("<canBeShielded>k__BackingField");
        knockbackDirectionWhenShielded = property.FindPropertyRelative("<knockbackDirectionWhenShielded>k__BackingField");
        knockbackSpeedWhenShielded = property.FindPropertyRelative("<knockbackSpeedWhenShielded>k__BackingField");
        knockbackTimeWhenShielded = property.FindPropertyRelative("<knockbackTimeWhenShielded>k__BackingField");
        easeFunctionWhenShielded = property.FindPropertyRelative("<easeFunctionWhenShielded>k__BackingField");

        canBeParried = property.FindPropertyRelative("<canBeParried>k__BackingField");
        knockbackDirectionWhenParried = property.FindPropertyRelative("<knockbackDirectionWhenParried>k__BackingField");
        knockbackSpeedWhenParried = property.FindPropertyRelative("<knockbackSpeedWhenParried>k__BackingField");
        knockbackTimeWhenParried = property.FindPropertyRelative("<knockbackTimeWhenParried>k__BackingField");
        easeFunctionWhenParried = property.FindPropertyRelative("<easeFunctionWhenParried>k__BackingField");
        counterKnockbackDirectionWhenParried = property.FindPropertyRelative("<counterKnockbackDirectionWhenParried>k__BackingField");
        counterKnockbackSpeedWhenParried = property.FindPropertyRelative("<counterKnockbackSpeedWhenParried>k__BackingField");
        counterKnockbackTimeWhenParried = property.FindPropertyRelative("<counterKnockbackTimeWhenParried>k__BackingField");
        counterEaseFunctionWhenParried = property.FindPropertyRelative("<counterEaseFunctionWhenParried>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackLevel, new GUIContent("Knockback Level"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), directionBase, new GUIContent("Direction Base"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirection, new GUIContent("Knockback Direction"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeed, new GUIContent("Knockback Speed"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTime, new GUIContent("Knockback Time"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunction, new GUIContent("Ease Function"));

            position.y += 2.0f * newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), canBeShielded, new GUIContent("Can Be Shielded"));

            if (canBeShielded.boolValue)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenShielded, new GUIContent("Knockback Direction When Shielded"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenShielded, new GUIContent("Knockback Speed When Shielded"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenShielded, new GUIContent("Knockback Time When Shielded"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenShielded, new GUIContent("Ease Function When Shielded"));
            }

            if (canBeShielded.boolValue)
            {
                position.y += 2.0f * newLineHeight;
            }
            else
            {
                position.y += newLineHeight;
            }
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), canBeParried, new GUIContent("Can Be Parried"));

            if (canBeParried.boolValue)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenParried, new GUIContent("Knockback Direction When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenParried, new GUIContent("Knockback Speed When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenParried, new GUIContent("Knockback Time When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenParried, new GUIContent("Ease Function When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackDirectionWhenParried, new GUIContent("Counter Knockback Direction When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackSpeedWhenParried, new GUIContent("Counter Knockback Speed When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackTimeWhenParried, new GUIContent("Counter Knockback Time When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterEaseFunctionWhenParried, new GUIContent("Counter Ease Function When Parried"));
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        canBeShielded = property.FindPropertyRelative("<canBeShielded>k__BackingField");
        canBeParried = property.FindPropertyRelative("<canBeParried>k__BackingField");

        if (!property.isExpanded)
        {
            return newLineHeight;
        }
        else
        {
            if (canBeShielded.boolValue)
            {
                if (canBeParried.boolValue)
                {
                    return newLineHeight * 23.0f;
                }
                else
                {
                    return newLineHeight * 15.0f;
                }
            }
            else
            {
                if (canBeParried.boolValue)
                {
                    return newLineHeight * 18.0f;
                }
                else
                {
                    return newLineHeight * 10.0f;
                }
            }
        }
    }
}
