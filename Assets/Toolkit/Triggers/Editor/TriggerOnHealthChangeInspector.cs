using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(TriggerOnHealthChanged))]
    public class TriggerOnHealthChangeInspector : Editor
    {
        #region Variables

        private SerializedProperty healthChangedListener;
        private SerializedProperty repeatable;
        private SerializedProperty useRealtime;
        private SerializedProperty cooldown;

        #endregion

        #region Init

        private void OnEnable() {
            healthChangedListener = serializedObject.FindProperty("healthChangedListener");
            repeatable = serializedObject.FindProperty("repeatable");
            useRealtime = serializedObject.FindProperty("useRealtime");
            cooldown = serializedObject.FindProperty("cooldown");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(healthChangedListener);
                    EditorGUILayout.PropertyField(repeatable);
                    if(repeatable.boolValue) {
                        using(new EditorGUI.IndentLevelScope(1)) {
                            EditorGUILayout.PropertyField(cooldown);
                            EditorGUILayout.PropertyField(useRealtime);
                        }
                    }
                }

                TriggerEditorUtility.DrawEditorDebug(this, target);
            }
        }

        #endregion
    }
}
