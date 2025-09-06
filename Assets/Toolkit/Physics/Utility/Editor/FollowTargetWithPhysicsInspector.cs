using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(FollowTargetWithPhysics))]
    public class FollowTargetWithPhysicsInspector : Editor
    {
        #region Variables

        private SerializedProperty targetProperty;
        private SerializedProperty settings;

        #endregion

        #region Init

        private void OnEnable() {
            targetProperty = serializedObject.FindProperty("target");
            settings = serializedObject.FindProperty("settings");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.PropertyField(targetProperty);
                EditorGUILayout.PropertyField(settings);
                if(settings.objectReferenceValue) {
                    var ftsettings = settings.objectReferenceValue as FollowTargetWithPhysicsSettings;
                    Draw(ftsettings);
                }
            }
        }

        private void Draw(FollowTargetWithPhysicsSettings s) {
            if(s == null)
                return;
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUI.IndentLevelScope(1)) {
                    EditorGUILayout.LabelField("Max Velocity", $"{s.MaxVelocity}");
                    EditorGUILayout.LabelField("Max Torque", $"{s.MaxTorque}");
                    EditorGUILayout.LabelField("Sensitivity", $"{s.Sensitivity}");
                    EditorGUILayout.LabelField("Predicition", $"{s.Prediction}");
                }
            }
        }

        #endregion
    }
}
