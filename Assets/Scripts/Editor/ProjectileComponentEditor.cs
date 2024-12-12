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
    private SerializedProperty rotateTransform;
    private SerializedProperty rotatePrefab;
    private SerializedProperty manualDirectionVector;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        projectilePrefab = property.FindPropertyRelative("<projectileEPrefab>k__BackingField");
        checkProjectileRoute = property.FindPropertyRelative("<checkProjectileRoute>k__BackingField>");
        rotateTransform = property.FindPropertyRelative("<rotateTransform>k__BackingField");
        rotatePrefab = property.FindPropertyRelative("<rotatePrefabk__BackingField");
        manualDirectionVector = property.FindPropertyRelative("<manualDirectionVector>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), projectilePrefab, new GUIContent("Projectile / Explosion Prefab"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), checkProjectileRoute, new GUIContent("Check Projectile Route"));

            GameObject prefab = (projectilePrefab.objectReferenceValue as GameObject);
            Projectile projectile = prefab?.GetComponent<Projectile>();

            if (projectile != null && projectile.GetProjectileType() == ProjectileType.Straight)
            {
                StraightProjectileDirection projectileDirection = projectile.GetStraightProjectileDirection();
                
                switch (projectileDirection)
                {
                    case StraightProjectileDirection.Toward:
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), rotateTransform, new GUIContent("Rotate Identical"));
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), rotatePrefab, new GUIContent("Rotate Independent"));
                        break;

                    case StraightProjectileDirection.Manual:
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), manualDirectionVector, new GUIContent("Manual Direction Vector"));
                        break;

                    default: break;
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        projectilePrefab = property.FindPropertyRelative("<projectileEPrefab>k__BackingField");
        GameObject prefab = (projectilePrefab.objectReferenceValue as GameObject);
        Projectile projectile = prefab?.GetComponent<Projectile>();

        if (!property.isExpanded)
        {
            return newLineHeight;
        }
        else
        {
            if (projectile != null && projectile.GetProjectileType() == ProjectileType.Straight)
            {
                StraightProjectileDirection projectileDirection = projectile.GetStraightProjectileDirection();

                switch (projectileDirection)
                {
                    case StraightProjectileDirection.Toward:
                        return newLineHeight * 5.0f;

                    case StraightProjectileDirection.Manual:
                        return newLineHeight * 4.0f;

                    default:
                        return newLineHeight * 3.0f;
                }
            }
            else
            {
                return newLineHeight * 3.0f;
            }
        }
    }
}
