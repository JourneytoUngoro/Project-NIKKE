using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DamageComponent))]
public class DamageComponentEditor : PropertyDrawer
{
    private SerializedProperty baseHealthDamage;
    private SerializedProperty basePostureDamage;
    private SerializedProperty healthDamageIncreaseByLevel;
    private SerializedProperty postureDamageIncreaseByLevel;
    private SerializedProperty pauseTimeWhenHit;

    private SerializedProperty canBeShielded;
    private SerializedProperty healthDamageShieldRate;
    private SerializedProperty postureDamageShieldRate;
    private SerializedProperty pauseTimeWhenShielded;

    private SerializedProperty canBeParried;
    private SerializedProperty healthDamageParryRate;
    private SerializedProperty postureDamageParryRate;
    private SerializedProperty healthCounterDamageRate;
    private SerializedProperty postureCounterDamageRate;
    private SerializedProperty pauseTimeWhenParried;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        baseHealthDamage = property.FindPropertyRelative("<baseHealthDamage>k__BackingField");
        basePostureDamage = property.FindPropertyRelative("<basePostureDamage>k__BackingField");
        healthDamageIncreaseByLevel = property.FindPropertyRelative("<healthDamageIncreaseByLevel>k__BackingField");
        postureDamageIncreaseByLevel = property.FindPropertyRelative("<postureDamageIncreaseByLevel>k__BackingField");
        pauseTimeWhenHit = property.FindPropertyRelative("<pauseTimeWhenHit>k__BackingField");

        canBeShielded = property.FindPropertyRelative("<canBeShielded>k__BackingField");
        healthDamageShieldRate = property.FindPropertyRelative("<healthDamageShieldRate>k__BackingField");
        postureDamageShieldRate = property.FindPropertyRelative("<postureDamageShieldRate>k__BackingField");
        pauseTimeWhenShielded = property.FindPropertyRelative("<pauseTimeWhenShielded>k__BackingField");

        canBeParried = property.FindPropertyRelative("<canBeParried>k__BackingField");
        healthDamageParryRate = property.FindPropertyRelative("<healthDamageParryRate>k__BackingField");
        postureDamageParryRate = property.FindPropertyRelative("<postureDamageParryRate>k__BackingField");
        healthCounterDamageRate = property.FindPropertyRelative("<healthCounterDamageRate>k__BackingField");
        postureCounterDamageRate = property.FindPropertyRelative("<postureCounterDamageRate>k__BackingField");
        pauseTimeWhenParried = property.FindPropertyRelative("<pauseTimeWhenParried>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), baseHealthDamage, new GUIContent("Health Damage"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), healthDamageIncreaseByLevel, new GUIContent("Health Damage Increase By Level"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), basePostureDamage, new GUIContent("Posture Damage"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), postureDamageIncreaseByLevel, new GUIContent("Posture Damage Increase By Level"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), pauseTimeWhenHit, new GUIContent("Pause Time When Hit"));

            position.y += 2.0f * newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), canBeShielded, new GUIContent("Can Be Shielded"));

            if (canBeShielded.boolValue)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), healthDamageShieldRate, new GUIContent("Health Damage Shield Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), postureDamageShieldRate, new GUIContent("Posture Damage Shield Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), pauseTimeWhenShielded, new GUIContent("Pause Time When Shielded"));
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
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), healthDamageParryRate, new GUIContent("Health Damage Parry Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), postureDamageParryRate, new GUIContent("Posture Damage Parry Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), healthCounterDamageRate, new GUIContent("Health Counter Damage Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), postureCounterDamageRate, new GUIContent("Posture Counter Damage Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), pauseTimeWhenParried, new GUIContent("Pause Time When Parried"));
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
                    return newLineHeight * 18.0f;
                }
                else
                {
                    return newLineHeight * 13.0f;
                }
            }
            else
            {
                if (canBeParried.boolValue)
                {
                    return newLineHeight * 14.0f;
                }
                else
                {
                    return newLineHeight * 9.0f;
                }
            }
        }
    }
}
