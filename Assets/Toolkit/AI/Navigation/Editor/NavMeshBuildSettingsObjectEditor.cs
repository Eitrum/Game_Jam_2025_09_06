using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;

namespace Toolkit.AI.Navigation
{
    [CustomEditor(typeof(NavMeshBuildSettingsObject))]
    public class NavMeshBuildSettingsObjectEditor : Editor
    {
        private SerializedProperty radiusProperty = null;
        private SerializedProperty heightProperty = null;
        private SerializedProperty stepHeightProperty = null;
        private SerializedProperty maxSlopeProperty = null;

        private void OnEnable() {
            radiusProperty = serializedObject.FindProperty("radius");
            heightProperty = serializedObject.FindProperty("height");
            stepHeightProperty = serializedObject.FindProperty("stepHeight");
            maxSlopeProperty = serializedObject.FindProperty("maxSlope");
        }

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField(target.name, EditorStyles.boldLabel);
                if(Application.isPlaying)
                    EditorGUILayout.LabelField("Agent Type ID: " + (target as NavMeshBuildSettingsObject).Settings.agentTypeID, EditorStyles.boldLabel);
                else
                    EditorGUILayout.LabelField("Agent Type ID: undefined", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(radiusProperty);
                EditorGUILayout.PropertyField(heightProperty);
                EditorGUILayout.PropertyField(stepHeightProperty);
                EditorGUILayout.PropertyField(maxSlopeProperty);
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                Rect agentDiagramRect = EditorGUILayout.GetControlRect(false, 120f);
                NavMeshEditorHelpers.DrawAgentDiagram(agentDiagramRect, radiusProperty.floatValue, heightProperty.floatValue, stepHeightProperty.floatValue, maxSlopeProperty.floatValue);
            }
        }
    }
}
