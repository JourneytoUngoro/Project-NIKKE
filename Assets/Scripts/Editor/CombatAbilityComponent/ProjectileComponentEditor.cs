using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ProjectileComponent))]
public class ProjectileComponentEditor : PropertyDrawer
{
    private SerializedProperty projectilePrefab;
    private SerializedProperty singleTarget;
    private SerializedProperty multipleTarget;
    private SerializedProperty identicalTargetHit;
    private SerializedProperty targetTypePriority;
    private SerializedProperty targetTypePriorities;
    private SerializedProperty checkProjectileRoute;
    private SerializedProperty autoManualDirection;

    private bool toggle = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        projectilePrefab = property.FindPropertyRelative("<projectilePrefab>k__BackingField");
        singleTarget = property.FindPropertyRelative("<singleTarget>k__BackingField");
        multipleTarget = property.FindPropertyRelative("<multipleTarget>k__BackingField");
        identicalTargetHit = property.FindPropertyRelative("<identicalTargetHit>k__BackingField");
        targetTypePriority = property.FindPropertyRelative("<targetTypePriority>k__BackingField");
        targetTypePriorities = property.FindPropertyRelative("<targetTypePriorities>k__BackingField");
        checkProjectileRoute = property.FindPropertyRelative("<checkProjectileRoute>k__BackingField");
        autoManualDirection = property.FindPropertyRelative("<autoManualDirection>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, singleLineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), projectilePrefab, new GUIContent("Projectile / Explosion Prefab"));

            GameObject prefab = (projectilePrefab.objectReferenceValue as GameObject);
            Projectile projectile = prefab?.GetComponent<Projectile>();

            if (projectile != null)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), checkProjectileRoute, new GUIContent("Check Projectile Route"));

                if (projectile.GetProjectileType() == ProjectileType.Bazier || projectile.GetProjectileType() == ProjectileType.Throw || projectile.GetProjectileType() == ProjectileType.Follow || (projectile.GetProjectileType() == ProjectileType.Straight && projectile.GetStraightProjectileDirection() == StraightProjectileDirection.Toward))
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), singleTarget, new GUIContent("Single Target"));
                    EditorGUI.PropertyField(new Rect(position.x + position.size.x / 2.0f, position.y, position.size.x, singleLineHeight), multipleTarget, new GUIContent("Multiple Target"));

                    position.y += newLineHeight;
                    if (singleTarget.boolValue)
                    {
                        if (toggle)
                        {
                            multipleTarget.boolValue = false;
                            toggle = false;
                        }
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), targetTypePriority, new GUIContent("Target Type Priority"));
                    }
                    else
                    {
                        multipleTarget.boolValue = true;
                    }

                    if (multipleTarget.boolValue)
                    {
                        toggle = true;
                        singleTarget.boolValue = false;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), identicalTargetHit, new GUIContent("Identical Target Hit"));
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), targetTypePriorities, new GUIContent("Target Type Priorities"));
                    }
                    else
                    {
                        singleTarget.boolValue = true;
                    }
                }
                else if (projectile.GetProjectileType() == ProjectileType.Straight && projectile.GetStraightProjectileDirection() == StraightProjectileDirection.Manual)
                {
                    singleTarget.boolValue = false;
                    multipleTarget.boolValue = false;

                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), autoManualDirection, new GUIContent("Auto Manual Direction"));
                }
                else if (projectile.GetProjectileType() == ProjectileType.Straight && projectile.GetStraightProjectileDirection() == StraightProjectileDirection.Forward)
                {
                    singleTarget.boolValue = false;
                    multipleTarget.boolValue = false;
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        float multiplier = 1;

        projectilePrefab = property.FindPropertyRelative("<projectilePrefab>k__BackingField");
        singleTarget = property.FindPropertyRelative("<singleTarget>k__BackingField");
        multipleTarget = property.FindPropertyRelative("<multipleTarget>k__BackingField");
        GameObject prefab = (projectilePrefab.objectReferenceValue as GameObject);
        Projectile projectile = prefab?.GetComponent<Projectile>();
        targetTypePriorities = property.FindPropertyRelative("<targetTypePriorities>k__BackingField");

        if (property.isExpanded)
        {
            if (projectile != null)
            {
                multiplier += 1;

                if (multipleTarget.boolValue)
                {
                    multiplier += 1;
                }

                if (projectile.GetProjectileType() == ProjectileType.Bazier || projectile.GetProjectileType() == ProjectileType.Throw || projectile.GetProjectileType() == ProjectileType.Follow || (projectile.GetProjectileType() == ProjectileType.Straight && projectile.GetStraightProjectileDirection() == StraightProjectileDirection.Toward))
                {
                    multiplier += 3;

                    if (multipleTarget.boolValue && targetTypePriorities.isExpanded)
                    {
                        multiplier += 1.5f;
                        multiplier += targetTypePriorities.arraySize == 0 ? 1 : targetTypePriorities.arraySize;
                    }
                }
                else if (projectile.GetProjectileType() == ProjectileType.Straight && projectile.GetStraightProjectileDirection() == StraightProjectileDirection.Manual)
                {
                    multiplier += 2;
                }
                else
                {
                    multiplier += 1;
                }
            }
            else
            {
                multiplier += 1;
            }
        }

        return multiplier * newLineHeight;
    }
}
