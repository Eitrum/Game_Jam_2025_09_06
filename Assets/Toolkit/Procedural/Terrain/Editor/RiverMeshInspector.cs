using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Procedural.Terrain
{
    [CustomEditor(typeof(RiverMesh))]
    public class RiverMeshInspector : Editor
    {
        private SerializedProperty layer;
        private SerializedProperty mesh;
        private SerializedProperty material;
        private SerializedProperty generatedMesh;

        private void OnEnable() {
            layer = serializedObject.FindProperty("layer");
            mesh = serializedObject.FindProperty("mesh");
            material = serializedObject.FindProperty("material");
            generatedMesh = serializedObject.FindProperty("generatedMesh");

            if(mesh.objectReferenceValue) {
                var path = AssetDatabase.GetAssetPath(mesh.objectReferenceValue);
                if(string.IsNullOrEmpty(path)) {
                    generatedMesh.boolValue = true;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(layer);

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Rendering", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(mesh);
                EditorGUILayout.PropertyField(material);
            }

            using(new EditorGUI.DisabledScope(mesh.objectReferenceValue == null)) {
                if(generatedMesh.boolValue) {
                    if(GUILayout.Button("Save Mesh", GUILayout.Width(100))) {
                        var path = AssetDatabase.GenerateUniqueAssetPath("Assets/RiverMesh.asset");
                        AssetDatabase.CreateAsset(mesh.objectReferenceValue, path);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        mesh.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                        generatedMesh.boolValue = false;
                    }
                }
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
