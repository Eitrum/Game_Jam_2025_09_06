using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.AI.Navigation
{
    [CustomEditor(typeof(NavMeshSourceMesh))]
    public class NavMeshSourceMeshEditor : Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            //var instancedProp = serializedObject.FindProperty("instanced");
            var areaProp = serializedObject.FindProperty("area");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(areaProp);
           // EditorGUILayout.PropertyField(instancedProp);
            EditorGUILayout.EndVertical();

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    [CustomEditor(typeof(NavMeshSourceModifier))]
    public class NavMeshSourceModifierEditor : Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();
           // var instancedProp = serializedObject.FindProperty("instanced");
            var areaProp = serializedObject.FindProperty("area");
            var sizeProp = serializedObject.FindProperty("size");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(areaProp);
            EditorGUILayout.PropertyField(sizeProp);
            //EditorGUILayout.PropertyField(instancedProp);
            EditorGUILayout.EndVertical();

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
