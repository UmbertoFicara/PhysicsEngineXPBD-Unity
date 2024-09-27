using System;
using UnityEditor;
using UnityEngine;
using XPBD_Engine.Scripts.Physics;


[CustomEditor(typeof(PhysicsEngine))]
public class PhysicsEngineEditor : Editor
{
    private SerializedProperty _gravity;
    private SerializedProperty _worldBoundCenter;
    private SerializedProperty _worldBoundType;
    private SerializedProperty _worldBoundSize;
    private SerializedProperty _worldBoundRadius;
    private SerializedProperty _worldCapsulePos1;
    private SerializedProperty _worldCapsulePos2;
    
    private SerializedProperty _numSubsteps;
    private SerializedProperty _paused;
    private void OnEnable()
    {
        // Ottieni riferimenti ai campi serializzati
        _gravity = serializedObject.FindProperty("gravity");
        _worldBoundCenter = serializedObject.FindProperty("worldBoundCenter");
        _worldBoundType = serializedObject.FindProperty("worldBoundType");
        _worldBoundSize = serializedObject.FindProperty("worldBoundSize");
        _worldBoundRadius = serializedObject.FindProperty("worldBoundRadius");
        _worldCapsulePos1 = serializedObject.FindProperty("worldCapsulePos1");
        _worldCapsulePos2 = serializedObject.FindProperty("worldCapsulePos2");
        _numSubsteps = serializedObject.FindProperty("numSubsteps");
        _paused = serializedObject.FindProperty("paused");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        var physicalWorld = (PhysicsEngine)target;
        EditorGUILayout.PropertyField(_gravity);
        EditorGUILayout.PropertyField(_numSubsteps);
        EditorGUILayout.PropertyField(_paused);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("World Bound", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_worldBoundType,new GUIContent("Type"));
        if (physicalWorld.worldBoundType != WorldBoundType.None)
        {
            switch (physicalWorld.worldBoundType)
            {
                case WorldBoundType.Cube:
                    EditorGUILayout.PropertyField(_worldBoundCenter,new GUIContent("Position"));
                    EditorGUILayout.PropertyField(_worldBoundSize,new GUIContent("Size"));
                    break;
                case WorldBoundType.Sphere:
                    EditorGUILayout.PropertyField(_worldBoundCenter,new GUIContent("Position"));
                    EditorGUILayout.PropertyField(_worldBoundRadius,new GUIContent("Radius"));
                    break;
                case WorldBoundType.Capsule:
                    EditorGUILayout.PropertyField(_worldCapsulePos1,new GUIContent("Position 1"));
                    EditorGUILayout.PropertyField(_worldCapsulePos2,new GUIContent("Position 2"));
                    EditorGUILayout.PropertyField(_worldBoundRadius,new GUIContent("Radius"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Pause Physics"))
        {
            physicalWorld.SwitchPaused();
        }
        
        GUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
}
