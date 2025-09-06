using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(VelocityTrackingSimple))]
    public class VelocityTrackingSimpleInspector : Editor
    {
        public override void OnInspectorGUI() {
            var t = target as IVelocityTracking;
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUI.DisabledScope(true)) {
                    EditorGUILayout.LabelField("Velocity", EditorStylesUtility.BoldLabel);
                    using(new EditorGUI.IndentLevelScope(1)) {
                        EditorGUILayout.LabelField("Direction", $"{t.Velocity.normalized}");
                        EditorGUILayout.LabelField("Force", $"{t.Velocity.magnitude}");
                    }
                    EditorGUILayout.LabelField("Angular Velocity", $"{t.AngularVelocity}");
                }
            }
            if(Application.isPlaying)
                Repaint();
        }
    }
}
