using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit.Procedural.Items
{
    [CustomEditor(typeof(PartRuntimeBuilder))]
    public class PartRuntimeBuilderEditor : Editor
    {
        private SerializedProperty assemblyProperty;
        private SerializedProperty applyOnStartProperty;
        private SerializedProperty containerProperty;

        private PartAssembly[] assemblies;
        private string[] assemblyNames;

        private void OnEnable() {
            assemblyProperty = serializedObject.FindProperty("assembly");
            applyOnStartProperty = serializedObject.FindProperty("applyOnStart");
            containerProperty = serializedObject.FindProperty("container");
            assemblies = AssetDatabaseUtility.LoadAssets<PartAssembly>();
            assemblyNames = assemblies.Select(x => x.name).Insert(0, "None").ToArray();
        }

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                var assembly = assemblyProperty.objectReferenceValue;
                int index = 0;
                for(int i = 0, length = assemblies.Length; i < length; i++) {
                    if(assembly == assemblies[i]) {
                        index = i + 1;
                        break;
                    }
                }
                var newIndex = EditorGUILayout.Popup(assemblyProperty.displayName, index, assemblyNames);
                if(index != newIndex) {
                    assemblyProperty.objectReferenceValue = newIndex > 0 ? assemblies[newIndex - 1] : null;
                }
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(applyOnStartProperty);
                EditorGUILayout.PropertyField(containerProperty);
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
