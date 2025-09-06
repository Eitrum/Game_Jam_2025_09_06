using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(SetActiveOnTrigger))]
    public class SetActiveOnTriggerInspector : Editor
    {
        #region Variables
        
        private SerializedProperty optionalSources;
        private SerializedProperty setInactiveOnAwake;
        private SerializedProperty delay;
        private SerializedProperty toSetActive;
        private SerializedProperty toSetInactive;

        private GUIContent setInactiveOnAwakeContent = new GUIContent("Prepare objects", "Set all objects inverted-active/inactive at Awake");

        #endregion

        #region Init

        private void OnEnable() {
            optionalSources = serializedObject.FindProperty("optionalSources");
            setInactiveOnAwake = serializedObject.FindProperty("setInactiveOnAwake");
            delay = serializedObject.FindProperty("delay");
            toSetActive = serializedObject.FindProperty("toSetActive");
            toSetInactive = serializedObject.FindProperty("toSetInactive");
        }

        #endregion

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                TriggerEditorUtility.CheckForTriggerWithOptionalSources(target, optionalSources);

                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Config", EditorStylesUtility.BoldLabel);
                    using(new EditorGUI.IndentLevelScope(1)) {
                        EditorGUILayout.PropertyField(delay);
                        EditorGUILayout.PropertyField(setInactiveOnAwake, setInactiveOnAwakeContent);
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(toSetActive);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(toSetInactive);
            }
        }
    }
}
