using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit {
    [CustomPropertyDrawer(typeof(AttributeRequirement))]
    public class AttributeRequirementPropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var w = EditorGUIUtility.labelWidth;
            var labelContent = property.propertyPath.EndsWith(']') ? GUIContent.none : label;
            try {
                EditorGUIUtility.labelWidth *= 0.3f;
                var type = property.FindPropertyRelative("type");
                var stat = property.FindPropertyRelative("amount");

                position.SplitHorizontal(out Rect left, out Rect right, 0.4f, 24);
                left.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(left, type, labelContent);
                EditorGUI.PropertyField(right, stat, GUIContent.none);
            }
            finally {
                EditorGUIUtility.labelWidth = w;
            }
        }
    }
}
