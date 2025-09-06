using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Health.Utility
{
    [CustomEditor(typeof(InstantiateOnDeath))]
    public class InstantiateOnDeathInspector : Editor
    {
        private SerializedProperty prefab;
        private SerializedProperty copyRotation;

        private void OnEnable() {
            prefab = serializedObject.FindProperty("prefab");
            copyRotation = serializedObject.FindProperty("copyRotation");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            HealthEditorUtility.DrawHealthComponentReference(target as InstantiateOnDeath);

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(prefab);
                EditorGUILayout.PropertyField(copyRotation);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(InstantiateOnDeathAdvanced))]
    public class InstantiateOnDeathAdvancedInspector : Editor
    {
        private SerializedProperty prefab;
        private SerializedProperty rotation;
        private SerializedProperty positionOffset;
        private SerializedProperty rotationOffset;

        private void OnEnable() {
            prefab = serializedObject.FindProperty("prefab");
            rotation = serializedObject.FindProperty("rotation");
            positionOffset = serializedObject.FindProperty("positionOffset");
            rotationOffset = serializedObject.FindProperty("rotationOffset");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            HealthEditorUtility.DrawHealthComponentReference(target as InstantiateOnDeathAdvanced);

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(prefab);
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(rotation);
                EditorGUILayout.PropertyField(positionOffset);
                EditorGUILayout.PropertyField(rotationOffset);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
