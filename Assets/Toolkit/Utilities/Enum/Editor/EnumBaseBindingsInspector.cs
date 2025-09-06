using System;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    [CustomEditor(typeof(EnumBaseBindings<>), editorForChildClasses: true)]
    public class EnumBaseBindingsInspector : Editor {

        private SerializedProperty key;
        private SerializedProperty enumType;
        private SerializedProperty values;

        private void OnEnable() {
            key = serializedObject.FindProperty("key");
            enumType = serializedObject.FindProperty("enumType");
            values = serializedObject.FindProperty("values");
        }

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                var baseBindings = (EnumBaseBindings)target;
                baseBindings.ClearCache();
                EditorGUILayout.PropertyField(key);
                using(new EditorGUI.DisabledScope(baseBindings.IsValid))
                    EditorGUILayout.PropertyField(enumType);
                if(baseBindings.IsValid) {
                    using(new EditorGUI.DisabledScope(Application.isPlaying)) {
                        VerifyElements();
                        DrawAllElements();
                    }
                }
            }
        }

        private void VerifyElements() {
            var baseBindings = (EnumBaseBindings)target;
            var list = FastEnum.GetNames(baseBindings.EnumType);
            var count = list.Count;
            values.arraySize = count;
            for(int i = 0; i < count; i++) {
                var ele = values.GetArrayElementAtIndex(i);
                var key = ele.FindPropertyRelative("Key");
                if(key.stringValue != list[i]) {
                    key.stringValue = list[i];
                }
            }
        }

        private void DrawAllElements() {
            using(new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField("Data", EditorStylesUtility.BoldLabel);

                if(ToolkitEditorUtility.IsHoldingAlt) {
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("Clear", GUILayout.Width(80))) {
                        values.arraySize = 1;
                    }
                }
            }

            using(new EditorGUI.IndentLevelScope(1)) {
                var count = values.arraySize;
                for(int i = 0; i < count; i++) {
                    var element = values.GetArrayElementAtIndex(i);
                    var keyProp = element.FindPropertyRelative("Key");
                    var valueProp = element.FindPropertyRelative("Value");
                    using(new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.LabelField(keyProp.stringValue, EditorStylesUtility.ItalicLabel, GUILayout.Width(100));
                        using(new EditorGUILayout.VerticalScope()) {
                            DrawValue(valueProp);
                        }
                    }
                }
            }
        }

        protected virtual void DrawValue(SerializedProperty valueProp) {
            EditorGUILayout.PropertyField(valueProp, GUIContent.none, true);
        }
    }
}
