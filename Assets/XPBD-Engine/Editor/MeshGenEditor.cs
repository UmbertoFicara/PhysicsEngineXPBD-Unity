using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGen))]
public class MeshGenEditor : Editor
{
    private SerializedProperty _useGameObjectTransforms;
    private SerializedProperty _vertexResolution;
    private SerializedProperty _width;
    private SerializedProperty _height;
    private SerializedProperty _depth;
    private SerializedProperty _displayEdges;

    

    private void OnEnable()
    {
        _useGameObjectTransforms = serializedObject.FindProperty("useGameObjectTransforms");
        _vertexResolution = serializedObject.FindProperty("vertexResolution");
        _width = serializedObject.FindProperty("width");
        _height = serializedObject.FindProperty("height");
        _depth = serializedObject.FindProperty("depth");
        _displayEdges = serializedObject.FindProperty("displayEdges");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MeshGen meshGen = (MeshGen)target;
        
        // Toggle for switching generation modes
        EditorGUILayout.PropertyField(_useGameObjectTransforms, new GUIContent("Use GameObject Transforms"));

        if (_useGameObjectTransforms.boolValue)
        {
            EditorGUILayout.HelpBox("Vertices will be derived from GameObject transforms.", MessageType.Info);
        }
        else
        {
            // Only show procedural properties if not using transforms
            EditorGUILayout.LabelField("Procedural Mesh Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_vertexResolution, new GUIContent("Vertex Resolution"));
            EditorGUILayout.PropertyField(_width);
            EditorGUILayout.PropertyField(_height);
            EditorGUILayout.PropertyField(_depth);
        }

        EditorGUILayout.PropertyField(_displayEdges, new GUIContent("Display Mesh Edges"));

        if (GUILayout.Button("Refresh Mesh"))
        {
            serializedObject.ApplyModifiedProperties();
            meshGen.InitMesh();
            meshGen.PrintVerticesPositions();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
