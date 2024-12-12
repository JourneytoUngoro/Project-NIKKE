using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ProjectileComponent))]
public class ProjectileComponentEditor : PropertyDrawer
{
    private SerializedProperty projectileExplosionPrefab;
    private SerializedProperty rotateIdentical;
    private SerializedProperty rotateIndependent;
    private SerializedProperty manualDirectionVector;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        projectileExplosionPrefab = property.FindPropertyRelative("<projectileExplosionPrefab>k__BackingField");
        rotateIdentical = property.FindPropertyRelative("<rotateIdentical>k__BackingField");
        rotateIndependent = property.FindPropertyRelative("<rotateIndependent>k__BackingField");
        manualDirectionVector = property.FindPropertyRelative("<manualDirectionVector>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), projectileExplosionPrefab, new GUIContent("Projectile / Explosion Prefab"));

            GameObject prefab = (projectileExplosionPrefab.objectReferenceValue as GameObject);
            Projectile projectile = prefab.GetComponent<Projectile>();

            if (projectile != null && projectile.GetProjectileType() == ProjectileType.Straight)
            {
                StraightProjectileDirection projectileDirection = projectile.GetStraightProjectileDirection();
                
                switch (projectileDirection)
                {
                    case StraightProjectileDirection.Toward:
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), rotateIdentical, new GUIContent("Rotate Identical"));
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), rotateIndependent, new GUIContent("Rotate Independent"));
                        break;

                    case StraightProjectileDirection.Manual:
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), projectileExplosionPrefab, new GUIContent("Projectile / Explosion Prefab"));
                        break;

                    default: break;
                }
            }
        }
    }
}
