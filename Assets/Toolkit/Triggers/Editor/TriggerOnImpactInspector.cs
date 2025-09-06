using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(TriggerOnImpact))]
    public class TriggerOnImpactInspector : Editor
    {
        #region Variables

        private SerializedProperty repeatable;
        private SerializedProperty minimumImpactVelocity;
        private SerializedProperty mask;

        #endregion

        #region Init

        private void OnEnable() {
            repeatable = serializedObject.FindProperty("repeatable");
            minimumImpactVelocity = serializedObject.FindProperty("minimumImpactVelocity");
            mask = serializedObject.FindProperty("mask");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(repeatable);
                    EditorGUILayout.PropertyField(minimumImpactVelocity);
                    EditorGUILayout.PropertyField(mask);
                }

                TriggerEditorUtility.DrawEditorDebug(this, target);
            }
        }

        #endregion
    }
}
