using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ProjectileComponent))]
public class ProjectileComponentEditor : PropertyDrawer
{
    private SerializedProperty projectilePrefab;
    private SerializedProperty checkProjectileRoute;
    private SerializedProperty autoManualDirection;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        projectilePrefab = property.FindPropertyRelative("<projectilePrefab>k__BackingField");
        checkProjectileRoute = property.FindPropertyRelative("<checkProjectileRoute>k__BackingField");
        autoManualDirection = property.FindPropertyRelative("<autoManualDirection>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), projectilePrefab, new GUIContent("Projectile / Explosion Prefab"));

            GameObject prefab = (projectilePrefab.objectReferenceValue as GameObject);
            Projectile projectile = prefab?.GetComponent<Projectile>();

            if (projectile != null)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), checkProjectileRoute, new GUIContent("Check Projectile Route"));

                if (projectile.GetProjectileType() == ProjectileType.Straight && projectile.GetStraightProjectileDirection() == StraightProjectileDirection.Manual)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), autoManualDirection, new GUIContent("Auto Manual Direction"));
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        projectilePrefab = property.FindPropertyRelative("<projectilePrefab>k__BackingField");
        GameObject prefab = (projectilePrefab.objectReferenceValue as GameObject);
        Projectile projectile = prefab?.GetComponent<Projectile>();

        if (!property.isExpanded)
        {
            return newLineHeight;
        }
        else
        {
            if (projectile != null)
            {
                if (projectile.GetProjectileType() == ProjectileType.Straight && projectile.GetStraightProjectileDirection() == StraightProjectileDirection.Manual)
                {
                    return newLineHeight * 4.0f;
                }
                else
                {
                    return newLineHeight * 3.0f;
                }
            }
            else
            {
                return newLineHeight * 2.0f;
            }
        }
    }
}
