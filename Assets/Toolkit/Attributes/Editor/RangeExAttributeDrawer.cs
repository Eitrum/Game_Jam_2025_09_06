using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(RangeExAttribute))]
    public class RangeExAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var att = attribute as RangeExAttribute;

            switch(property.propertyType) {
                case SerializedPropertyType.Integer: {
                        EditorGUI.BeginChangeCheck();
                        int value = EditorGUI.IntSlider(position, label, property.intValue, Mathf.RoundToInt(att.Min), Mathf.RoundToInt(att.Max));
                        if(EditorGUI.EndChangeCheck()) {
                            var steps = Mathf.Max(Mathf.RoundToInt(att.Step), 1);
                            var min = Mathf.RoundToInt(att.Min);

                            var calc = value - min;
                            var rounded = Mathf.RoundToInt(calc / (float)steps);
                            property.intValue = Mathf.Clamp(min + rounded * steps, Mathf.RoundToInt(att.Min), Mathf.RoundToInt(att.Max));
                        }
                    }
                    break;

                case SerializedPropertyType.Float: {
                        EditorGUI.BeginChangeCheck();
                        float value = EditorGUI.Slider(position, label, property.floatValue, att.Min, att.Max);
                        if(EditorGUI.EndChangeCheck()) {
                            property.floatValue = value.Snap(att.Step);
                        }
                    }
                    break;

                default:
                    EditorGUI.HelpBox(position, $"Variable is not of a supported type", MessageType.Error);
                    break;
            }
        }
    }
}
