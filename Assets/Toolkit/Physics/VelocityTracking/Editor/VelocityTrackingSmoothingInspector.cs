using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(VelocityTrackingSmoothing))]
    public class VelocityTrackingSmoothingInspector : Editor
    {
        private SerializedProperty smoothing;

        private void OnEnable() {
            smoothing = serializedObject.FindProperty("smoothing");
        }

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(smoothing);
                    if(EditorGUI.EndChangeCheck()) {
                        (target as VelocityTrackingSmoothing).Smoothing = smoothing.floatValue;
                    }
                }
            }

            var t = target as IVelocityTracking;
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUI.DisabledScope(true)) {
                    EditorGUILayout.LabelField("Velocity", EditorStylesUtility.BoldLabel);
                    using(new EditorGUI.IndentLevelScope(1)) {
                        EditorGUILayout.LabelField("Direction", $"{t.Velocity.normalized}");
                        EditorGUILayout.LabelField("Force", $"{t.Velocity.magnitude}");
                    }
                    EditorGUILayout.LabelField("Angular Velocity", $"{t.AngularVelocity}");
                    EditorGUILayout.LabelField("Tracked Frames", $"{(target as VelocityTrackingSmoothing).TrackedFrames}");
                }
            }
            if(Application.isPlaying)
                Repaint();
        }
    }
}
