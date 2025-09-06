using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public static class EditorGUITextArea
    {
        #region Variables

        public const int DEFAULT_LINE_SIZE = 2;

        public static GUIStyle TextArea => EditorStylesUtility.TextArea;

        #endregion

        #region Draw Layout

        public static bool DrawLayout(SerializedProperty property)
            => DrawLayout(property, GUIContent.none);

        public static bool DrawLayout(SerializedProperty property, GUIContent content) {
            EditorGUI.BeginChangeCheck();
            string temp = null;
            if(content == GUIContent.none) {
                temp = EditorGUILayout.TextArea(property.stringValue, TextArea);
            }
            else {
                temp = EditorGUILayout.TextField(content, property.stringValue, TextArea);
            }
            var changed = EditorGUI.EndChangeCheck();
            if(changed) {
                property.stringValue = temp;
            }
            return changed;
        }

        public static bool DrawLayout(SerializedProperty property, string content)
            => DrawLayout(property, content, DEFAULT_LINE_SIZE);

        public static bool DrawLayout(SerializedProperty property, string content, int lines) {
            EditorGUI.BeginChangeCheck();
            string temp = null;
            if(string.IsNullOrEmpty(content)) {
                temp = EditorGUILayout.TextArea(property.stringValue, TextArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * lines));
            }
            else {
                temp = EditorGUILayout.TextField(content, property.stringValue, TextArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * lines));
            }
            var changed = EditorGUI.EndChangeCheck();
            if(changed) {
                property.stringValue = temp;
            }
            return changed;
        }

        #endregion
    }
}
