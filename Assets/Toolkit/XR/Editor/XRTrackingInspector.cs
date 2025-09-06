using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.XR
{
    [CustomEditor(typeof(XRTracking))]
    public class XRTrackingInspector : Editor
    {
        #region Variables

        private static List<ITracking> trackingComponents = new List<ITracking>();

        private SerializedProperty hand;
        private SerializedProperty highFrequencyUpdate;
        private SerializedProperty useOffset;
        private SerializedProperty offset;

        #endregion

        #region Init

        private void OnEnable() {
            hand = serializedObject.FindProperty("hand");
            highFrequencyUpdate = serializedObject.FindProperty("highFrequencyUpdate");
            useOffset = serializedObject.FindProperty("useOffset");
            offset = serializedObject.FindProperty("offset");
        }

        #endregion

        #region GUI

        public override void OnInspectorGUI() {
            var tracking = target as XRTracking;

            using(new EditorGUILayout.VerticalScope("box")) {
                if(tracking.IsCamera) {
                    EditorGUILayout.LabelField("Camera Tracking", EditorStylesUtility.BoldLabel);
                }
                else {
                    EditorGUILayout.PropertyField(hand);
                    EditorGUILayout.PropertyField(highFrequencyUpdate);
                }
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(useOffset);
                if(useOffset.boolValue)
                    using(new EditorGUI.IndentLevelScope(1))
                        EditorGUILayout.PropertyField(offset);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            tracking.GetComponentsInChildren(trackingComponents);
            if(trackingComponents.Count > 1) {
                EditorGUILayout.HelpBox($"Prefab has '{trackingComponents.Count}' tracking components", MessageType.Warning);
            }

        }

        #endregion
    }
}
