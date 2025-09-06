using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(PowerOfTwoAttribute))]
    public class PowerOfTwoAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            var att = attribute as PowerOfTwoAttribute;
            switch(property.propertyType) {
                case SerializedPropertyType.Integer:
                    var iValue = property.intValue;
                    var index = iValue.GetFlagIndex();
                    index = EditorGUI.IntSlider(position, $"{label.text} - ({iValue})", index, (int)att.min, (int)Mathf.Clamp(att.max, 0, 31));
                    if(EditorGUI.EndChangeCheck())
                        property.intValue = 1 << index;
                    break;
                case SerializedPropertyType.Float:
                    var fValue = property.floatValue;
                    var logValue = Mathf.Log(fValue, 2f);
                    logValue = EditorGUI.Slider(position, $"{label.text} - ({fValue:0.0})", logValue, att.min, att.max);
                    if(EditorGUI.EndChangeCheck())
                        property.floatValue = Mathf.Pow(2f, logValue);
                    break;
                default:
                    EditorGUI.HelpBox(position, $"Field '{label.text}' is not of a supported type!", MessageType.Error);
                    break;
            }
        }
    }
}
