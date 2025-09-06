using UnityEditor;
using UnityEngine;
using System;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(PerformanceResultsAttribute))]
    public class PerformanceResultsAttributeDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(property.propertyType != SerializedPropertyType.Integer) {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType != SerializedPropertyType.Integer) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            EditorGUI.LabelField(position, label);
            var labelWidth = EditorGUIUtility.labelWidth;
            var fieldLocation = position.Pad(labelWidth, 0f, 0f, 0f);
            var perfAttribute = attribute as PerformanceResultsAttribute;
            var longValue = property.longValue;
            using(new EditorGUI.DisabledScope(true)) {
                switch(perfAttribute.Mode) {
                    case PerformanceResultsAttribute.ViewMode.Default:
                        GUI.Label(fieldLocation, $"{(longValue * StopwatchExtensions.TICKS_TO_MILLISECONDS):0.0000}ms\t\t[{longValue} ticks]");
                        break;
                    case PerformanceResultsAttribute.ViewMode.Ticks:
                        GUI.Label(fieldLocation, $"{(longValue)} ticks");
                        break;
                    case PerformanceResultsAttribute.ViewMode.Milliseconds:
                        GUI.Label(fieldLocation, $"{(longValue * StopwatchExtensions.TICKS_TO_MILLISECONDS):0.0000}ms");
                        break;
                }
            }
        }
    }
}
