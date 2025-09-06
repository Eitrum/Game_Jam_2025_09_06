using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Game.Settings.UI {
    [CustomEditor(typeof(SettingsApplyOnClickUIEvent))]
    public class SettingsApplyOnClickUIEventInspector : Editor {

        private SerializedProperty operationType;
        private SerializedProperty mode;
        private SerializedProperty groupName;
        private SerializedProperty triggerSave;

        private void OnEnable() {
            operationType = serializedObject.FindProperty("operationType");
            mode = serializedObject.FindProperty("mode");
            groupName = serializedObject.FindProperty("groupName");
            triggerSave = serializedObject.FindProperty("triggerSave");
        }

        public override void OnInspectorGUI() {
            var comp = target as SettingsApplyOnClickUIEvent;
            var settingsBase = comp.GetComponentInParent<SettingsBase>();
            using(var e = new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.PropertyField(operationType);
                EditorGUILayout.PropertyField(mode);
                var opType = operationType.intValue.ToEnum<SettingsOperationType>();

                if(!settingsBase) {
                    EditorGUILayout.PropertyField(groupName);
                }
                if(opType == SettingsOperationType.Apply) {
                    EditorGUILayout.PropertyField(triggerSave);
                }

                e.DrawExtras();
            }
        }
    }
}
