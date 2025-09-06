using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    [CustomEditor(typeof(Container))]
    public class ContainerEditor : Editor
    {
        private SerializedProperty containerNameProperty;
        private SerializedProperty keepAliveProperty;

        private void OnEnable() {
            containerNameProperty = serializedObject.FindProperty("containerName");
            keepAliveProperty = serializedObject.FindProperty("keepAlive");
        }

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUILayout.HorizontalScope()) {
                    containerNameProperty.stringValue = EditorGUILayout.TextField("Name", containerNameProperty.stringValue);
                    var hasName = !string.IsNullOrEmpty(containerNameProperty.stringValue);
                    if(!hasName) {
                        using(new EditorGUI.DisabledGroupScope(true)) {
                            var area = GUILayoutUtility.GetLastRect();
                            EditorGUI.LabelField(area, "Name", target.name);
                        }
                    }
                    if(GUILayout.Button("c", GUILayout.Width(16f))) {
                        var n = hasName ? containerNameProperty.stringValue : target.name;
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent($"Copied text\n'{n}'"), 2d);
                        EditorGUIUtility.systemCopyBuffer = n;
                    }
                }
                EditorGUILayout.PropertyField(keepAliveProperty);
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }

    }
}
