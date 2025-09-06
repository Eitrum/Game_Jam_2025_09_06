using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    [CustomEditor(typeof(RotateSimple))]
    public class RotateSimpleInspector : Editor
    {
        #region Variables

        private SerializedProperty axis;
        private SerializedProperty degreesPerSecond;

        #endregion

        #region Init

        private void OnEnable() {
            axis = serializedObject.FindProperty("axis");
            degreesPerSecond = serializedObject.FindProperty("degreesPerSecond");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this, true)) {
                EditorGUILayout.PropertyField(axis);
                EditorGUILayout.PropertyField(degreesPerSecond);
            }

            using(new ToolkitEditorUtility.DebugScope()) {
                var rs = target as RotateSimple;
                EditorGUILayout.LabelField($"Angular Velocity {rs.AngularVelocity}");
                EditorGUILayout.LabelField($"\tRadians {rs.RadiansPerSecond}");
            }
        }

        #endregion
    }
}
