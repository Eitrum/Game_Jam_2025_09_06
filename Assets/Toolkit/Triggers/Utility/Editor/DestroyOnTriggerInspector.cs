using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(DestroyOnTrigger))]
    public class DestroyOnTriggerInspector : Editor
    {
        #region Variables
        
        private SerializedProperty optionalSources;
        private SerializedProperty target_prop;
        private SerializedProperty delay;

        #endregion

        #region Init

        private void OnEnable() {
            optionalSources = serializedObject.FindProperty("optionalSources");
            target_prop = serializedObject.FindProperty("target");
            delay = serializedObject.FindProperty("delay");
        }

        #endregion

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    TriggerEditorUtility.CheckForTriggerWithOptionalSources(target, optionalSources);
                    target_prop.objectReferenceValue = EditorGUILayout.ObjectField("Target (Optional)", target_prop.objectReferenceValue, typeof(Transform), true);
                    EditorGUILayout.PropertyField(delay);
                }
            }
        }
    }
}
