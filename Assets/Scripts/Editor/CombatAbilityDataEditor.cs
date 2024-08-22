using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CustomEditor(typeof(CombatAbilityData))]
public class CombatAbilityDataEditor : Editor
{
    private static List<Type> combatAbilityComponentTypes = new List<Type>();

    private CombatAbilityData combatAbilityData;

    private bool showAddCombatAbilityComponentsButtons;

    private void OnEnable()
    {
        combatAbilityData = target as CombatAbilityData;
        SceneView.duringSceneGui += DrawGizmos;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DrawGizmos;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        showAddCombatAbilityComponentsButtons = EditorGUILayout.Foldout(showAddCombatAbilityComponentsButtons, "Add Combat Ability Components");

        if (showAddCombatAbilityComponentsButtons)
        {
            foreach (Type combatAbilityComponentType in combatAbilityComponentTypes)
            {
                if (GUILayout.Button(combatAbilityComponentType.Name))
                {
                    CombatAbilityComponentData combatAbilityComponent = Activator.CreateInstance(combatAbilityComponentType) as CombatAbilityComponentData;

                    if (combatAbilityComponent == null)
                    {
                        Debug.LogError($"Tried to add Combat Ability Component of type \"{combatAbilityComponentType.Name}\", but failed to create instance.");
                    }

                    combatAbilityComponent.InitializeCombatAbilityData(combatAbilityData.numberOfStrokes);

                    combatAbilityData.AddComponent(combatAbilityComponent);

                    EditorUtility.SetDirty(combatAbilityData);
                }
            }
        }

        
        if (GUILayout.Button("Set Number of Strokes"))
        {
            foreach (CombatAbilityComponentData attackComponent in combatAbilityData.combatAbilityComponents)
            {
                attackComponent.InitializeCombatAbilityData(combatAbilityData.numberOfStrokes);
            }
        }
    }

    private void DrawGizmos(SceneView sceneView)
    {
        if (Application.isPlaying)
        {

        }
    }

    [DidReloadScripts]
    private static void OnRecompile()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        IEnumerable<Type> types = assemblies.SelectMany(assembly => assembly.GetTypes());
        IEnumerable<Type> filteredTypes = types.Where(type => type.IsSubclassOf(typeof(CombatAbilityComponentData)) && type.IsClass && !type.ContainsGenericParameters);
        combatAbilityComponentTypes = filteredTypes.ToList();
    }
}
