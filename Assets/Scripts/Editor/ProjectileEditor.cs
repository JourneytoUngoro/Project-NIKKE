using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    private Projectile projectile;

    private SerializedProperty overlapCollider;
    private SerializedProperty projectileType;
    private SerializedProperty projectileTypeWhenParried;
    private SerializedProperty whatIsGround;
    private SerializedProperty projectileSpeed;
    private SerializedProperty explosionDurationTime;
    private SerializedProperty maximumDeflection;
    private SerializedProperty autoDestroyTime;

    private SerializedProperty pointAXOffsetBase;
    private SerializedProperty pointAXOffsetDeviation;
    private SerializedProperty pointAYOffsetBase;
    private SerializedProperty pointAYOffsetDeviation;
    private SerializedProperty pointBXOffsetBase;
    private SerializedProperty pointBXOffsetDeviation;
    private SerializedProperty pointBYOffsetBase;
    private SerializedProperty pointBYOffsetDeviation;

    private SerializedProperty minStep;
    private SerializedProperty maxStep;
    private SerializedProperty timeToMaxStep;

    private SerializedProperty straightProjectileDirection;
    private SerializedProperty loseSpeedTime;

    private SerializedProperty timeStepValue;
    private SerializedProperty speedStepValue;
    private SerializedProperty minSpeedValue;
    private SerializedProperty targetingSuccessDistance;

    private SerializedProperty combatAbility;

    private void OnEnable()
    {
        projectile = target as Projectile;

        overlapCollider = serializedObject.FindProperty("<overlapCollider>k__BackingField");
        projectileType = serializedObject.FindProperty("projectileType");
        projectileTypeWhenParried = serializedObject.FindProperty("projectileTypeWhenParried");
        whatIsGround = serializedObject.FindProperty("whatIsGround");
        projectileSpeed = serializedObject.FindProperty("projectileSpeed");
        explosionDurationTime = serializedObject.FindProperty("explosionDurationTime");
        maximumDeflection = serializedObject.FindProperty("maximumDeflection");
        autoDestroyTime = serializedObject.FindProperty("autoDestroyTime");
        combatAbility = serializedObject.FindProperty("<combatAbility>k__BackingField");

        pointAXOffsetBase = serializedObject.FindProperty("pointAXOffsetBase");
        pointAXOffsetDeviation = serializedObject.FindProperty("pointAXOffsetDeviation");
        pointAYOffsetBase = serializedObject.FindProperty("pointAYOffsetBase");
        pointAYOffsetDeviation = serializedObject.FindProperty("pointAYOffsetDeviation");
        pointBXOffsetBase = serializedObject.FindProperty("pointBXOffsetBase");
        pointBXOffsetDeviation = serializedObject.FindProperty("pointBXOffsetDeviation");
        pointBYOffsetBase = serializedObject.FindProperty("pointBYOffsetBase");
        pointBYOffsetDeviation = serializedObject.FindProperty("pointBYOffsetDeviation");

        minStep = serializedObject.FindProperty("minStep");
        maxStep = serializedObject.FindProperty("maxStep");
        timeToMaxStep = serializedObject.FindProperty("timeToMaxStep");

        straightProjectileDirection = serializedObject.FindProperty("straightProjectileDirection");
        loseSpeedTime = serializedObject.FindProperty("loseSpeedTime");

        minSpeedValue = serializedObject.FindProperty("minSpeedValue");
        speedStepValue = serializedObject.FindProperty("speedStepValue");
        timeStepValue = serializedObject.FindProperty("timeStepValue");
        targetingSuccessDistance = serializedObject.FindProperty("targetingSuccessDistance");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.LabelField("Shared Variables", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(overlapCollider, new GUIContent("Overlap Collider"));
        EditorGUILayout.PropertyField(projectileType, new GUIContent("Projectile Type"));
        EditorGUILayout.PropertyField(projectileTypeWhenParried, new GUIContent("Projectile Type When Parried"));
        EditorGUILayout.PropertyField(whatIsGround, new GUIContent("whatIsGround"));
        EditorGUILayout.PropertyField(projectileSpeed, new GUIContent("Projectile Speed"));
        EditorGUILayout.PropertyField(explosionDurationTime, new GUIContent("Explosion Duration Time"));
        EditorGUILayout.PropertyField(maximumDeflection, new GUIContent("Maximum Deflection"));
        EditorGUILayout.PropertyField(autoDestroyTime, new GUIContent("Auto Destroy Time"));
        EditorGUILayout.PropertyField(combatAbility, new GUIContent("Combat Ability"));

        switch (projectile.GetProjectileType())
        {
            case ProjectileType.Bazier:
                EditorGUILayout.PropertyField(pointAXOffsetBase, new GUIContent("Point A X Offset Base"));
                EditorGUILayout.PropertyField(pointAXOffsetDeviation, new GUIContent("Point A X Offset Deviation"));
                EditorGUILayout.PropertyField(pointAYOffsetBase, new GUIContent("Point A Y Offset Base"));
                EditorGUILayout.PropertyField(pointAYOffsetDeviation, new GUIContent("Point A Y Offset Deviation"));
                EditorGUILayout.PropertyField(pointBXOffsetBase, new GUIContent("Point B X Offset Base"));
                EditorGUILayout.PropertyField(pointBXOffsetDeviation, new GUIContent("Point B X Offset Deviation"));
                EditorGUILayout.PropertyField(pointBYOffsetBase, new GUIContent("Point B Y Offset Base"));
                EditorGUILayout.PropertyField(pointBYOffsetDeviation, new GUIContent("Point B Y Offset Deviation"));
                break;

            case ProjectileType.Follow:
                EditorGUILayout.PropertyField(minStep, new GUIContent("Min Step"));
                EditorGUILayout.PropertyField(maxStep, new GUIContent("Max Step"));
                EditorGUILayout.PropertyField(timeToMaxStep, new GUIContent("Time To Max Step"));
                break;

            case ProjectileType.Throw:
                EditorGUILayout.PropertyField(minSpeedValue, new GUIContent("Min Speed Value"));
                EditorGUILayout.PropertyField(speedStepValue, new GUIContent("Speed Step Value"));
                EditorGUILayout.PropertyField(timeStepValue, new GUIContent("Time Step Value"));
                EditorGUILayout.PropertyField(targetingSuccessDistance, new GUIContent("Targeting Success Distance"));
                break;

            case ProjectileType.Straight:
                EditorGUILayout.PropertyField(straightProjectileDirection, new GUIContent("Straight Projectile Direction"));
                EditorGUILayout.PropertyField(loseSpeedTime, new GUIContent("Lose Speed Time"));
                break;

            default: break;
        }

        if (projectile.overlapCollider.overlapBox)
        {
            BoxCollider2D boxCollider = projectile.gameObject.GetComponent<BoxCollider2D>() != null ? projectile.gameObject.GetComponent<BoxCollider2D>() : projectile.gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = projectile.overlapCollider.boxSize;
            boxCollider.offset = projectile.overlapCollider.centerTransform.localPosition;

            if (projectile.gameObject.layer.Equals(LayerMask.NameToLayer("ImmediateProjectile")))
            {
                projectile.GetComponent<Collider2D>().isTrigger = true;
            }
            else
            {
                projectile.GetComponent<Collider2D>().isTrigger = false;
            }
        }
        else
        {
            DestroyImmediate(projectile.gameObject.GetComponent<BoxCollider2D>(), true);
        }

        if (projectile.overlapCollider.overlapCircle)
        {
            CircleCollider2D boxCollider = projectile.gameObject.GetComponent<CircleCollider2D>() != null ? projectile.gameObject.GetComponent<CircleCollider2D>() : projectile.gameObject.AddComponent<CircleCollider2D>();
            boxCollider.radius = projectile.overlapCollider.circleRadius;
            boxCollider.offset = projectile.overlapCollider.centerTransform.localPosition;

            if (projectile.gameObject.layer.Equals(LayerMask.NameToLayer("ImmediateProjectile")))
            {
                projectile.GetComponent<Collider2D>().isTrigger = true;
            }
            else
            {
                projectile.GetComponent<Collider2D>().isTrigger = false;
            }
        }
        else
        {
            DestroyImmediate(projectile.gameObject.GetComponent<CircleCollider2D>(), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}