using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(TriggerOnTimeChange))]
    public class TriggerOnTimeChangeInspector : Editor
    {
        #region Variables

        private SerializedProperty repeatable;
        private SerializedProperty triggerAtTime;

        #endregion

        #region Init

        private void OnEnable() {
            repeatable = serializedObject.FindProperty("repeatable");
            triggerAtTime = serializedObject.FindProperty("triggerAtTime");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(repeatable);
                    EditorGUILayout.PropertyField(triggerAtTime);
                }

                TriggerEditorUtility.DrawEditorDebug(this, target);
            }
        }

        #endregion
    }
}
