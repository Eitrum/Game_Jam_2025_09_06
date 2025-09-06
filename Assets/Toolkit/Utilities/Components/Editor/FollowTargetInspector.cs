using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(FollowTarget))]
    public class FollowTargetInspector : Editor
    {
        #region Variables

        private SerializedProperty target_prop;
        private SerializedProperty updateMode;
        private SerializedProperty space;
        private SerializedProperty mask;
        private SerializedProperty smoothingMode;
        private SerializedProperty smoothing;
        private SerializedProperty maxDistancePerSecond;
        private SerializedProperty maxRotationPerSecond;
        private SerializedProperty maxScalePerSecond;

        #endregion

        #region Init

        private void OnEnable() {
            target_prop = serializedObject.FindProperty("target");
            updateMode = serializedObject.FindProperty("updateMode");
            space = serializedObject.FindProperty("space");
            mask = serializedObject.FindProperty("mask");
            smoothingMode = serializedObject.FindProperty("smoothingMode");
            smoothing = serializedObject.FindProperty("smoothing");
            maxDistancePerSecond = serializedObject.FindProperty("maxDistancePerSecond");
            maxRotationPerSecond = serializedObject.FindProperty("maxRotationPerSecond");
            maxScalePerSecond = serializedObject.FindProperty("maxScalePerSecond");
        }

        #endregion


        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(target_prop);
                    if(Application.isPlaying && target is FollowTarget ft) {
                        EditorGUI.BeginChangeCheck();
                        var newUpdateMode = (UpdateMode)EditorGUILayout.EnumPopup(updateMode.displayName, ft.UpdateMode);
                        if(EditorGUI.EndChangeCheck()) {
                            if(ft.UpdateMode != newUpdateMode) {
                                ft.UpdateMode = newUpdateMode;
                            }
                        }
                    }
                    else
                        EditorGUILayout.PropertyField(updateMode);
                }

                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(space);
                    EditorGUILayout.PropertyField(mask);
                }
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(smoothingMode);
                    var mode = smoothingMode.intValue.ToEnum<FollowTarget.SmoothingMode>();
                    using(new EditorGUI.IndentLevelScope(1)) {
                        switch(mode) {
                            case FollowTarget.SmoothingMode.Distance:
                                EditorGUILayout.PropertyField(smoothing);
                                break;
                            case FollowTarget.SmoothingMode.Consistent:
                                EditorGUILayout.PropertyField(maxDistancePerSecond);
                                EditorGUILayout.PropertyField(maxRotationPerSecond);
                                EditorGUILayout.PropertyField(maxScalePerSecond);
                                break;
                            case FollowTarget.SmoothingMode.Mixed:
                                EditorGUILayout.PropertyField(smoothing);
                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(maxDistancePerSecond);
                                EditorGUILayout.PropertyField(maxRotationPerSecond);
                                EditorGUILayout.PropertyField(maxScalePerSecond);
                                break;
                        }
                    }
                }
            }
        }
    }
}
