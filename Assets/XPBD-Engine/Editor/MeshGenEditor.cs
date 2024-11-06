using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGen))]
public class MeshGenEditor : Editor
{
    private SerializedProperty _vertexResolution;
    private SerializedProperty _width;
    private SerializedProperty _height;
    private SerializedProperty _depth;
    private SerializedProperty _gizmoSphereRadius;
    private SerializedProperty _meshTopology;

    private void OnEnable()
    {
        _vertexResolution = serializedObject.FindProperty("vertexResolution");
        _width = serializedObject.FindProperty("width");
        _height = serializedObject.FindProperty("height");
        _depth = serializedObject.FindProperty("depth");
        _gizmoSphereRadius = serializedObject.FindProperty("gizmoSphereRadius");
        _meshTopology = serializedObject.FindProperty("meshTopology");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MeshGen meshGen = (MeshGen)target;
        
        EditorGUILayout.PropertyField(_vertexResolution);
        EditorGUILayout.PropertyField(_meshTopology);
        EditorGUILayout.PropertyField(_width);
        EditorGUILayout.PropertyField(_height);
        EditorGUILayout.PropertyField(_depth);
        EditorGUILayout.PropertyField(_gizmoSphereRadius);
        
        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            meshGen.InitMesh();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }
}
