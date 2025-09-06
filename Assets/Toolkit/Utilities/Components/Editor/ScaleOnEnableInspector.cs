using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(ScaleOnEnable))]
    public class ScaleOnEnableInspector : Editor
    {
        #region Variables

        private SerializedProperty overrideInitialScale;
        private SerializedProperty initialScale;
        private SerializedProperty targetScale;

        private SerializedProperty useConsistentDuration;
        private SerializedProperty durationPerUnit;
        private SerializedProperty easing;

        private static GUIContent durationContent = new GUIContent("Duration");
        private static GUIContent durationPerUnitContent = new GUIContent("Duration (per unit)");

        #endregion

        #region Init

        private void OnEnable() {
            overrideInitialScale = serializedObject.FindProperty("overrideInitialScale");
            initialScale = serializedObject.FindProperty("initialScale");
            targetScale = serializedObject.FindProperty("targetScale");

            useConsistentDuration = serializedObject.FindProperty("useConsistentDuration");
            durationPerUnit = serializedObject.FindProperty("durationPerUnit");
            easing = serializedObject.FindProperty("easing");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                DrawScaleSettings();
                EditorGUILayout.Space();
                DrawTimeSettings();
            }
        }

        private void DrawScaleSettings() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(overrideInitialScale);
                if(overrideInitialScale.boolValue)
                    using(new EditorGUI.IndentLevelScope(1))
                        EditorGUILayout.PropertyField(initialScale);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(targetScale);
                if(EditorGUI.EndChangeCheck()) {
                    UpdateScale();
                }
            }
        }

        private void DrawTimeSettings() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Time", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(useConsistentDuration);
                EditorGUILayout.PropertyField(durationPerUnit, (useConsistentDuration.boolValue ? durationContent : durationPerUnitContent));
                EditorGUILayout.PropertyField(easing);
            }
        }

        private void UpdateScale() {
            if(!Application.isPlaying)
                return;
            EditorApplication.delayCall += () => (target as ScaleOnEnable).Play();
        }

        #endregion
    }
}
