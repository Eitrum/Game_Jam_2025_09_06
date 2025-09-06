using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(HSV))]
    public class HSVPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, label, true);
            position.height = EditorGUIUtility.singleLineHeight;
            var col = GetHSV(property);
            EditorGUI.ColorField(position.Pad(position.width / 2f, 0, 0, 0), GUIContent.none, col.Color);
        }

        public static HSV GetHSV(SerializedProperty property) {
            property.NextVisible(true);
            var h = property.floatValue;
            property.NextVisible(true);
            var s = property.floatValue;
            property.NextVisible(true);
            var v = property.floatValue;
            property.NextVisible(true);
            var a = property.floatValue;
            return new HSV(h, s, v, a);
        }
    }
}
