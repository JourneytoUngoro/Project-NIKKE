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
    private SerializedProperty limitAngle;
    private SerializedProperty clockwiseAngle;
    private SerializedProperty counterClockwiseAngle;
    private float vector2BoudaryWidth = 345.0f;

    private bool toggle = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        overlapBox = property.FindPropertyRelative("overlapBox");
        overlapCircle = property.FindPropertyRelative("overlapCircle");
        boxSize = property.FindPropertyRelative("boxSize");
        circleRadius = property.FindPropertyRelative("circleRadius");
        limitAngle = property.FindPropertyRelative("limitAngle");
        clockwiseAngle = property.FindPropertyRelative("clockwiseAngle");
        counterClockwiseAngle = property.FindPropertyRelative("counterClockwiseAngle");

        Rect labelText = new Rect(position.min.x, position.min.y, position.size.x, lineHeight);
        property.isExpanded = EditorGUI.Foldout(labelText, property.isExpanded, label);

        if (property.isExpanded)
        {
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

            if (overlapCircle.boolValue != false || overlapBox.boolValue != false)
            {
                Rect limitAngleRect;

                if (overlapBox.boolValue)
                {
                    if (EditorGUIUtility.currentViewWidth > vector2BoudaryWidth)
                    {
                        limitAngleRect = new Rect(position.min.x, position.min.y + lineHeight * 3.0f, position.size.x, lineHeight);
                    }
                    else
                    {
                        limitAngleRect = new Rect(position.min.x, position.min.y + lineHeight * 4.0f, position.size.x, lineHeight);
                    }

                    EditorGUI.PropertyField(limitAngleRect, limitAngle, new GUIContent("Limit Angle"));

                    if (limitAngle.boolValue)
                    {
                        if (EditorGUIUtility.currentViewWidth > vector2BoudaryWidth)
                        {
                            Rect drawRect = new Rect(position.min.x, position.min.y + lineHeight * 4.0f, position.size.x, lineHeight);
                            EditorGUI.PropertyField(drawRect, clockwiseAngle, new GUIContent("Clockwise Angle"));
                            drawRect = new Rect(position.min.x, position.min.y + lineHeight * 5.0f, position.size.x, lineHeight);
                            EditorGUI.PropertyField(drawRect, counterClockwiseAngle, new GUIContent("Counter Clockwise Angle"));
                        }
                        else
                        {
                            Rect drawRect = new Rect(position.min.x, position.min.y + lineHeight * 5.0f, position.size.x, lineHeight);
                            EditorGUI.PropertyField(drawRect, clockwiseAngle, new GUIContent("Clockwise Angle"));
                            drawRect = new Rect(position.min.x, position.min.y + lineHeight * 6.0f, position.size.x, lineHeight);
                            EditorGUI.PropertyField(drawRect, counterClockwiseAngle, new GUIContent("Counter Clockwise Angle"));
                        }
                    }
                }
                else if (overlapCircle.boolValue)
                {
                    limitAngleRect = new Rect(position.min.x, position.min.y + lineHeight * 3.0f, position.size.x, lineHeight);
                    EditorGUI.PropertyField(limitAngleRect, limitAngle, new GUIContent("Limit Angle"));

                    if (limitAngle.boolValue)
                    {
                        Rect drawRect = new Rect(position.min.x, position.min.y + lineHeight * 4.0f, position.size.x, lineHeight);
                        EditorGUI.PropertyField(drawRect, clockwiseAngle, new GUIContent("Clockwise Angle"));
                        drawRect = new Rect(position.min.x, position.min.y + lineHeight * 5.0f, position.size.x, lineHeight);
                        EditorGUI.PropertyField(drawRect, counterClockwiseAngle, new GUIContent("Counter Clockwise Angle"));
                    }
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        overlapBox = property.FindPropertyRelative("overlapBox");
        overlapCircle = property.FindPropertyRelative("overlapCircle");
        limitAngle = property.FindPropertyRelative("limitAngle");

        if (!property.isExpanded)
        {
            return lineHeight;
        }
        else
        {
            if (overlapBox.boolValue)
            {
                if (EditorGUIUtility.currentViewWidth > vector2BoudaryWidth)
                {
                    if (limitAngle.boolValue)
                    {
                        return lineHeight * 6.0f;
                    }
                    else
                    {
                        return lineHeight * 4.0f;
                    }
                }
                else
                {
                    if (limitAngle.boolValue)
                    {
                        return lineHeight * 7.0f;
                    }
                    else
                    {
                        return lineHeight * 5.0f;
                    }
                }
            }

            if (overlapCircle.boolValue)
            {
                if (limitAngle.boolValue)
                {
                    return lineHeight * 6.0f;
                }
                else
                {
                    return lineHeight * 4.0f;
                }
            }

            return lineHeight * 2.0f;
        }

    }
}
