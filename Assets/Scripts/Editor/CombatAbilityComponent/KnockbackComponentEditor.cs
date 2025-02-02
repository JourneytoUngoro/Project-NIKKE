using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(KnockbackComponent))]
public class KnockbackComponentEditor : PropertyDrawer
{
    private SerializedProperty entityProtruded;
    private SerializedProperty directionBase;
    private SerializedProperty knockbackDirection;
    private SerializedProperty knockbackSpeed;
    private SerializedProperty knockbackTime;
    private SerializedProperty easeFunction;
    private SerializedProperty isKnockbackDifferentWhenAerial;
    private SerializedProperty knockbackDirectionWhenAerial;
    private SerializedProperty knockbackSpeedWhenAerial;
    private SerializedProperty knockbackTimeWhenAerial;
    private SerializedProperty easeFunctionWhenAerial;

    private SerializedProperty canBeShielded;
    private SerializedProperty knockbackDirectionWhenShielded;
    private SerializedProperty knockbackSpeedWhenShielded;
    private SerializedProperty knockbackTimeWhenShielded;
    private SerializedProperty easeFunctionWhenShielded;
    private SerializedProperty isKnockbackDifferentWhenAerialShielded;
    private SerializedProperty knockbackDirectionWhenAerialShielded;
    private SerializedProperty knockbackSpeedWhenAerialShielded;
    private SerializedProperty knockbackTimeWhenAerialShielded;
    private SerializedProperty easeFunctionWhenAerialShielded;

    private SerializedProperty canBeParried;
    private SerializedProperty knockbackDirectionWhenParried;
    private SerializedProperty knockbackSpeedWhenParried;
    private SerializedProperty knockbackTimeWhenParried;
    private SerializedProperty easeFunctionWhenParried;
    private SerializedProperty counterProtrudedWhenParried;
    private SerializedProperty counterKnockbackDirectionWhenParried;
    private SerializedProperty counterKnockbackSpeedWhenParried;
    private SerializedProperty counterKnockbackTimeWhenParried;
    private SerializedProperty counterEaseFunctionWhenParried;
    private SerializedProperty isKnockbackDifferentWhenAerialParried;
    private SerializedProperty knockbackDirectionWhenAerialParried;
    private SerializedProperty knockbackSpeedWhenAerialParried;
    private SerializedProperty knockbackTimeWhenAerialParried;
    private SerializedProperty easeFunctionWhenAerialParried;
    private SerializedProperty counterProtrudedWhenAerialParried;
    private SerializedProperty counterKnockbackDirectionWhenAerialParried;
    private SerializedProperty counterKnockbackSpeedWhenAerialParried;
    private SerializedProperty counterKnockbackTimeWhenAerialParried;
    private SerializedProperty counterEaseFunctionWhenAerialParried;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        entityProtruded = property.FindPropertyRelative("<entityProtruded>k__BackingField");
        directionBase = property.FindPropertyRelative("<directionBase>k__BackingField");
        knockbackDirection = property.FindPropertyRelative("<knockbackDirection>k__BackingField");
        knockbackSpeed = property.FindPropertyRelative("<knockbackSpeed>k__BackingField");
        knockbackTime = property.FindPropertyRelative("<knockbackTime>k__BackingField");
        easeFunction = property.FindPropertyRelative("<easeFunction>k__BackingField");
        isKnockbackDifferentWhenAerial = property.FindPropertyRelative("<isKnockbackDifferentWhenAerial>k__BackingField");
        knockbackDirectionWhenAerial = property.FindPropertyRelative("<knockbackDirectionWhenAerial>k__BackingField");
        knockbackSpeedWhenAerial = property.FindPropertyRelative("<knockbackSpeedWhenAerial>k__BackingField");
        knockbackTimeWhenAerial = property.FindPropertyRelative("<knockbackTimeWhenAerial>k__BackingField");
        easeFunctionWhenAerial = property.FindPropertyRelative("<easeFunctionWhenAerial>k__BackingField");

        canBeShielded = property.FindPropertyRelative("<canBeShielded>k__BackingField");
        knockbackDirectionWhenShielded = property.FindPropertyRelative("<knockbackDirectionWhenShielded>k__BackingField");
        knockbackSpeedWhenShielded = property.FindPropertyRelative("<knockbackSpeedWhenShielded>k__BackingField");
        knockbackTimeWhenShielded = property.FindPropertyRelative("<knockbackTimeWhenShielded>k__BackingField");
        easeFunctionWhenShielded = property.FindPropertyRelative("<easeFunctionWhenShielded>k__BackingField");
        isKnockbackDifferentWhenAerialShielded = property.FindPropertyRelative("<isKnockbackDifferentWhenAerialShielded>k__BackingField");
        knockbackDirectionWhenAerialShielded = property.FindPropertyRelative("<knockbackDirectionWhenAerialShielded>k__BackingField");
        knockbackSpeedWhenAerialShielded = property.FindPropertyRelative("<knockbackSpeedWhenAerialShielded>k__BackingField");
        knockbackTimeWhenAerialShielded = property.FindPropertyRelative("<knockbackTimeWhenAerialShielded>k__BackingField");
        easeFunctionWhenAerialShielded = property.FindPropertyRelative("<easeFunctionWhenAerialShielded>k__BackingField");

        canBeParried = property.FindPropertyRelative("<canBeParried>k__BackingField");
        knockbackDirectionWhenParried = property.FindPropertyRelative("<knockbackDirectionWhenParried>k__BackingField");
        knockbackSpeedWhenParried = property.FindPropertyRelative("<knockbackSpeedWhenParried>k__BackingField");
        knockbackTimeWhenParried = property.FindPropertyRelative("<knockbackTimeWhenParried>k__BackingField");
        easeFunctionWhenParried = property.FindPropertyRelative("<easeFunctionWhenParried>k__BackingField");
        counterProtrudedWhenParried = property.FindPropertyRelative("<counterProtrudedWhenParried>k__BackingField");
        counterKnockbackDirectionWhenParried = property.FindPropertyRelative("<counterKnockbackDirectionWhenParried>k__BackingField");
        counterKnockbackSpeedWhenParried = property.FindPropertyRelative("<counterKnockbackSpeedWhenParried>k__BackingField");
        counterKnockbackTimeWhenParried = property.FindPropertyRelative("<counterKnockbackTimeWhenParried>k__BackingField");
        counterEaseFunctionWhenParried = property.FindPropertyRelative("<counterEaseFunctionWhenParried>k__BackingField");
        isKnockbackDifferentWhenAerialParried = property.FindPropertyRelative("<isKnockbackDifferentWhenAerialParried>k__BackingField");
        knockbackDirectionWhenAerialParried = property.FindPropertyRelative("<knockbackDirectionWhenAerialParried>k__BackingField");
        knockbackSpeedWhenAerialParried = property.FindPropertyRelative("<knockbackSpeedWhenAerialParried>k__BackingField");
        knockbackTimeWhenAerialParried = property.FindPropertyRelative("<knockbackTimeWhenAerialParried>k__BackingField");
        easeFunctionWhenAerialParried = property.FindPropertyRelative("<easeFunctionWhenAerialParried>k__BackingField");
        counterProtrudedWhenAerialParried = property.FindPropertyRelative("<counterProtrudedWhenAerialParried>k__BackingField");
        counterKnockbackDirectionWhenAerialParried = property.FindPropertyRelative("<counterKnockbackDirectionWhenAerialParried>k__BackingField");
        counterKnockbackSpeedWhenAerialParried = property.FindPropertyRelative("<counterKnockbackSpeedWhenAerialParried>k__BackingField");
        counterKnockbackTimeWhenAerialParried = property.FindPropertyRelative("<counterKnockbackTimeWhenAerialParried>k__BackingField");
        counterEaseFunctionWhenAerialParried = property.FindPropertyRelative("<counterEaseFunctionWhenAerialParried>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), entityProtruded, new GUIContent("Entity Protruded"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), directionBase, new GUIContent("Direction Base"));
            
            if (directionBase.intValue != (int)DirectionBase.Relative)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirection, new GUIContent("Knockback Direction"));
            }

            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeed, new GUIContent("Knockback Speed"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTime, new GUIContent("Knockback Time"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunction, new GUIContent("Ease Function"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), isKnockbackDifferentWhenAerial, new GUIContent("Is Knockback Different When Aerial"));
            if (isKnockbackDifferentWhenAerial.boolValue)
            {
                if (directionBase.intValue != (int)DirectionBase.Relative)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenAerial, new GUIContent("Knockback Direction When Aerial"));
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenAerial, new GUIContent("Knockback Speed When Aerial"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenAerial, new GUIContent("Knockback Time When Aerial"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenAerial, new GUIContent("Ease Function When Aerial"));
            }


            position.y += 2.0f * newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), canBeShielded, new GUIContent("Can Be Shielded"));

            if (canBeShielded.boolValue)
            {
                if (directionBase.intValue != (int)DirectionBase.Relative)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenShielded, new GUIContent("Knockback Direction When Shielded"));
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenShielded, new GUIContent("Knockback Speed When Shielded"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenShielded, new GUIContent("Knockback Time When Shielded"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenShielded, new GUIContent("Ease Function When Shielded"));

                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), isKnockbackDifferentWhenAerialShielded, new GUIContent("Is Knockback Different When Aerial Shielded"));
                if (isKnockbackDifferentWhenAerialShielded.boolValue)
                {
                    if (directionBase.intValue != (int)DirectionBase.Relative)
                    {
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenAerialShielded, new GUIContent("Knockback Direction When Aerial Shielded"));
                    }
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenAerialShielded, new GUIContent("Knockback Speed When Aerial Shielded"));
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenAerialShielded, new GUIContent("Knockback Time When Aerial Shielded"));
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenAerialShielded, new GUIContent("Ease Function When Aerial Shielded"));
                }
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
                if (directionBase.intValue != (int)DirectionBase.Relative)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenParried, new GUIContent("Knockback Direction When Parried"));
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenParried, new GUIContent("Knockback Speed When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenParried, new GUIContent("Knockback Time When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenParried, new GUIContent("Ease Function When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterProtrudedWhenParried, new GUIContent("Counter Protruded When Parried"));
                if (directionBase.intValue != (int)DirectionBase.Relative)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackDirectionWhenParried, new GUIContent("Counter Knockback Direction When Parried"));
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackSpeedWhenParried, new GUIContent("Counter Knockback Speed When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackTimeWhenParried, new GUIContent("Counter Knockback Time When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterEaseFunctionWhenParried, new GUIContent("Counter Ease Function When Parried"));

                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), isKnockbackDifferentWhenAerialParried, new GUIContent("Is Knockback Different When Aerial Parried"));
                if (isKnockbackDifferentWhenAerialParried.boolValue)
                {
                    if (directionBase.intValue != (int)DirectionBase.Relative)
                    {
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenAerialParried, new GUIContent("Knockback Direction When Aerial Parried"));
                    }
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenAerialParried, new GUIContent("Knockback Speed When Aerial Parried"));
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenAerialParried, new GUIContent("Knockback Time When Aerial Parried"));
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenAerialParried, new GUIContent("Ease Function When Aerial Parried"));
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterProtrudedWhenAerialParried, new GUIContent("Counter Protruded When Aerial Parried"));
                    if (directionBase.intValue != (int)DirectionBase.Relative)
                    {
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackDirectionWhenAerialParried, new GUIContent("Counter Knockback Direction When Aerial Parried"));
                    }
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackSpeedWhenAerialParried, new GUIContent("Counter Knockback Speed When Aerial Parried"));
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackTimeWhenAerialParried, new GUIContent("Counter Knockback Time When Aerial Parried"));
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterEaseFunctionWhenAerialParried, new GUIContent("Counter Ease Function When Aerial Parried"));
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int multiplier = 1;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        directionBase = property.FindPropertyRelative("<directionBase>k__BackingField");
        isKnockbackDifferentWhenAerial = property.FindPropertyRelative("<isKnockbackDifferentWhenAerial>k__BackingField");
        canBeShielded = property.FindPropertyRelative("<canBeShielded>k__BackingField");
        isKnockbackDifferentWhenAerialShielded = property.FindPropertyRelative("<isKnockbackDifferentWhenAerialShielded>k__BackingField");
        canBeParried = property.FindPropertyRelative("<canBeParried>k__BackingField");
        isKnockbackDifferentWhenAerialParried = property.FindPropertyRelative("<isKnockbackDifferentWhenAerialParried>k__BackingField");

        if (property.isExpanded)
        {
            multiplier += 10;

            if (directionBase.intValue == (int)DirectionBase.Relative)
            {
                multiplier -= 1;
            }

            if (isKnockbackDifferentWhenAerial.boolValue)
            {
                multiplier += 4;

                if (directionBase.intValue == (int)DirectionBase.Relative)
                {
                    multiplier -= 1;
                }
            }
            if (canBeShielded.boolValue)
            {
                multiplier += 6;

                if (directionBase.intValue == (int)DirectionBase.Relative)
                {
                    multiplier -= 1;
                }

                if (isKnockbackDifferentWhenAerialShielded.boolValue)
                {
                    multiplier += 4;

                    if (directionBase.intValue == (int)DirectionBase.Relative)
                    {
                        multiplier -= 1;
                    }
                }
            }

            if (canBeParried.boolValue)
            {
                multiplier += 10;

                if (directionBase.intValue == (int)DirectionBase.Relative)
                {
                    multiplier -= 2;
                }

                if (isKnockbackDifferentWhenAerialParried.boolValue)
                {
                    multiplier += 9;

                    if (directionBase.intValue == (int)DirectionBase.Relative)
                    {
                        multiplier -= 2;
                    }
                }
            }
        }

        return multiplier * newLineHeight;
    }
}
