using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Unit {
    [CustomPropertyDrawer(typeof(StatsModifier))]
    public class StatsModifierPropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var w = EditorGUIUtility.labelWidth;
            var labelContent = property.propertyPath.EndsWith(']') ? GUIContent.none : label;
            try {
                EditorGUIUtility.labelWidth *= 0.5f;
                var statType = property.FindPropertyRelative("statType");
                var valueType = property.FindPropertyRelative("valueType");
                var value = property.FindPropertyRelative("value");

                Rect left, mid, right;
                position.SplitHorizontal(out left, out mid, out right, 0.45f, 0.25f, 6f);

                EditorGUI.PropertyField(left, statType, labelContent);
                EditorGUI.PropertyField(mid, valueType, GUIContent.none);
                EditorGUI.PropertyField(right, value, GUIContent.none);
            }
            finally {
                EditorGUIUtility.labelWidth = w;
            }
        }
    }
}
