using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Unit {
    [CustomPropertyDrawer(typeof(AttributeEntry))]
    public class AttributeEntryPropertyDrawer : PropertyDrawer {

        private static GUIContent statContent = new GUIContent("Stat");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var stat = property.FindPropertyRelative("stat");
            return Mathf.Max(EditorGUIUtility.singleLineHeight, EditorGUI.GetPropertyHeight(stat));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var w = EditorGUIUtility.labelWidth;
            var labelContent = property.propertyPath.EndsWith(']') ? GUIContent.none : label;
            try {
                EditorGUIUtility.labelWidth *= 0.3f;
                var type = property.FindPropertyRelative("type");
                var stat = property.FindPropertyRelative("stat");

                position.SplitHorizontal(out Rect left, out Rect right, 0.4f, 24);
                left.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(left, type, labelContent);
                EditorGUI.PropertyField(right, stat, statContent);
            }
            finally {
                EditorGUIUtility.labelWidth = w;
            }
        }
    }
}
