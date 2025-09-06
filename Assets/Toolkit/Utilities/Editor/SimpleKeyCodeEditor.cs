using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(KeyCode))]
    public class SimpleKeyCodeEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            var index = SimpleKeyCode.GetIndex((KeyCode)property.intValue);
            var newIndex = EditorGUI.Popup(position, label.text, index, SimpleKeyCode.PathArray);
            var keyCode = SimpleKeyCode.GetKeyCode(newIndex);
            // Check whether any changes was applied or not.
            if(EditorGUI.EndChangeCheck()) {
                property.intValue = (int)keyCode;
            }
        }

        #region Layout Draw

        public static KeyCode DrawKeyCodeEditor(KeyCode keyCode) {
            var index = SimpleKeyCode.GetIndex(keyCode);
            var newIndex = EditorGUILayout.Popup(index, SimpleKeyCode.PathArray);
            return SimpleKeyCode.GetKeyCode(newIndex);
        }

        public static KeyCode DrawKeyCodeEditor(string label, KeyCode keyCode) {
            var index = SimpleKeyCode.GetIndex(keyCode);
            var newIndex = EditorGUILayout.Popup(label, index, SimpleKeyCode.PathArray);
            return SimpleKeyCode.GetKeyCode(newIndex);
        }

        #endregion

        #region Rect Draw

        public static KeyCode DrawKeyCodeEditor(Rect rect, KeyCode keyCode) {
            var index = SimpleKeyCode.GetIndex(keyCode);
            var newIndex = EditorGUI.Popup(rect, index, SimpleKeyCode.PathArray);
            return SimpleKeyCode.GetKeyCode(newIndex);
        }

        public static KeyCode DrawKeyCodeEditor(Rect rect, string label, KeyCode keyCode) {
            var index = SimpleKeyCode.GetIndex(keyCode);
            var newIndex = EditorGUI.Popup(rect, label, index, SimpleKeyCode.PathArray);
            return SimpleKeyCode.GetKeyCode(newIndex);
        }

        #endregion
    }
}
