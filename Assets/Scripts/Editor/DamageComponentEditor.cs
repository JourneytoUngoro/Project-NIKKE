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
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight * 1.1f;
        baseHealthDamage = property.FindPropertyRelative("baseHealthDamage");
        basePostureDamage = property.FindPropertyRelative("basePostureDamage");
        healthDamageIncreaseByLevel = property.FindPropertyRelative("healthDamageIncreaseByLevel");
        postureDamageIncreaseByLevel = property.FindPropertyRelative("postureDamageIncreaseByLevel");
        pauseTimeWhenHit = property.FindPropertyRelative("pauseTimeWhenHit");

        canBeShielded = property.FindPropertyRelative("canBeShielded");
        healthDamageShieldRate = property.FindPropertyRelative("healthDamageShieldRate");
        postureDamageShieldRate = property.FindPropertyRelative("postureDamageShieldRate");
        pauseTimeWhenShielded = property.FindPropertyRelative("pauseTimeWhenShielded");

        canBeParried = property.FindPropertyRelative("canBeParried");
        healthDamageParryRate = property.FindPropertyRelative("healthDamageParryRate");
        postureDamageParryRate = property.FindPropertyRelative("postureDamageParryRate");
        healthCounterDamageRate = property.FindPropertyRelative("healthCounterDamageRate");
        postureCounterDamageRate = property.FindPropertyRelative("postureCounterDamageRate");
        pauseTimeWhenParried = property.FindPropertyRelative("pauseTimeWhenParried");

        Rect labelText = new Rect(position.min.x, position.min.y, position.size.x, lineHeight);
        property.isExpanded = EditorGUI.Foldout(labelText, property.isExpanded, label);

        if (property.isExpanded)
        {
            Rect drawArea = new Rect(position.min.x, position.min.y + newLineHeight, position.size.x, lineHeight);
            EditorGUI.PropertyField(drawArea, baseHealthDamage, new GUIContent("Health Damage"));
            drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 2, position.size.x, lineHeight);
            EditorGUI.PropertyField(drawArea, healthDamageIncreaseByLevel, new GUIContent("Health Damage Increase By Level"));
            drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 3, position.size.x, lineHeight);
            EditorGUI.PropertyField(drawArea, basePostureDamage, new GUIContent("Posture Damage"));
            drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 4, position.size.x, lineHeight);
            EditorGUI.PropertyField(drawArea, postureDamageIncreaseByLevel, new GUIContent("Posture Damage Increase By Level"));
            drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 5, position.size.x, lineHeight);
            EditorGUI.PropertyField(drawArea, pauseTimeWhenHit, new GUIContent("Pause Time When Hit"));

            drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 7, position.size.x, lineHeight);
            EditorGUI.PropertyField(drawArea, canBeShielded, new GUIContent("Can Be Shielded"));

            if (canBeShielded.boolValue)
            {
                drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 8, position.size.x, lineHeight);
                EditorGUI.PropertyField(drawArea, healthDamageShieldRate, new GUIContent("Health Damage Shield Rate"));
                drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 9, position.size.x, lineHeight);
                EditorGUI.PropertyField(drawArea, postureDamageShieldRate, new GUIContent("Posture Damage Shield Rate"));
                drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 10, position.size.x, lineHeight);
                EditorGUI.PropertyField(drawArea, pauseTimeWhenShielded, new GUIContent("Pause Time When Shielded"));

                drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 12, position.size.x, lineHeight);
                EditorGUI.PropertyField(drawArea, canBeParried, new GUIContent("Can Be Parried"));
                if (canBeParried.boolValue)
                {
                    drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 13, position.size.x, lineHeight);
                    EditorGUI.PropertyField(drawArea, healthDamageParryRate, new GUIContent("Health Damage Parry Rate"));
                    drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 14, position.size.x, lineHeight);
                    EditorGUI.PropertyField(drawArea, postureDamageParryRate, new GUIContent("Posture Damage Parry Rate"));
                    drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 15, position.size.x, lineHeight);
                    EditorGUI.PropertyField(drawArea, healthCounterDamageRate, new GUIContent("Health Counter Damage Rate"));
                    drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 16, position.size.x, lineHeight);
                    EditorGUI.PropertyField(drawArea, postureCounterDamageRate, new GUIContent("Posture Counter Damage Rate"));
                    drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 17, position.size.x, lineHeight);
                    EditorGUI.PropertyField(drawArea, pauseTimeWhenParried, new GUIContent("Pause Time When Parried"));
                }
            }
            else
            {
                drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 8, position.size.x, lineHeight);
                EditorGUI.PropertyField(drawArea, canBeParried, new GUIContent("Can Be Parried"));

                if (canBeParried.boolValue)
                {
                    if (canBeParried.boolValue)
                    {
                        drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 9, position.size.x, lineHeight);
                        EditorGUI.PropertyField(drawArea, healthDamageParryRate, new GUIContent("Health Damage Parry Rate"));
                        drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 10, position.size.x, lineHeight);
                        EditorGUI.PropertyField(drawArea, postureDamageParryRate, new GUIContent("Posture Damage Parry Rate"));
                        drawArea = new Rect(position.min.x, position.min.y + newLineHeight * 11, position.size.x, lineHeight);
                        EditorGUI.PropertyField(drawArea, pauseTimeWhenParried, new GUIContent("Pause Time When Parried"));
                    }
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight * 1.1f;

        canBeShielded = property.FindPropertyRelative("canBeShielded");
        canBeParried = property.FindPropertyRelative("canBeParried");
        
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
                    return newLineHeight * 12.0f;
                }
                else
                {
                    return newLineHeight * 9.0f;
                }
            }
        }
    }
}
