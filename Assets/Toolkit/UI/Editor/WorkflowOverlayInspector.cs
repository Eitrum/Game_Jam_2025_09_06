using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.UI
{
    [CustomEditor(typeof(WorkflowOverlay))]
    public class WorkflowOverlayInspector : Editor
    {
        private SerializedProperty overrideTransparency;
        private SerializedProperty overrideTransparencyValue;

        private void OnEnable() {
            overrideTransparency = serializedObject.FindProperty("overrideTransparency");
            overrideTransparencyValue = serializedObject.FindProperty("overrideTransparencyValue");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            var wo = target as WorkflowOverlay;
            using(new EditorGUILayout.HorizontalScope("box")) {
                EditorGUI.BeginChangeCheck();
                var ovTran = EditorGUILayout.Toggle(overrideTransparency.boolValue, GUILayout.Width(20f));
                if(EditorGUI.EndChangeCheck()) {
                    overrideTransparency.boolValue = ovTran;
                    wo.UpdateTransparency(ovTran ? overrideTransparencyValue.floatValue : WorkflowOverlay.Transparency);
                }
                using(new EditorGUI.DisabledScope(!ovTran)) {
                    EditorGUI.BeginChangeCheck();
                    var ovTranVal = EditorGUILayout.Slider("Transparancy", overrideTransparencyValue.floatValue, 0f, 1f);
                    if(EditorGUI.EndChangeCheck()) {
                        overrideTransparencyValue.floatValue = ovTranVal;
                        wo.UpdateTransparency(ovTranVal);
                    }
                }
            }
            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
            if(GUILayout.Button("Open Window", GUILayout.Width(100f))) {
                WorkflowOverlayWindow.OpenEditor();
            }
        }
    }
}
