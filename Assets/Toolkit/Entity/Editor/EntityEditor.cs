using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomEditor(typeof(Entity))]
    public class EntityEditor : Editor
    {
        private SerializedProperty nameProperty;
        private SerializedProperty typeProperty;
        private GUIStyle helpStyle;


        private void OnEnable() {
            nameProperty = serializedObject.FindProperty("entityName");
            typeProperty = serializedObject.FindProperty("type");
        }


        public override void OnInspectorGUI() {
            serializedObject.UpdateIfRequiredOrScript();

            if(helpStyle == null) {
                helpStyle = new GUIStyle(EditorStyles.miniLabel);
                var col = helpStyle.normal.textColor;
                col *= 0.7f;
                helpStyle.normal.textColor = col;
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUI.DisabledGroupScope(Application.isPlaying)) {
                    EditorGUILayout.PropertyField(nameProperty);
                    if(string.IsNullOrEmpty(nameProperty.stringValue)) {
                        var area = GUILayoutUtility.GetLastRect();

                        EditorGUI.LabelField(area, " ", serializedObject.targetObject.name, helpStyle);
                    }

                    EditorGUILayout.PropertyField(typeProperty);
                }
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
