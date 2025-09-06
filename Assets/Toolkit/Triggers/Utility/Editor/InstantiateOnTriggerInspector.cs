using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Trigger {
    [CustomEditor(typeof(InstantiateOnTrigger))]
    public class InstantiateOnTriggerInspector : Editor {
        #region Variables

        private SerializedProperty optionalSources;
        private SerializedProperty prefab;
        private SerializedProperty copyTransform;
        private SerializedProperty offset;
        private SerializedProperty rotationOffset;
        private SerializedProperty delay;

        #endregion

        #region Init

        private void OnEnable() {
            optionalSources = serializedObject.FindProperty("optionalSources");
            prefab = serializedObject.FindProperty("prefab");
            copyTransform = serializedObject.FindProperty("copyTransform");
            offset = serializedObject.FindProperty("offset");
            rotationOffset = serializedObject.FindProperty("rotationOffset");
            delay = serializedObject.FindProperty("delay");
        }

        #endregion

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                TriggerEditorUtility.CheckForTriggerWithOptionalSources(target, optionalSources);
                //EditorGUILayout.PropertyField(optionalSources);
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(prefab);
                    EditorGUILayout.PropertyField(delay);
                }
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(copyTransform);
                    using(new EditorGUI.IndentLevelScope(1)) {
                        EditorGUILayout.PropertyField(offset);
                        EditorGUILayout.PropertyField(rotationOffset);
                    }
                }
            }
        }
    }
}
